using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DobotController : MonoBehaviour
{
	public static int CubeCounter = 0;
	public GameObject Magician;
	public GameObject BaseRotator; // magician_link_1
	public GameObject LowerArm; // magician_link_2
	public GameObject UpperArm; // magician_link_3
	public GameObject Hand; // magician_link_4
	public GameObject EndEffector; // magician_end_effector
	public GameObject SuctionCup; //magicianSuctionCup
	public GameObject Cube;
	//private bool plus = true;
	//private bool isHolding = false;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.DownArrow))
		{
			LowerArm.transform.Rotate(1, 0, 0);
			Hand.transform.Rotate(-1, 0, 0);
			//Debug.Log("rot:" + LowerArm.transform.rotation.x + " pos:" + EndEffector.transform.position.y.ToString());
		}
		else if (Input.GetKey(KeyCode.UpArrow))
		{
			LowerArm.transform.Rotate(-1, 0, 0);
			Hand.transform.Rotate(1, 0, 0);
			//Debug.Log(EndEffector.transform.position);
		}
		else if (Input.GetKey(KeyCode.LeftArrow))
		{
			BaseRotator.transform.Rotate(0, -1, 0);
			//Debug.Log(EndEffector.transform.position);
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			BaseRotator.transform.Rotate(0, 1, 0);
			//Debug.Log(EndEffector.transform.position);
		}
		else if (Input.GetKey(KeyCode.PageDown))
		{
			UpperArm.transform.Rotate(1, 0, 0);
			Hand.transform.Rotate(-1, 0, 0);
		}
		else if (Input.GetKey(KeyCode.PageUp))
		{
			UpperArm.transform.Rotate(-1, 0, 0);
			Hand.transform.Rotate(1, 0, 0);
		}

		if (Input.GetKeyDown(KeyCode.A))
		{
			CubeCounter++;
			Cube.name = "Cube" + CubeCounter;
			Instantiate(Cube, WorldGrid.grid[8, 7, 0] + new Vector3(0.05f, 0.01f, 0.05f), Quaternion.identity);
		}

		if (Input.GetKeyDown(KeyCode.B))
		{
			CubeCounter++;
			Cube.name = "Cube" + CubeCounter;
			Instantiate(Cube, WorldGrid.grid[9, 7, 0] + new Vector3(0.05f, 0.01f, 0.05f), Quaternion.identity);
		}

		if (Input.GetKeyDown(KeyCode.C))
		{
			CubeCounter++;
			Cube.name = "Cube" + CubeCounter;
			Instantiate(Cube, WorldGrid.grid[10, 7, 0] + new Vector3(0.05f, 0.01f, 0.05f), Quaternion.identity);
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			//WorldStateManager wordStateManager = GameObject.Find("WorldStateManager").GetComponent<WorldStateManager>();
			//wordStateManager.RecordInitialState();
			WorldState ws = GameObject.FindObjectOfType<WorldState>();
			ws.Initial();
		}

		if (Input.GetKeyDown(KeyCode.G))
		{
			//WorldStateManager wordStateManager = GameObject.Find("WorldStateManager").GetComponent<WorldStateManager>();
			//wordStateManager.RecordGoalState();
			WorldState ws = GameObject.FindObjectOfType<WorldState>();
			ws.Goal();
		}

		if (Input.GetKeyDown(KeyCode.End)) {
			WorldState ws = GameObject.FindObjectOfType<WorldState>();
			ws.Solve();
		}

		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			Debug.Log("base:" + BaseRotator.transform.rotation.eulerAngles.y);
			Debug.Log("lower:" + LowerArm.transform.rotation.eulerAngles.x);
			Debug.Log("upper:" + UpperArm.transform.rotation.eulerAngles.x);
		}

		/*
		BaseRotator.transform.Rotate(0, 1, 0);
		if (plus) {
			LowerArm.transform.Rotate(1, 0, 0);
			UpperArm.transform.Rotate(-1, 0, 0);
			Hand.transform.Rotate(0, 0, 0);
		} else {
			LowerArm.transform.Rotate(-1, 0, 0);
			UpperArm.transform.Rotate(1, 0, 0);
			Hand.transform.Rotate(0, 0, 0);
		}

		if (LowerArm.transform.rotation.eulerAngles.x < 1) { plus = true; }
		if (LowerArm.transform.rotation.eulerAngles.x > 30) { plus = false; }
	*/
	}
}
