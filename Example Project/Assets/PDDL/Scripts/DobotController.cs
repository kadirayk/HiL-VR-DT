using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DobotController : MonoBehaviour
{
	public static int CubeCounter = 0;
	public GameObject Magician;
	public GameObject BaseRotator; // magician_link_1
	public GameObject LowerArm; // magician_link_2
	public GameObject UpperArm; // magician_link_3
	public GameObject DobotHand; // magician_link_4
	public GameObject EndEffector; // magician_end_effector
	public GameObject SuctionCup; //magicianSuctionCup
	public GameObject Cube;
	public bool suctionActive;
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
			DobotHand.transform.Rotate(-1, 0, 0);
			//Debug.Log("rot:" + LowerArm.transform.rotation.x + " pos:" + EndEffector.transform.position.y.ToString());
		}
		else if (Input.GetKey(KeyCode.UpArrow))
		{
			LowerArm.transform.Rotate(-1, 0, 0);
			DobotHand.transform.Rotate(1, 0, 0);
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
			DobotHand.transform.Rotate(-1, 0, 0);
		}
		else if (Input.GetKey(KeyCode.PageUp))
		{
			UpperArm.transform.Rotate(-1, 0, 0);
			DobotHand.transform.Rotate(1, 0, 0);
		}

		if (Input.GetKeyDown(KeyCode.A))
		{
			CubeCounter++;
			Cube.name = "Cube" + CubeCounter;
			Instantiate(Cube, WorldGrid.grid[8, 12, 8] + new Vector3(0.05f, 0.01f, 0.05f), Quaternion.identity);
			Debug.Log("Cube at A");
		}

		if (Input.GetKeyDown(KeyCode.B))
		{
			CubeCounter++;
			Cube.name = "Cube" + CubeCounter;
			Instantiate(Cube, WorldGrid.grid[9, 12, 8] + new Vector3(0.05f, 0.01f, 0.05f), Quaternion.identity);
		}

		if (Input.GetKeyDown(KeyCode.C))
		{
			CubeCounter++;
			Cube.name = "Cube" + CubeCounter;
			Instantiate(Cube, WorldGrid.grid[10, 12, 8] + new Vector3(0.05f, 0.01f, 0.05f), Quaternion.identity);
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

		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			InverseKinematics ik = GameObject.FindObjectOfType<InverseKinematics>();
			ik.moveTarget("left");
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			InverseKinematics ik = GameObject.FindObjectOfType<InverseKinematics>();
			ik.moveTarget("right");
		}
		if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			InverseKinematics ik = GameObject.FindObjectOfType<InverseKinematics>();
			ik.moveTarget("up");
		}
		if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			InverseKinematics ik = GameObject.FindObjectOfType<InverseKinematics>();
			ik.moveTarget("down");
		}
		if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			InverseKinematics ik = GameObject.FindObjectOfType<InverseKinematics>();
			ik.moveTarget("fw");
		}
		if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			InverseKinematics ik = GameObject.FindObjectOfType<InverseKinematics>();
			ik.moveTarget("bw");
		}

		if (Input.GetKeyDown(KeyCode.R)) {
			MovementRecorder mr = GameObject.FindObjectOfType<MovementRecorder>();
			mr.StartRecording();
			GameObject textObj = GameObject.Find("CurrentState_Text");
			Text text = textObj.GetComponent<Text>();
			text.text = "Recording Movements";
		}

		if (Input.GetKeyDown(KeyCode.P))
		{
			MovementRecorder mr = GameObject.FindObjectOfType<MovementRecorder>();
			mr.StopRecording();
			GameObject textObj = GameObject.Find("CurrentState_Text");
			Text text = textObj.GetComponent<Text>();
			text.text = "Stopped Recording";
		}

		if (Input.GetKeyDown(KeyCode.O))
		{
			MovementRecorder mr = GameObject.FindObjectOfType<MovementRecorder>();
			mr.Replay();
			GameObject textObj = GameObject.Find("CurrentState_Text");
			Text text = textObj.GetComponent<Text>();
			text.text = "Replaying";
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			CollisionDetection cd = GameObject.FindObjectOfType<CollisionDetection>();
			cd.Drop();
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

	public bool SuctionActive { get => suctionActive; set => suctionActive = value; }
}
