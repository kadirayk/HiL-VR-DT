using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Text;
using System;
using System.Threading.Tasks;
using Assets.Util;
using RosSharp.RosBridgeClient;
using Assets.PDDL;
using Assets.PDDL.Scripts;
using System.Collections;
using System.Net.Http;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class WorldState : MonoBehaviour, IListener, IProblemState
{
	private static readonly HttpClient client = new HttpClient();
	List<GameObject> gameObjects = new List<GameObject>();
	IList<GameObject> positions = new List<GameObject>();
	private float tableHeight;
	private float conveyorHeight;
	private string initialState;
	private string goalState;
	public GameObject Cube;
	float[] actionTarget = new float[3] { 180, 0, 0 };
	bool performAnimation = false;
	GameObject baseRotator;
	GameObject lowerArm;
	GameObject upperArm;
	GameObject hand;
	GameObject endEffector;
	Vector3 endPos;
	Queue<String> actionQueue = new Queue<string>();
	bool animationDone = false;
	Queue<RobotArmState> plannedMovements = new Queue<RobotArmState>();
	float[] startAngles = new float[3];
	bool shouldSolve = false;
	Queue<String> solutionLines;
	MovementRecorder mr;
	private static int SOLVE_TIMEOUT;
	private static string WORK_PATH;
	private Dictionary<GameObject, Boolean> putDownPositions = new Dictionary<GameObject, bool>(); // true if empty
	Queue<KeyValuePair<string, Vector3>> commands = new Queue<KeyValuePair<string, Vector3>>();

	Dictionary<string, Vector3> initialBlockPositions = new Dictionary<string, Vector3>();
	Dictionary<string, Quaternion> initialBlockRotations = new Dictionary<string, Quaternion>();
	UIStatus uiStatus;
	bool isSolveActive = false;
	Actuator actuator;
	ModeManager modeManager;

	public void Awake()
	{
		SOLVE_TIMEOUT = Configuration.getInt("SOLVE_TIMEOUT");
		WORK_PATH = Configuration.getString("WORK_PATH");
	}

	public void Initial()
	{
		StringBuilder str = new StringBuilder("(:init (HANDEMPTY) ");
		extractState(str);

		str.Append(")");
		initialState = str.ToString();
		Debug.Log(initialState);
		uiStatus.setInitial(initialState);

		foreach (GameObject movable in gameObjects)
		{
			initialBlockPositions[movable.name] = movable.transform.position;
			initialBlockRotations[movable.name] = movable.transform.rotation;
		}

	}

	private void extractState(StringBuilder str)
	{
		// find blocks at positions
		// (at block pos)
		foreach (GameObject movable in gameObjects)
		{
			foreach (GameObject pos in positions)
			{
				str.Append(findObjectAtPosition(movable, pos));
			}
		}

		// if no block at position, position is free
		// (free pos)
		foreach (GameObject pos in positions)
		{
			string label = "pos" + pos.GetComponentInChildren<TextMesh>().text;
			if (!str.ToString().Contains(label))
			{
				str.Append("(free ").Append(label).Append(")");
			}
		}

		// if block is not at position, it can be ontable. find blocks on table
		// (ontable block)
		foreach (GameObject block in gameObjects)
		{
			if (!str.ToString().Contains(block.name))
			{
				str.Append(findObjectsOnTable(block));
				//str.Append("(clear ").Append(label).Append(")").Append("(ontable ").Append(label).Append(")");
			}
		}

		// (on block1 block2)
		foreach (GameObject block in gameObjects)
		{
			//if (!str.ToString().Contains(block.name)) // if object is not at a pos, and not ontable, it must be stacked on another object
			//{
			if (!isObjectOnTableLevel(block) && !isObjectOnConveyorLevel(block))
			{ // higher than table level
				foreach (GameObject other in gameObjects)
				{
					if (!block.Equals(other))
					{
						str.Append(findBlockOnTop(block, other));
					}
				}
			}
			//	}
		}

		// find clear blocks, that dont have anything above them
		// clear(block)
		foreach (GameObject block in gameObjects)
		{
			Regex regex = new Regex("\\(on [A-Za-z_*0-9]* " + block.name + "\\)");
			Match match = regex.Match(str.ToString());
			if (!match.Success)
			{
				str.Append("(clear ").Append(block.name).Append(")");
			}
		}
	}

	private string findBlockOnTop(GameObject block, GameObject other)
	{

		Vector3 objMin = block.GetComponent<Renderer>().bounds.min;
		Vector3 objMax = block.GetComponent<Renderer>().bounds.max;
		Vector3 objCenter = block.GetComponent<Renderer>().bounds.center;
		Vector3 minA = other.GetComponent<Renderer>().bounds.min;
		Vector3 maxA = other.GetComponent<Renderer>().bounds.max;
		Vector3 centerA = other.GetComponent<Renderer>().bounds.center;

		StringBuilder str = new StringBuilder();

		if (Math.Abs(objCenter.x - centerA.x) < Math.Abs(objMax.x - objMin.x) / 2
			&& Math.Abs(objCenter.z - centerA.z) < Math.Abs(objMax.z - objMin.z) / 2
			&& Math.Abs(objMin.y - maxA.y) < 0.015)
		{
			string objName = block.gameObject.name;
			if (objName.Contains("(Clone)"))
			{
				objName = objName.Replace("(Clone)", "");
			}
			str.Append("(on ").Append(objName).Append(" ").Append(other.gameObject.name).Append(")");
		}

		return str.ToString();
	}

	public void Goal()
	{
		StringBuilder str = new StringBuilder("(:goal (and ");
		extractState(str);
		str.Append("))");
		goalState = str.ToString();
		Debug.Log(goalState);
		uiStatus.setGoal(goalState);

		// move blocks to initial positions

		ToInitialPosition();

		//foreach (GameObject obj in gameObjects)
		//{
		//	Destroy(obj);
		//}
		//loadSceneFromPDDL(initialState);
	}

	public void ToInitialPosition()
	{
		foreach (KeyValuePair<string, Vector3> entry in initialBlockPositions)
		{
			GameObject obj = GameObject.Find(entry.Key);
			obj.transform.position = entry.Value;
			obj.transform.rotation = initialBlockRotations[entry.Key];
		}
	}

	public void Solve()
	{
		isSolveActive = true;
	}

	IEnumerator MakeSolveRequest(System.Action<List<string>> callback = null) {
		string problem = createPDDLProblem(); // "(define (problem dobot01)\n(:domain BLOCKS)\n(:objects cube_red_0 cube_blue_0 cube_yellow_0 - block\nposyellow posred posblue - position)\n(:init (HANDEMPTY) (free posyellow)(free posred)(free posblue)(ontable cube_red_0)(ontable cube_blue_0)(ontable cube_yellow_0)(clear cube_red_0)(clear cube_blue_0)(clear cube_yellow_0))\n(:goal (and (at cube_yellow_0 posyellow)(at cube_red_0 posred)(at cube_blue_0 posblue)(clear cube_red_0)(clear cube_blue_0)(clear cube_yellow_0)))\n)";
		TextAsset file = Resources.Load("pddl-domain") as TextAsset;
		string domain = file.text.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");
		problem = problem.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");

		string json = "{ \"domain\":\"" + domain + "\", \"problem\":\"" + problem + "\"}";
		Debug.Log(json);
		var uwr = new UnityWebRequest("http://solver.planning.domains/solve", "POST");
		byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
		uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
		uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		uwr.SetRequestHeader("Content-Type", "application/json");

		//Send the request then wait here until it returns
		yield return uwr.SendWebRequest();

		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
			callback.Invoke(null);
		}
		else
		{
			var jsonObject = JObject.Parse(uwr.downloadHandler.text);
			var plan = jsonObject["result"]["plan"];
			List<string> solution = new List<string>();
			foreach (var action in plan)
			{
				solution.Add(action["name"].ToString());
			}
			callback.Invoke(solution);

		}
	}

	IEnumerator SolveCoroutine()
	{
		mr.SetAutomatedMode(true);
		uiStatus.setStatus("Solving");
		List<string> lines;
		StartCoroutine(MakeSolveRequest(solution=>
		{
			lines = solution;
			solutionLines = new Queue<string>();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string line in lines)
			{
				solutionLines.Enqueue(line);
				Debug.Log(line);
				stringBuilder.Append(line).Append("\n");
				//divideActions(line);
				//MovementRecorder mr = GameObject.FindObjectOfType<MovementRecorder>();
				//mr.SetRecordedMovements(plannedMovements);
				//mr.Replay();
			}
			uiStatus.setSolution(stringBuilder.ToString());
			shouldSolve = true;
		}));
		yield return null;

		/*
		DateTime modification = System.IO.File.GetLastWriteTime(WORK_PATH + @"PDDLSolver\solution.txt");
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		var startInfo = new System.Diagnostics.ProcessStartInfo
		{
			WorkingDirectory = WORK_PATH + @"PDDLSolver",
			WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
			FileName = "cmd.exe",
			Arguments = "/C python solver.py domain.pddl problem.pddl solution.txt"
		};
		process.StartInfo = startInfo;
		process.Start();
		DateTime newModification = modification;
		int i = 0;
		while (newModification == modification)
		{
			if (i > SOLVE_TIMEOUT)
			{
				Debug.LogError("No solution is found in " + i + " seconds");
				yield return null;
			}
			yield return new WaitForSeconds(1);
			newModification = System.IO.File.GetLastWriteTime(WORK_PATH + @"PDDLSolver\solution.txt");
			i++;
		}
		Debug.Log("solved in: " + i + " seconds");
		process.Close();
		yield return new WaitForSeconds(2);
		string[] lines = System.IO.File.ReadAllLines(WORK_PATH + @"PDDLSolver\solution.txt");
		//List<string> cleanLines = cleanUpLines(lines);
		//GameObject cube = GameObject.Find("RedCube1");
		//Debug.Log("redcube1 init x:" + cube.transform.position.x + " y:" + cube.transform.position.y + " z:" + cube.transform.position.z);
		solutionLines = new Queue<string>();
		StringBuilder stringBuilder = new StringBuilder();
		*/
		/*
		foreach (string line in lines)
		{
			solutionLines.Enqueue(line);
			Debug.Log(line);
			stringBuilder.Append(line).Append("\n");
			//divideActions(line);
			//MovementRecorder mr = GameObject.FindObjectOfType<MovementRecorder>();
			//mr.SetRecordedMovements(plannedMovements);
			//mr.Replay();
		}
		uiStatus.setSolution(stringBuilder.ToString());
		shouldSolve = true;
		//cube = GameObject.Find("RedCube1");
		//Debug.Log("redcube1 end x:" + cube.transform.position.x + " y:" + cube.transform.position.y + " z:" + cube.transform.position.z);
		*/
	}

	private async void waiter()
	{
		MovementRecorder mr = GameObject.FindObjectOfType<MovementRecorder>();
		mr.SetRecordedMovements(plannedMovements);
		Task task = Task.Run(() => mr.Replay());
		mr.Replay();
		while (!mr.isReplayDone())
		{
			await Task.Delay(25);
		}
	}

	private void divideActions(String action)
	{

		if (action.StartsWith("(place"))
		{
			action = action.Replace("(place ", "");
			action = action.Replace(")", "");
			string blockName = action.Split(new string[] { " " }, StringSplitOptions.None)[0].Trim();
			string posName = action.Split(new string[] { " " }, StringSplitOptions.None)[1].Trim();
			placeBlock(blockName, posName);

		}
		else if (action.StartsWith("(pick-up"))
		{
			action = action.Replace("(pick-up ", "");
			action = action.Replace(")", "");
			String block = action.Trim();
			pickupBlock(block);
		}
		else if (action.StartsWith("(put-down"))
		{
			action = action.Replace("(put-down ", "");
			action = action.Replace(")", "");
			String block = action.Trim();
			putdownBlock(block);
		}
		else if (action.StartsWith("(stack"))
		{
			action = action.Replace("(stack ", "");
			action = action.Replace(")", "");
			string blockInHand = action.Split(new string[] { " " }, StringSplitOptions.None)[0].Trim();
			string targetBlock = action.Split(new string[] { " " }, StringSplitOptions.None)[1].Trim();
			stackBlock(blockInHand, targetBlock);
		}
		else if (action.StartsWith("(unstack"))
		{
			action = action.Replace("(unstack ", "");
			action = action.Replace(")", "");
			string targetBlock = action.Split(new string[] { " " }, StringSplitOptions.None)[0].Trim();
			pickupBlock(targetBlock);
		}
		//action = action.Replace("move(", "");
		//action = action.Replace(")", "");
		//string from = action.Split(new string[] { "," }, StringSplitOptions.None)[1].Trim();
		//string to = action.Split(new string[] { "," }, StringSplitOptions.None)[2].Trim();
		////actionQueue.Enqueue("reset");
		//actionQueue.Enqueue(from);
		////actionQueue.Enqueue("reset");
		//actionQueue.Enqueue(to);
	}

	private void putdownBlock(string blockName)
	{
		GameObject targetBlock = null;
		foreach (GameObject block in gameObjects)
		{
			if (blockName.Equals(block.name, StringComparison.InvariantCultureIgnoreCase))
			{
				targetBlock = block;
			}
		}

		if (targetBlock == null)
		{
			throw new Exception("Target Block not Found in gameobjects");
		}

		foreach (GameObject pos in putDownPositions.Keys)
		{
			if (putDownPositions[pos])
			{
				//float blockHeight = targetBlock.GetComponent<Renderer>().bounds.max.y - targetBlock.GetComponent<Renderer>().bounds.min.y;
				//float blockTop = block.GetComponent<Renderer>().bounds.max.y - block.GetComponent<Renderer>().bounds.min.y;
				Vector3 target = new Vector3(pos.transform.position.x, pos.transform.position.y + 0.027f, pos.transform.position.z);
				//Debug.Log("center x:" + pos.GetComponent<Renderer>().bounds.center.x + " y:" + pos.GetComponent<Renderer>().bounds.center.y + " z:" + pos.GetComponent<Renderer>().bounds.center.z);
				//Debug.Log("max x:" + pos.GetComponent<Renderer>().bounds.max.x + " y:" + pos.GetComponent<Renderer>().bounds.max.y + " z:" + pos.GetComponent<Renderer>().bounds.max.z);
				//Debug.Log("min x:" + pos.GetComponent<Renderer>().bounds.min.x + " y:" + pos.GetComponent<Renderer>().bounds.min.y + " z:" + pos.GetComponent<Renderer>().bounds.min.z);
				//Debug.Log("pos x:" + pos.transform.position.x + " y:" + pos.transform.position.y + " z:" + pos.transform.position.z);
				moveToPos(target, true);
				drop();
				jump(false);
				putDownPositions[pos] = false;
				break;
			}
		}
	}

	private void stackBlock(string blockInHandName, string targetBlockName)
	{
		GameObject blockInHand = null;
		GameObject targetBlock = null;
		foreach (GameObject block in gameObjects)
		{
			if (blockInHandName.Equals(block.name, StringComparison.InvariantCultureIgnoreCase))
			{
				blockInHand = block;
			}
		}

		if (blockInHand == null)
		{
			throw new Exception("InHand Block not Found in gameobjects");
		}

		foreach (GameObject block in gameObjects)
		{
			if (targetBlockName.Equals(block.name, StringComparison.InvariantCultureIgnoreCase))
			{
				targetBlock = block;
			}
		}

		if (targetBlock == null)
		{
			throw new Exception("Target Block not Found in gameobjects");
		}


		float blockHeight = 0.029f; //blockInHand.GetComponent<Renderer>().bounds.max.y - blockInHand.GetComponent<Renderer>().bounds.min.y;
		float halfHeight = 0.0125f;
		Vector3 target = new Vector3(targetBlock.transform.position.x, targetBlock.transform.position.y + halfHeight + blockHeight, targetBlock.transform.position.z);
		commands.Enqueue(new KeyValuePair<string, Vector3>("stack", target));
		moveToPos(target, true);
		drop();
		jump(false);

	}

	private void jump(Boolean suction)
	{
		float diff = 1.0f - endPos.y;
		endPos += new Vector3(0, diff, 0);
		moveToPos(endPos, suction);
	}

	private void drop()
	{
		for (int i = 0; i < 10; i++)
		{
			RobotArmState state = new RobotArmState(startAngles[0], startAngles[1], startAngles[2], false, endPos);
			plannedMovements.Enqueue(state);
		}
	}

	private void pickupBlock(String blockName)
	{
		foreach (GameObject block in gameObjects)
		{
			if (blockName.Equals(block.name, StringComparison.InvariantCultureIgnoreCase))
			{
				//float blockTop = block.GetComponent<Renderer>().bounds.max.y - block.GetComponent<Renderer>().bounds.min.y;
				Vector3 target = new Vector3(block.transform.position.x - 0.003f, block.transform.position.y + 0.013f, block.transform.position.z);
				commands.Enqueue(new KeyValuePair<string, Vector3>("pick-up", target));
				jump(true);
				moveToPos(target + new Vector3(0, 0.06f, 0), true);
				moveToPos(target, true);
				jump(true);
			}
		}
	}

	private void placeBlock(String blockName, String posName)
	{
		GameObject targetBlock = null;
		foreach (GameObject block in gameObjects)
		{
			if (blockName.Equals(block.name, StringComparison.InvariantCultureIgnoreCase))
			{
				targetBlock = block;
			}
		}

		if (targetBlock == null)
		{
			throw new Exception("Target Block not Found in gameobjects");
		}

		foreach (GameObject pos in positions)
		{
			string label = "pos" + pos.GetComponentInChildren<TextMesh>().text;
			if (posName.Equals(label, StringComparison.InvariantCultureIgnoreCase))
			{
				//float blockHeight = targetBlock.GetComponent<Renderer>().bounds.max.y - targetBlock.GetComponent<Renderer>().bounds.min.y;
				//float blockTop = block.GetComponent<Renderer>().bounds.max.y - block.GetComponent<Renderer>().bounds.min.y;
				Vector3 target = new Vector3(pos.transform.position.x, pos.transform.position.y + 0.027f, pos.transform.position.z);
				//Debug.Log("center x:" + pos.GetComponent<Renderer>().bounds.center.x + " y:" + pos.GetComponent<Renderer>().bounds.center.y + " z:" + pos.GetComponent<Renderer>().bounds.center.z);
				//Debug.Log("max x:" + pos.GetComponent<Renderer>().bounds.max.x + " y:" + pos.GetComponent<Renderer>().bounds.max.y + " z:" + pos.GetComponent<Renderer>().bounds.max.z);
				//Debug.Log("min x:" + pos.GetComponent<Renderer>().bounds.min.x + " y:" + pos.GetComponent<Renderer>().bounds.min.y + " z:" + pos.GetComponent<Renderer>().bounds.min.z);
				//Debug.Log("pos x:" + pos.transform.position.x + " y:" + pos.transform.position.y + " z:" + pos.transform.position.z);
				commands.Enqueue(new KeyValuePair<string, Vector3>("place", target));
				moveToPos(target + new Vector3(0, 0.03f, 0), true);
				moveToPos(target, true);
				drop();
				jump(false);
			}
		}
	}

	private void moveToPos(Vector3 target, Boolean suction)
	{
		InverseKinematics ik = GameObject.FindObjectOfType<InverseKinematics>();
		float[] angleTarget = ik.GetAnglesForPosition(target);
		angleTarget = ik.GetAnglesForPositionCorrection(target, angleTarget);
		angleTarget = ik.GetAnglesForPositionCorrection(target, angleTarget);
		float baseStart = startAngles[0];
		float l2Start = startAngles[1];
		float l3Start = startAngles[2];
		float baseAngle = 0;
		if (baseStart > 180)
		{
			baseAngle = baseStart - 360;
		}
		else
		{
			baseAngle = baseStart;
		}

		float baseRotatorDifference = angleTarget[0] - baseAngle;
		float lowerArmDifference = angleTarget[1] - l2Start;
		float upperArmDifference = angleTarget[2] - l3Start;


		while (Math.Abs(baseRotatorDifference) > 0.1250 && Math.Abs(lowerArmDifference) > 0.125 && Math.Abs(upperArmDifference) > 0.125)
		{
			//Debug.Log(baseRotatorDifference);
			if (baseRotatorDifference < 0)
			{
				baseAngle -= 0.25f;
			}
			else
			{
				baseAngle += 0.25f;
			}
			if (lowerArmDifference < 0)
			{
				l2Start -= 0.25f;
			}
			else
			{
				l2Start += 0.25f;
			}
			if (upperArmDifference < 0)
			{
				l3Start -= 0.25f;
			}
			else
			{
				l3Start += 0.25f;
			}

			RobotArmState state = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
			plannedMovements.Enqueue(state);
			baseRotatorDifference = angleTarget[0] - baseAngle;
			lowerArmDifference = angleTarget[1] - l2Start;
			upperArmDifference = angleTarget[2] - l3Start;
		}

		while (Math.Abs(baseRotatorDifference) > 0.250)
		{
			//Debug.Log(baseRotatorDifference);
			if (baseRotatorDifference < 0)
			{
				baseAngle -= 0.5f;
				RobotArmState state = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				plannedMovements.Enqueue(state);
			}
			else
			{
				baseAngle += 0.5f;
				RobotArmState state = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				plannedMovements.Enqueue(state);
			}
			baseRotatorDifference = angleTarget[0] - baseAngle;
		}

		while (Math.Abs(lowerArmDifference) > 0.25)
		{
			if (lowerArmDifference < 0)
			{
				l2Start -= 0.5f;
				RobotArmState state = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				plannedMovements.Enqueue(state);
			}
			else
			{
				l2Start += 0.5f;
				RobotArmState state = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				plannedMovements.Enqueue(state);
			}
			lowerArmDifference = angleTarget[1] - l2Start;
		}

		while (Math.Abs(upperArmDifference) > 0.25)
		{
			if (upperArmDifference < 0)
			{
				l3Start -= 0.5f;
				RobotArmState state = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				plannedMovements.Enqueue(state);
			}
			else
			{
				l3Start += 0.5f;
				RobotArmState state = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				plannedMovements.Enqueue(state);
			}
			upperArmDifference = angleTarget[2] - l3Start;
		}


		startAngles = angleTarget;
		endPos = target;
	}


	public void loadSceneFromPDDL(string pddlState)
	{
		//(:init (at Cube1 posA)(at Cube2 posB)(free posC))
		int cursor = pddlState.IndexOf("(at");
		while (cursor != -1)
		{
			int closing = pddlState.IndexOf(")", cursor);
			string atomic = pddlState.Substring(cursor + 1, closing - cursor - 1);
			realizeAtomic(atomic);
			cursor = pddlState.IndexOf("(at", closing);
		}
	}

	private void realizeAtomic(string atomic)
	{
		Debug.Log("atomic: " + atomic);
		string cubeName = atomic.Split(null)[1];
		string posName = atomic.Split(null)[2];
		foreach (GameObject position in positions)
		{
			string label = "pos" + position.GetComponentInChildren<TextMesh>().text;
			if (label.Equals(posName, System.StringComparison.InvariantCultureIgnoreCase))
			{
				Vector3 pos = position.transform.position + new Vector3(0f, 0.04f, 0f);

				foreach (GameObject pddlObj in gameObjects)
				{
					if (pddlObj.name.Equals(cubeName))
					{
						pddlObj.transform.position = pos;
					}
				}
			}
		}
	}

	public string createPDDLProblem()
	{
		StringBuilder problem = new StringBuilder("(define (problem dobot01)\n");
		problem.Append("(:domain BLOCKS)\n");
		//problem.Append("(:objects cube1 cube2 - cube posA posB posC - position)\n");
		problem.Append("(:objects ");
		foreach (GameObject movable in gameObjects)
		{
			problem.Append(movable.gameObject.name.Replace("(Clone)", "")).Append(" ");
		}
		problem.Append("- block\n");
		foreach (GameObject pos in positions)
		{
			string label = "pos" + pos.GetComponentInChildren<TextMesh>().text;
			problem.Append(label).Append(" ");
		}
		problem.Append("- position)\n");
		problem.Append(initialState).Append("\n");
		problem.Append(goalState).Append("\n");
		problem.Append(")");
		return problem.ToString();
	}

	private string findObjectsOnTable(GameObject pddlObj)
	{
		StringBuilder str = new StringBuilder();
		if (isObjectOnTableLevel(pddlObj))
		{
			str.Append("(ontable ").Append(pddlObj.name).Append(")");
		}
		return str.ToString();
	}

	private Boolean isObjectOnTableLevel(GameObject obj)
	{
		Vector3 objMin = obj.GetComponent<Renderer>().bounds.min;
		float baseVerticalPosition = objMin.y;
		if (Math.Abs(baseVerticalPosition - tableHeight) < 0.015)
		{
			return true;
		}
		return false;
	}

	private Boolean isObjectOnConveyorLevel(GameObject obj)
	{
		Vector3 objMin = obj.GetComponent<Renderer>().bounds.min;
		float baseVerticalPosition = objMin.y;
		if (Math.Abs(baseVerticalPosition - conveyorHeight) < 0.015)
		{
			return true;
		}
		return false;
	}

	private string findObjectAtPosition(GameObject pddlObj, GameObject position)
	{
		Vector3 objMin = pddlObj.GetComponent<Renderer>().bounds.min;
		Vector3 objMax = pddlObj.GetComponent<Renderer>().bounds.max;
		Vector3 objCenter = pddlObj.GetComponent<Renderer>().bounds.center;
		Vector3 minA = position.GetComponent<Renderer>().bounds.min;
		Vector3 maxA = position.GetComponent<Renderer>().bounds.max;
		Vector3 centerA = position.GetComponent<Renderer>().bounds.center;

		StringBuilder str = new StringBuilder();
		if (isObjectOnConveyorLevel(pddlObj)) // previosly isObjectOnTableLevel  obj is immediately above position, position is always on table
		{
			if (Math.Abs(objCenter.x - centerA.x) < Math.Abs(objMax.x - objMin.x) / 2 && Math.Abs(objCenter.z - centerA.z) < Math.Abs(objMax.z - objMin.z) / 2)
			{
				string objName = pddlObj.gameObject.name;
				if (objName.Contains("(Clone)"))
				{
					objName = objName.Replace("(Clone)", "");
				}
				str.Append("(at ").Append(objName).Append(" pos").Append(position.GetComponentInChildren<TextMesh>().text).Append(")");
			}
		}
		return str.ToString();
	}

	public void Register(GameObject gameObject)
	{
		gameObjects.Add(gameObject);
		modeManager.Register(gameObject);
	}

	public void Unregister(GameObject gameObject)
	{
		gameObjects.Remove(gameObject);
		modeManager.Unregister(gameObject);
	}

	// Start is called before the first frame update
	void Start()
	{

		//Debug.Log("A:" + UnityUtil.PositionToString(UnityUtil.DobotArmToVR(new Vector3(260,100,-15))));
		//Debug.Log("B:" + UnityUtil.PositionToString(UnityUtil.DobotArmToVR(new Vector3(155, 100, -15))));
		//Debug.Log("C:" + UnityUtil.PositionToString(UnityUtil.DobotArmToVR(new Vector3(155, 100, -70))));
		uiStatus = GameObject.FindObjectOfType<UIStatus>();
		actuator = GameObject.FindObjectOfType<Actuator>();
		positions = GameObject.FindGameObjectsWithTag("Position");
		foreach (GameObject pos in positions)
		{
			Debug.Log("converted:" + UnityUtil.PositionToString(UnityUtil.VRToDobotArm(pos.transform.position)));
		}

		IList<GameObject> putPos = GameObject.FindGameObjectsWithTag("PutDownPos");
		foreach (GameObject obj in putPos)
		{
			putDownPositions.Add(obj, true);
		}
		baseRotator = GameObject.Find("magician_link_1");
		lowerArm = GameObject.Find("magician_link_2");
		upperArm = GameObject.Find("magician_link_3");
		hand = GameObject.Find("magician_link_4");
		endEffector = GameObject.Find("magician_end_effector");
		endPos = endEffector.transform.position;
		//gameObjects.AddRange(GameObject.FindGameObjectsWithTag("PDDLObject"));
		Debug.Log(UnityUtil.PositionToString(endPos));
		//Debug.Log(UnityUtil.PositionToString(UnityUtil.DobotArmToVR(new Vector3(0, 147, 135))));
		//Debug.Log(UnityUtil.PositionToString(UnityUtil.DobotArmToVR(new Vector3(147, 0, 135))));
		Debug.Log(UnityUtil.PositionToString(UnityUtil.VRToDobotArm(endPos)));
		GameObject table = GameObject.Find("Table");
		tableHeight = table.GetComponent<Renderer>().bounds.max.y;
		GameObject conveyor = GameObject.Find("Belt");
		conveyorHeight = conveyor.GetComponent<Renderer>().bounds.max.y;
		mr = GameObject.FindObjectOfType<MovementRecorder>();
		modeManager = GameObject.FindObjectOfType<ModeManager>();

		ServiceCaller sc = ServiceCaller.getInstance();
		sc.SetPTPCmd(1, 147, 0, 135, 0, false);


	}

	float timer = 0.0f;
	bool once = true;
	bool isInitialStateRegistered = false;
	bool calcTime = true;
	int seconds = 0;

	public void Execute()
	{
		actuator.executeCommands(commands);
	}

	// Update is called once per frame
	void Update()
	{
		if (calcTime)
		{
			timer += Time.deltaTime;
			seconds = (int)timer % 60;
		}
		if (once && seconds >= 4) // 10 for actual camera input
		{
			LoadGraspDetectionSubscriber graspDetection = GameObject.FindObjectOfType<LoadGraspDetectionSubscriber>();
			graspDetection.updateObjectVisualization();
			once = false;
		}
		if (!isInitialStateRegistered && seconds >= 6)
		{
			// register initial state
			Initial();
			isInitialStateRegistered = true;
			calcTime = false;
		}

		if (isSolveActive)
		{
			StartCoroutine(SolveCoroutine());
			isSolveActive = false;
		}

		if (shouldSolve)
		{
			if (solutionLines.Count != 0 && mr.isReplayDone())
			{
				string line = solutionLines.Dequeue();
				Debug.Log("Deququed: " + line);
				uiStatus.setStatus(line);
				divideActions(line);

				Queue<RobotArmState> copiedMovements = new Queue<RobotArmState>(plannedMovements);
				mr.SetRecordedMovements(plannedMovements);
				mr.Replay();
			}
			else if (solutionLines.Count == 0 && mr.isReplayDone())
			{
				Debug.Log("should solve replay done");
				shouldSolve = false;
				EndEffectorCollisionController cd = GameObject.FindObjectOfType<EndEffectorCollisionController>();
				cd.AutomatedMode(false);
				uiStatus.setStatus("Solution Done");

				//foreach (GameObject block in gameObjects)
				//{
				//	block.GetComponent<Rigidbody>().useGravity = true;
				//}
			}

		}
	}
}