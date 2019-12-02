using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.PDDL;
using System.Text;
using System;

public class WorldState : MonoBehaviour, IListener, IProblemState
{
	IList<GameObject> gameObjects = new List<GameObject>();
	IList<GameObject> stationaryObjects = new List<GameObject>();
	private string initialState;
	private string goalState;
	public GameObject Cube;
	float[] actionTarget = new float[3] { 180, 0, 0 };
	bool performAnimation = false;
	GameObject baseRotator;
	GameObject lowerArm;
	GameObject upperArm;
	GameObject hand;
	Queue<String> actionQueue = new Queue<string>();
	bool animationDone = false;


	public void Initial()
	{
		StringBuilder str = new StringBuilder("(:init ");
		foreach (GameObject movable in gameObjects)
		{
			foreach (GameObject stationary in stationaryObjects)
			{
				str.Append(findObjectLocations(movable, stationary));
				//Vector3 relative = stationary.transform.InverseTransformPoint(movable.transform.position);
				//Debug.Log(stationary.name + ":" + UnityUtil.PositionToString(relative));
			}
		}


		foreach (GameObject stationary in stationaryObjects)
		{
			string label = "pos" + stationary.GetComponentInChildren<TextMesh>().text;
			if (!str.ToString().Contains(label))
			{
				str.Append("(free ").Append(label).Append(")");
			}
		}

		str.Append(")");
		initialState = str.ToString();
		Debug.Log(initialState);

	}

	public void Goal()
	{
		StringBuilder str = new StringBuilder("(:goal (and ");
		foreach (GameObject movable in gameObjects)
		{
			foreach (GameObject stationary in stationaryObjects)
			{
				str.Append(findObjectLocations(movable, stationary));
				//Vector3 relative = stationary.transform.InverseTransformPoint(movable.transform.position);
				//Debug.Log(stationary.name + ":" + UnityUtil.PositionToString(relative));
			}
		}
		str.Append("))");
		goalState = str.ToString();
		Debug.Log(goalState);
		foreach (GameObject obj in gameObjects)
		{
			Destroy(obj);
		}
		loadSceneFromPDDL(initialState);
	}

	public void Solve()
	{
		string problem = createPDDLProblem();
		System.IO.File.WriteAllText(@"C:\PDDLSolver\problem.pddl", problem);

		System.Diagnostics.Process process = new System.Diagnostics.Process();
		var startInfo = new System.Diagnostics.ProcessStartInfo
		{
			WorkingDirectory = @"C:\PDDLSolver",
			WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
			FileName = "cmd.exe",
			Arguments = "/C java -jar PDDLSolver.jar domain.pddl problem.pddl"
		};
		process.StartInfo = startInfo;
		process.Start();
		System.Threading.Thread.Sleep(2000);
		process.Close();
		string[] lines = System.IO.File.ReadAllLines(@"C:\PDDLSolver\solution.txt");
		foreach (string line in lines)
		{
			Debug.Log(line);
			divideActions(line);
		}
	}

	private void divideActions(String action)
	{
		action = action.Replace("move(", "");
		action = action.Replace(")", "");
		string from = action.Split(new string[] { "," }, StringSplitOptions.None)[1].Trim();
		string to = action.Split(new string[] { "," }, StringSplitOptions.None)[2].Trim();
		//actionQueue.Enqueue("reset");
		actionQueue.Enqueue(from);
		//actionQueue.Enqueue("reset");
		actionQueue.Enqueue(to);
	}

