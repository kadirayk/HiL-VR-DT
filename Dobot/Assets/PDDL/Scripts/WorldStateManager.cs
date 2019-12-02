using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
	public GameObject PositionA;
	public GameObject PositionB;
	public GameObject PositionC;
	private Vector3 minA;
	private Vector3 maxA;
	private Vector3 minB;
	private Vector3 maxB;
	private Vector3 minC;
	private Vector3 maxC;
	private List<GameObject> PDDLObjectsAtPositionA;
	private List<GameObject> PDDLObjectsAtPositionB;
	private List<GameObject> PDDLObjectsAtPositionC;
	private List<GameObject> handledObjects;

	private State initialState;
	private State goalState;

	// Start is called before the first frame update
	void Start()
	{
		//	handledObjects = new List<GameObject>();

		maxA = PositionA.GetComponent<Renderer>().bounds.max;
		minA = PositionA.GetComponent<Renderer>().bounds.min;
		maxB = PositionB.GetComponent<Renderer>().bounds.max;
		minB = PositionB.GetComponent<Renderer>().bounds.min;
		maxC = PositionC.GetComponent<Renderer>().bounds.max;
		minC = PositionC.GetComponent<Renderer>().bounds.min;
	}

	void OnDrawGizmos()
	{

		Renderer rend = PositionA.GetComponent<Renderer>();
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(rend.bounds.max, 0.01f);
		Gizmos.DrawSphere(rend.bounds.min, 0.01f);
		Gizmos.DrawSphere(new Vector3(rend.bounds.max.x, 0, rend.bounds.min.z), 0.01f);
		Gizmos.DrawSphere(new Vector3(rend.bounds.min.x, 0, rend.bounds.max.z), 0.01f);

	}


	// Update is called once per frame
	void Update()
	{
		//GameObject[] PDDLObjects = GameObject.FindGameObjectsWithTag("PDDLObject");
		//PDDLObjectsAtPositionA = new List<GameObject>();
		//PDDLObjectsAtPositionB = new List<GameObject>();
		//PDDLObjectsAtPositionC = new List<GameObject>();
		//foreach (GameObject pddlObj in PDDLObjects)
		//{

		//	Vector3 objMin = pddlObj.GetComponent<Renderer>().bounds.min;
		//	Vector3 objMax = pddlObj.GetComponent<Renderer>().bounds.max;
		//	if (objMin.x > minA.x && objMin.z > minA.z && objMax.x < maxA.x && objMax.z < maxA.z)
		//	{
		//		Debug.Log("object a A");
		//		PDDLObjectsAtPositionA.Add(pddlObj);
		//	}
		//	if (objMin.x > minB.x && objMin.z > minB.z && objMax.x < maxB.x && objMax.z < maxB.z)
		//	{
		//		Debug.Log("object a B");
		//		PDDLObjectsAtPositionB.Add(pddlObj);
		//	}
		//	if (objMin.x > minC.x && objMin.z > minC.z && objMax.x < maxC.x && objMax.z < maxC.z)
		//	{
		//		Debug.Log("object a C");
		//		PDDLObjectsAtPositionC.Add(pddlObj);
		//	}

		//}
	}

	public void RecordInitialState()
	{
		initialState = new State();
		foreach (GameObject obj in PDDLObjectsAtPositionA)
		{
			initialState.PDDLObjectsAtPositionA.Add(obj);
		}
		foreach (GameObject obj in PDDLObjectsAtPositionB)
		{
			initialState.PDDLObjectsAtPositionB.Add(obj);
		}
		foreach (GameObject obj in PDDLObjectsAtPositionC)
		{
			initialState.PDDLObjectsAtPositionC.Add(obj);
		}
		string pddl = initialState.toInitPDDL();
		Debug.Log(pddl);
	}

	public void RecordGoalState()
	{
		goalState = new State();
		foreach (GameObject obj in PDDLObjectsAtPositionA)
		{
			goalState.PDDLObjectsAtPositionA.Add(obj);
		}
		foreach (GameObject obj in PDDLObjectsAtPositionB)
		{
			goalState.PDDLObjectsAtPositionB.Add(obj);
		}
		foreach (GameObject obj in PDDLObjectsAtPositionC)
		{
			goalState.PDDLObjectsAtPositionC.Add(obj);
		}
		string pddl = goalState.toGoalPDDL();
		Debug.Log(pddl);
	}

}
