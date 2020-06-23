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

public class WorldState : MonoBehaviour, IListener, IProblemState
{
	List<GameObject> gameObjects = new List<GameObject>();
	IList<GameObject> positions = new List<GameObject>();
	private float tableHeight;
	private string initialState;
	private string goalState;
	public GameObject Cube;
	float[] actionTarget = new float[3] { 180, 0, 0 };
	bool performAnimation = false;
	public GameObject dobotLoaderBaseRotator;
	public GameObject dobotLoaderLowerArm;
	public GameObject dobotLoaderUpperArm;
	public GameObject dobotLoaderHand;
	public GameObject dobotLoaderEndEffector;
	InverseKinematics dobotLoaderIK;
	public GameObject dobotRailBaseRotator;
	public GameObject dobotRailLowerArm;
	public GameObject dobotRailUpperArm;
	public GameObject dobotRailHand;
	public GameObject dobotRailEndEffector;
	InverseKinematics dobotRailIK;
	Vector3 dobotLoaderEndPos;
	Vector3 dobotRailEndPos;
	Queue<String> actionQueue = new Queue<string>();
	bool animationDone = false;
	Queue<RoboticSystemState> plannedMovements = new Queue<RoboticSystemState>();
	float[] dobotLoaderStartAngles = new float[3];
	float[] dobotRailStartAngles = new float[3];
	bool shouldSolve = false;
	Queue<String> solutionLines;
	MovementRecorder movementRecorder;
	private static readonly int SOLVE_TIMEOUT = Configuration.getInt("SOLVE_TIMEOUT");
	private static readonly string WORK_PATH = Configuration.getString("WORK_PATH");
	private Dictionary<GameObject, Boolean> putDownPositions = new Dictionary<GameObject, bool>(); // true if empty
	Queue<KeyValuePair<string, Vector3>> commands = new Queue<KeyValuePair<string, Vector3>>();



	public void Initial()
	{
		StringBuilder str = new StringBuilder("(:init (HANDEMPTY) ");
		extractState(str);

		str.Append(")");
		initialState = str.ToString();
		Debug.Log(initialState);

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
			if (!isObjectOnTableLevel(block))
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
			Regex regex = new Regex("\\(on [A-Za-z0-9]* " + block.name + "\\)");
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
		//foreach (GameObject obj in gameObjects)
		//{
		//	Destroy(obj);
		//}
		//loadSceneFromPDDL(initialState);
	}