	public void findActionTarget(string pos)
	{
		if (pos.Equals("posA", System.StringComparison.InvariantCultureIgnoreCase))
		{
			actionTarget[0] = 210;
			actionTarget[1] = 59;
			actionTarget[2] = 38;
			//210 59 38
		}
		else if (pos.Equals("posC", System.StringComparison.InvariantCultureIgnoreCase))
		{
			actionTarget[0] = 172;
			actionTarget[1] = 48;
			actionTarget[2] = 38;
			//172 48 38
		}
		else if (pos.Equals("reset")) 
		{
			actionTarget[0] = 182;
			actionTarget[1] = 4;
			actionTarget[2] = 4;
		}
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

		string cubeName = atomic.Split(null)[1];
		string posName = atomic.Split(null)[2];
		foreach (GameObject stationary in stationaryObjects)
		{
			string label = "pos" + stationary.GetComponentInChildren<TextMesh>().text;
			if (label.Equals(posName, System.StringComparison.InvariantCultureIgnoreCase))
			{
				Vector3 pos = stationary.transform.position + new Vector3(0f, 0.04f, 0f);
				Cube.name = cubeName;
				GameObject obj = Instantiate(Cube, pos, Quaternion.identity);
				int index = Int32.Parse(cubeName.Replace("Cube", "")) - 1;
				gameObjects[index] = obj;
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
		foreach (GameObject stationary in stationaryObjects)
		{
			string label = "pos" + stationary.GetComponentInChildren<TextMesh>().text;
			problem.Append(label).Append(" ");
		}
		problem.Append("- position)\n");
		problem.Append(initialState).Append("\n");
		problem.Append(goalState).Append("\n");
		problem.Append(")");
		return problem.ToString();
	}

	private string findObjectLocations(GameObject pddlObj, GameObject stationaryObject)
	{
		Vector3 objMin = pddlObj.GetComponent<Renderer>().bounds.min;
		Vector3 objMax = pddlObj.GetComponent<Renderer>().bounds.max;
		Vector3 minA = stationaryObject.GetComponent<Renderer>().bounds.min;
		Vector3 maxA = stationaryObject.GetComponent<Renderer>().bounds.max;
		StringBuilder str = new StringBuilder();
		if (objMin.x > minA.x && objMin.z > minA.z && objMax.x < maxA.x && objMax.z < maxA.z)
		{
			string objName = pddlObj.gameObject.name;
			if (objName.Contains("(Clone)"))
			{
				objName = objName.Replace("(Clone)", "");
			}
			str.Append("(at ").Append(objName).Append(" pos").Append(stationaryObject.GetComponentInChildren<TextMesh>().text).Append(")");
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
		//stationaryObjects = GameObject.FindGameObjectsWithTag("Stationary");
		//baseRotator = GameObject.Find("magician_link_1");
		//lowerArm = GameObject.Find("magician_link_2");
		//upperArm = GameObject.Find("magician_link_3");
		//hand = GameObject.Find("magician_link_4");
	}

	// Update is called once per frame
	void Update()
	{
		if (actionQueue.Count != 0)
		{

			string pos = actionQueue.Peek();
			findActionTarget(pos);


			float baseRotatorDifference = baseRotator.transform.rotation.eulerAngles.y - actionTarget[0];
			float lowerArmDifference = lowerArm.transform.rotation.eulerAngles.x - actionTarget[1];
			float upperArmDifference = upperArm.transform.rotation.eulerAngles.x - actionTarget[2];
			bool baseDone = false;
			bool upperDone = false;
			bool lowerDone = false;

			if (Math.Abs(baseRotatorDifference) > 1)
			{
				if (baseRotatorDifference < 0)
				{
					baseRotator.transform.Rotate(0, 1, 0);
				}
				else
				{
					baseRotator.transform.Rotate(0, -1, 0);
				}
				System.Threading.Thread.Sleep(20);
			}
			else
			{
				baseDone = true;
			}

			if (baseDone)
			{
				if (Math.Abs(lowerArmDifference) > 1)
				{
					if (lowerArmDifference < 0)
					{
						lowerArm.transform.Rotate(1, 0, 0);
						hand.transform.Rotate(-1, 0, 0);
					}
					else
					{
						lowerArm.transform.Rotate(-1, 0, 0);
						hand.transform.Rotate(1, 0, 0);
					}
				}
				else
				{
					lowerDone = true;
				}

				if (lowerDone)
				{
					if (Math.Abs(upperArmDifference) > 1)
					{
						if (upperArmDifference < 0)
						{
							upperArm.transform.Rotate(1, 0, 0);
							hand.transform.Rotate(-1, 0, 0);
						}
						else
						{
							upperArm.transform.Rotate(-1, 0, 0);
							hand.transform.Rotate(1, 0, 0);
						}
					}
					else
					{
						upperDone = true;
					}
				}
			}
			animationDone = baseDone && lowerDone && upperDone;
			if (animationDone)
			{
				actionQueue.Dequeue();
				animationDone = false;
				System.Threading.Thread.Sleep(500);
			}
		}
	}
}