	public void Solve()
	{
		//string problem = createPDDLProblem();
		//System.IO.File.WriteAllText(WORK_PATH + @"PDDLSolver\problem.pddl", problem);
		//CollisionDetection cd = GameObject.FindObjectOfType<CollisionDetection>();
		//cd.AutomatedMode(true);

		//foreach (GameObject block in gameObjects)
		//{
		//	block.GetComponent<Rigidbody>().useGravity = false;
		//}

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
				return;
			}
			System.Threading.Thread.Sleep(1000);
			newModification = System.IO.File.GetLastWriteTime(WORK_PATH + @"PDDLSolver\solution.txt");
			i++;
		}
		Debug.Log("solved in: " + i + " seconds");
		process.Close();
		System.Threading.Thread.Sleep(2000);
		string[] lines = System.IO.File.ReadAllLines(WORK_PATH + @"PDDLSolver\solution.txt");
		//List<string> cleanLines = cleanUpLines(lines);
		//GameObject cube = GameObject.Find("RedCube1");
		//Debug.Log("redcube1 init x:" + cube.transform.position.x + " y:" + cube.transform.position.y + " z:" + cube.transform.position.z);
		solutionLines = new Queue<string>();
		foreach (string line in lines)
		{
			solutionLines.Enqueue(line);
			Debug.Log(line);
			//divideActions(line);
			//MovementRecorder mr = GameObject.FindObjectOfType<MovementRecorder>();
			//mr.SetRecordedMovements(plannedMovements);
			//mr.Replay();
		}
		shouldSolve = true;
		//cube = GameObject.Find("RedCube1");
		//Debug.Log("redcube1 end x:" + cube.transform.position.x + " y:" + cube.transform.position.y + " z:" + cube.transform.position.z);
	}

	private List<string> cleanUpLines(string[] lines)
	{
		List<string> cleanLines = new List<string>();
		Regex solutionLineRegex = new Regex(@"\d\d:");
		foreach (string line in lines) {
			if (solutionLineRegex.IsMatch(line))
			{
				string cleanLine = Regex.Replace(line, @"\d\d: \( *", "(");
				cleanLine = Regex.Replace(cleanLine, @" \[\d\]", "");
				cleanLines.Add(cleanLine);
			}
		}

		return cleanLines;
	}
	private void divideActions(String action)
	{
		if (action.StartsWith("(load"))
		{
			action = action.Replace("(load ", "");
			action = action.Replace(")", "");
			String block = action.Trim();
			dobotLoaderPickupBlock(block);
			dobotLoaderPlaceBlock(block, "posload");
		}
		else if (action.StartsWith("(moveonconveyor"))
		{
			action = action.Replace("(moveonconveyor ", "");
			action = action.Replace(")", "");
			String block = action.Trim();
			moveOnConveyor(block);
		}
		else if (action.StartsWith("(place"))
		{
			action = action.Replace("(place ", "");
			action = action.Replace(")", "");
			string blockName = action.Split(new string[] { " " }, StringSplitOptions.None)[0].Trim();
			string posName = action.Split(new string[] { " " }, StringSplitOptions.None)[1].Trim();
			dobotRailPlaceBlock(blockName, posName);

		}
		else if (action.StartsWith("(pick-up"))
		{
			action = action.Replace("(pick-up ", "");
			action = action.Replace(")", "");
			String block = action.Trim();
			dobotRailPickupBlock(block);
		}
		else if (action.StartsWith("(put-down"))
		{
			action = action.Replace("(put-down ", "");
			action = action.Replace(")", "");
			String block = action.Trim();
			dobotRailPutdownBlock(block);
		}
		else if (action.StartsWith("(stack"))
		{
			action = action.Replace("(stack ", "");
			action = action.Replace(")", "");
			string blockInHand = action.Split(new string[] { " " }, StringSplitOptions.None)[0].Trim();
			string targetBlock = action.Split(new string[] { " " }, StringSplitOptions.None)[1].Trim();
			dobotRailStackBlock(blockInHand, targetBlock);
		}
		else if (action.StartsWith("(unstack"))
		{
			action = action.Replace("(unstack ", "");
			action = action.Replace(")", "");
			string targetBlock = action.Split(new string[] { " " }, StringSplitOptions.None)[0].Trim();
			dobotRailPickupBlock(targetBlock);
		}
	}

	private void moveOnConveyor(string blockName)
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

		float targetX = 1.5f;
		float startX = targetBlock.transform.position.x;

		while (startX<targetX) {
			startX+=0.001f;
			RobotArmState dobotLoaderState = new RobotArmState(
					dobotLoaderStartAngles[0],
					dobotLoaderStartAngles[1],
					dobotLoaderStartAngles[2],
					false,
					dobotLoaderEndPos);
			RobotArmState dobotRailState = new RobotArmState(dobotRailStartAngles[0], dobotRailStartAngles[1], dobotRailStartAngles[2], false, dobotRailEndPos);
			RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, true);
			plannedMovements.Enqueue(systemState);
		}
	}

	private void dobotRailPutdownBlock(string blockName)
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
				dobotRailMoveToPos(target, true);
				dobotRailDrop();
				dobotRailJump(false);
				putDownPositions[pos] = false;
				break;
			}
		}
	}

	private void dobotRailStackBlock(string blockInHandName, string targetBlockName)
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
		dobotRailMoveToPos(target, true);
		dobotRailDrop();
		dobotRailJump(false);

	}

	private void dobotLoaderJump(Boolean suction)
	{
		float diff = 1.0f - dobotLoaderEndPos.y;
		dobotLoaderEndPos += new Vector3(0, diff, 0);
		dobotLoaderMoveToPos(dobotLoaderEndPos, suction);
	}

	private void dobotRailJump(Boolean suction)
	{
		float diff = 1.0f - dobotRailEndPos.y;
		dobotRailEndPos += new Vector3(0, diff, 0);
		dobotRailMoveToPos(dobotRailEndPos, suction);
	}

	private void dobotLoaderDrop()
	{
		for (int i = 0; i < 10; i++)
		{
			RobotArmState dobotLoaderState = new RobotArmState(
					dobotLoaderStartAngles[0],
					dobotLoaderStartAngles[1],
					dobotLoaderStartAngles[2],
					false,
					dobotLoaderEndPos);
			RobotArmState dobotRailState = new RobotArmState(dobotRailStartAngles[0], dobotRailStartAngles[1], dobotRailStartAngles[2], false, dobotRailEndPos);
			RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, true);
			plannedMovements.Enqueue(systemState);
		}
	}

	private void dobotRailDrop()
	{
		for (int i = 0; i < 10; i++)
		{
			RobotArmState dobotLoaderState = new RobotArmState(
					dobotLoaderStartAngles[0],
					dobotLoaderStartAngles[1],
					dobotLoaderStartAngles[2],
					false,
					dobotLoaderEndPos);
			RobotArmState dobotRailState = new RobotArmState(dobotRailStartAngles[0], dobotRailStartAngles[1], dobotRailStartAngles[2], false, dobotRailEndPos);
			RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, false);
			plannedMovements.Enqueue(systemState);
		}
	}

	private void dobotLoaderPickupBlock(String blockName)
	{
		foreach (GameObject block in gameObjects)
		{
			if (blockName.Equals(block.name, StringComparison.InvariantCultureIgnoreCase))
			{
				//float blockTop = block.GetComponent<Renderer>().bounds.max.y - block.GetComponent<Renderer>().bounds.min.y;
				Vector3 target = new Vector3(block.transform.position.x - 0.003f, block.transform.position.y + 0.013f, block.transform.position.z);
				commands.Enqueue(new KeyValuePair<string, Vector3>("pick-up", target));
				dobotLoaderJump(true);
				dobotLoaderMoveToPos(target + new Vector3(0, 0.06f, 0), true);
				dobotLoaderMoveToPos(target, true);
				dobotLoaderJump(true);
			}
		}
	}

	private void dobotRailPickupBlock(String blockName)
	{
		foreach (GameObject block in gameObjects)
		{
			if (blockName.Equals(block.name, StringComparison.InvariantCultureIgnoreCase))
			{
				//float blockTop = block.GetComponent<Renderer>().bounds.max.y - block.GetComponent<Renderer>().bounds.min.y;
				Vector3 target = new Vector3(block.transform.position.x - 0.003f, block.transform.position.y + 0.013f, block.transform.position.z);
				commands.Enqueue(new KeyValuePair<string, Vector3>("pick-up", target));
				dobotRailJump(true);
				dobotRailMoveToPos(target + new Vector3(0, 0.06f, 0), true);
				dobotRailMoveToPos(target, true);
				dobotRailJump(true);
			}
		}
	}

	private void dobotLoaderPlaceBlock(String blockName, String posName)
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
				dobotLoaderMoveToPos(target + new Vector3(0, 0.03f, 0), true);
				dobotLoaderMoveToPos(target, true);
				dobotLoaderDrop();
				dobotLoaderJump(false);
			}
		}
	}

	private void dobotRailPlaceBlock(String blockName, String posName)
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
				dobotRailMoveToPos(target + new Vector3(0, 0.03f, 0), true);
				dobotRailMoveToPos(target, true);
				dobotRailDrop();
				dobotRailJump(false);
			}
		}
	}

	private void dobotLoaderMoveToPos(Vector3 target, Boolean suction)
	{
		Debug.Log("movetotarget: " + UnityUtil.PositionToString(target));
		float[] angleTarget = dobotLoaderIK.GetAnglesForPosition(target);
		angleTarget = dobotLoaderIK.GetAnglesForPositionCorrection(target, angleTarget);
		angleTarget = dobotLoaderIK.GetAnglesForPositionCorrection(target, angleTarget);
		float baseStart = dobotLoaderStartAngles[0];
		float l2Start = dobotLoaderStartAngles[1];
		float l3Start = dobotLoaderStartAngles[2];
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

			RobotArmState dobotLoaderState = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
			RobotArmState dobotRailState = new RobotArmState(dobotRailStartAngles[0], dobotRailStartAngles[1], dobotRailStartAngles[2], false, dobotRailEndPos);
			RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, true);
			plannedMovements.Enqueue(systemState);

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
				RobotArmState dobotLoaderState = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				RobotArmState dobotRailState = new RobotArmState(dobotRailStartAngles[0], dobotRailStartAngles[1], dobotRailStartAngles[2], false, dobotRailEndPos);
				RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, true);
				plannedMovements.Enqueue(systemState);
			}
			else
			{
				baseAngle += 0.5f;
				RobotArmState dobotLoaderState = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				RobotArmState dobotRailState = new RobotArmState(dobotRailStartAngles[0], dobotRailStartAngles[1], dobotRailStartAngles[2], false, dobotRailEndPos);
				RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, true);
				plannedMovements.Enqueue(systemState);
			}
			baseRotatorDifference = angleTarget[0] - baseAngle;
		}

		while (Math.Abs(lowerArmDifference) > 0.25)
		{
			if (lowerArmDifference < 0)
			{
				l2Start -= 0.5f;
				RobotArmState dobotLoaderState = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				RobotArmState dobotRailState = new RobotArmState(dobotRailStartAngles[0], dobotRailStartAngles[1], dobotRailStartAngles[2], false, dobotRailEndPos);
				RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, true);
				plannedMovements.Enqueue(systemState);
			}
			else
			{
				l2Start += 0.5f;
				RobotArmState dobotLoaderState = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				RobotArmState dobotRailState = new RobotArmState(dobotRailStartAngles[0], dobotRailStartAngles[1], dobotRailStartAngles[2], false, dobotRailEndPos);
				RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, true);
				plannedMovements.Enqueue(systemState);
			}
			lowerArmDifference = angleTarget[1] - l2Start;
		}

		while (Math.Abs(upperArmDifference) > 0.25)
		{
			if (upperArmDifference < 0)
			{
				l3Start -= 0.5f;
				RobotArmState dobotLoaderState = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				RobotArmState dobotRailState = new RobotArmState(dobotRailStartAngles[0], dobotRailStartAngles[1], dobotRailStartAngles[2], false, dobotRailEndPos);
				RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, true);
				plannedMovements.Enqueue(systemState);
			}
			else
			{
				l3Start += 0.5f;
				RobotArmState dobotLoaderState = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				RobotArmState dobotRailState = new RobotArmState(dobotRailStartAngles[0], dobotRailStartAngles[1], dobotRailStartAngles[2], false, dobotRailEndPos);
				RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, true);
				plannedMovements.Enqueue(systemState);
			}
			upperArmDifference = angleTarget[2] - l3Start;
		}


		dobotLoaderStartAngles = angleTarget;
		dobotLoaderEndPos = target;
	}


	private void dobotRailMoveToPos(Vector3 target, Boolean suction)
	{
		Debug.Log("movetotarget: " + UnityUtil.PositionToString(target));
		float[] angleTarget = dobotRailIK.GetAnglesForPosition(target);
		angleTarget = dobotRailIK.GetAnglesForPositionCorrection(target, angleTarget);
		angleTarget = dobotRailIK.GetAnglesForPositionCorrection(target, angleTarget);
		float baseStart = dobotRailStartAngles[0];
		float l2Start = dobotRailStartAngles[1];
		float l3Start = dobotRailStartAngles[2];
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

			RobotArmState dobotRailState = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
			RobotArmState dobotLoaderState = new RobotArmState(dobotLoaderStartAngles[0], dobotLoaderStartAngles[1], dobotLoaderStartAngles[2], false, dobotLoaderEndPos);
			RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, false);
			plannedMovements.Enqueue(systemState);
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
				RobotArmState dobotRailState = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				RobotArmState dobotLoaderState = new RobotArmState(dobotLoaderStartAngles[0], dobotLoaderStartAngles[1], dobotLoaderStartAngles[2], false, dobotLoaderEndPos);
				RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, false);
				plannedMovements.Enqueue(systemState);
			}
			else
			{
				baseAngle += 0.5f;
				RobotArmState dobotRailState = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				RobotArmState dobotLoaderState = new RobotArmState(dobotLoaderStartAngles[0], dobotLoaderStartAngles[1], dobotLoaderStartAngles[2], false, dobotLoaderEndPos);
				RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, false);
				plannedMovements.Enqueue(systemState);
			}
			baseRotatorDifference = angleTarget[0] - baseAngle;
		}

		while (Math.Abs(lowerArmDifference) > 0.25)
		{
			if (lowerArmDifference < 0)
			{
				l2Start -= 0.5f;
				RobotArmState dobotRailState = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				RobotArmState dobotLoaderState = new RobotArmState(dobotLoaderStartAngles[0], dobotLoaderStartAngles[1], dobotLoaderStartAngles[2], false, dobotLoaderEndPos);
				RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, false);
				plannedMovements.Enqueue(systemState);
			}
			else
			{
				l2Start += 0.5f;
				RobotArmState dobotRailState = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				RobotArmState dobotLoaderState = new RobotArmState(dobotLoaderStartAngles[0], dobotLoaderStartAngles[1], dobotLoaderStartAngles[2], false, dobotLoaderEndPos);
				RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, false);
				plannedMovements.Enqueue(systemState);
			}
			lowerArmDifference = angleTarget[1] - l2Start;
		}

		while (Math.Abs(upperArmDifference) > 0.25)
		{
			if (upperArmDifference < 0)
			{
				l3Start -= 0.5f;
				RobotArmState dobotRailState = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				RobotArmState dobotLoaderState = new RobotArmState(dobotLoaderStartAngles[0], dobotLoaderStartAngles[1], dobotLoaderStartAngles[2], false, dobotLoaderEndPos);
				RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, false);
				plannedMovements.Enqueue(systemState);
			}
			else
			{
				l3Start += 0.5f;
				RobotArmState dobotRailState = new RobotArmState(
					baseAngle,
					l2Start,
					l3Start,
					suction,
					target);
				RobotArmState dobotLoaderState = new RobotArmState(dobotLoaderStartAngles[0], dobotLoaderStartAngles[1], dobotLoaderStartAngles[2], false, dobotLoaderEndPos);
				RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, false);
				plannedMovements.Enqueue(systemState);
			}
			upperArmDifference = angleTarget[2] - l3Start;
		}


		dobotRailStartAngles = angleTarget;
		dobotRailEndPos = target;
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
		problem.Append("(:domain CUBES)\n");
		//problem.Append("(:objects cube1 cube2 - cube posA posB posC - position)\n");
		problem.Append("(:objects ");
		foreach (GameObject movable in gameObjects)
		{
			problem.Append(movable.gameObject.name.Replace("(Clone)", "")).Append(" ");
		}
		problem.Append("- cube\n");
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

	private string findObjectAtPosition(GameObject pddlObj, GameObject position)
	{
		Vector3 objMin = pddlObj.GetComponent<Renderer>().bounds.min;
		Vector3 objMax = pddlObj.GetComponent<Renderer>().bounds.max;
		Vector3 objCenter = pddlObj.GetComponent<Renderer>().bounds.center;
		Vector3 minA = position.GetComponent<Renderer>().bounds.min;
		Vector3 maxA = position.GetComponent<Renderer>().bounds.max;
		Vector3 centerA = position.GetComponent<Renderer>().bounds.center;

		StringBuilder str = new StringBuilder();
		if (isObjectOnTableLevel(pddlObj)) // obj is immediately above position, position is always on table
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
		Debug.Log("registered");
	}

	public void Unregister(GameObject gameObject)
	{
		gameObjects.Remove(gameObject);
	}

	// Start is called before the first frame update
	void Start()
	{

		//Debug.Log("A:" + UnityUtil.PositionToString(UnityUtil.DobotArmToVR(new Vector3(260,100,-15))));
		//Debug.Log("B:" + UnityUtil.PositionToString(UnityUtil.DobotArmToVR(new Vector3(155, 100, -15))));
		//Debug.Log("C:" + UnityUtil.PositionToString(UnityUtil.DobotArmToVR(new Vector3(155, 100, -70))));

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
		//dobotLoaderBaseRotator = GameObject.Find("magician_link_1");
		//dobotLoaderLowerArm = GameObject.Find("magician_link_2");
		//dobotLoaderUpperArm = GameObject.Find("magician_link_3");
		//dobotLoaderHand = GameObject.Find("magician_link_4");
		//dobotLoaderEndEffector = GameObject.Find("magician_end_effector");
		dobotLoaderEndPos = dobotLoaderEndEffector.transform.position;
		dobotRailEndPos = dobotRailEndEffector.transform.position;
		GameObject dobotLoader = GameObject.Find("DobotLoader");
		dobotLoaderIK = dobotLoader.GetComponent<InverseKinematics>();
		GameObject dobotRail = GameObject.Find("DobotRail");
		dobotRailIK = dobotRail.GetComponent<InverseKinematics>();
		//gameObjects.AddRange(GameObject.FindGameObjectsWithTag("PDDLObject"));
		Debug.Log(UnityUtil.PositionToString(dobotLoaderEndPos));
		//Debug.Log(UnityUtil.PositionToString(UnityUtil.DobotArmToVR(new Vector3(0, 147, 135))));
		//Debug.Log(UnityUtil.PositionToString(UnityUtil.DobotArmToVR(new Vector3(147, 0, 135))));
		Debug.Log(UnityUtil.PositionToString(UnityUtil.VRToDobotArm(dobotLoaderEndPos)));
		GameObject table = GameObject.Find("Table");
		tableHeight = table.GetComponent<Renderer>().bounds.max.y;
		movementRecorder = GameObject.FindObjectOfType<MovementRecorder>();

		ServiceCaller sc = ServiceCaller.getInstance();
		sc.SetPTPCmd(1, 147, 0, 135, 0, false);
	}

	float timer = 0.0f;
	bool once = true;
	bool calcTime = true;
	int seconds = 0;

	// Update is called once per frame
	void Update()
	{
		if (calcTime)
		{
			timer += Time.deltaTime;
			seconds = (int)timer % 60;
		}
		if (once && seconds >= 4)
		{
			LoadGraspDetectionSubscriber graspDetection = GameObject.FindObjectOfType<LoadGraspDetectionSubscriber>();
			graspDetection.updateObjectVisualization();
			once = false;
			calcTime = false;
		}

		if (shouldSolve)
		{
			if (solutionLines.Count != 0 && movementRecorder.isReplayDone())
			{
				string line = solutionLines.Dequeue();
				divideActions(line);
				//Queue<RobotArmState> copiedMovements = new Queue<RobotArmState>(plannedMovements);
				movementRecorder.SetRecordedMovements(plannedMovements);
				movementRecorder.Replay();
				//Actuator actuator = new Actuator();
				//actuator.executeCommands(commands);
			}
			else if (solutionLines.Count == 0 && movementRecorder.isReplayDone())
			{
				shouldSolve = false;
				//CollisionDetection cd = GameObject.FindObjectOfType<CollisionDetection>();
				//cd.AutomatedMode(false);

				//foreach (GameObject block in gameObjects)
				//{
				//	block.GetComponent<Rigidbody>().useGravity = true;
				//}
			}

		}
	}
}
