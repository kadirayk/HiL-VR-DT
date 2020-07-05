using Assets.PDDL.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DobotController : MonoBehaviour
{
	public GameObject BaseRotator; // magician_link_1
	public GameObject LowerArm; // magician_link_2
	public GameObject UpperArm; // magician_link_3
	public GameObject DobotHand; // magician_link_4
	UIStatus uiStatus;

	Queue<RobotArmState> movements;

	// Start is called before the first frame update
	void Start()
	{
		uiStatus = GameObject.FindObjectOfType<UIStatus>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.DownArrow))
		{
			LowerArm.transform.Rotate(1, 0, 0);
			DobotHand.transform.Rotate(-1, 0, 0);
		}
		else if (Input.GetKey(KeyCode.UpArrow))
		{
			LowerArm.transform.Rotate(-1, 0, 0);
			DobotHand.transform.Rotate(1, 0, 0);
		}
		else if (Input.GetKey(KeyCode.LeftArrow))
		{
			BaseRotator.transform.Rotate(0, -1, 0);
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			BaseRotator.transform.Rotate(0, 1, 0);
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

		//if (Input.GetKeyDown(KeyCode.S))
		//{
		//	WorldState ws = GameObject.FindObjectOfType<WorldState>();
		//	ws.Initial();
		//}

		if (Input.GetKeyDown(KeyCode.G))
		{
			WorldState ws = GameObject.FindObjectOfType<WorldState>();
			ws.Goal();
		}

		if (Input.GetKeyDown(KeyCode.End))
		{
			WorldState ws = GameObject.FindObjectOfType<WorldState>();
			ws.Solve();
		}

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
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

		if (Input.GetKeyDown(KeyCode.R))
		{
			MovementRecorder mr = GameObject.FindObjectOfType<MovementRecorder>();
			mr.StartRecording();
			uiStatus.setStatus("Recording Movements");
			//GameObject textObj = GameObject.Find("CurrentState_Text");
			//Text text = textObj.GetComponent<Text>();
			//text.text = "Recording Movements";
		}

		if (Input.GetKeyDown(KeyCode.P))
		{
			MovementRecorder mr = GameObject.FindObjectOfType<MovementRecorder>();
			mr.StopRecording();
			movements = new Queue<RobotArmState>(mr.GetRecordedMovements());
			uiStatus.setStatus("Stopped Recording");
			//GameObject textObj = GameObject.Find("CurrentState_Text");
			//Text text = textObj.GetComponent<Text>();
			//text.text = "Stopped Recording";
		}

		if (Input.GetKeyDown(KeyCode.O))
		{
			MovementRecorder mr = GameObject.FindObjectOfType<MovementRecorder>();
			mr.Replay();
			uiStatus.setStatus("Replaying Movements");
			//GameObject textObj = GameObject.Find("CurrentState_Text");
			//Text text = textObj.GetComponent<Text>();
			//text.text = "Replaying";
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			WorldState ws = GameObject.FindObjectOfType<WorldState>();
			ws.Execute();
			uiStatus.setStatus("Executing");
			//Actuator act = new Actuator();
			//act.execute(movements);
			//GameObject textObj = GameObject.Find("CurrentState_Text");
			//Text text = textObj.GetComponent<Text>();
			//text.text = "Executing";
		}

		if (Input.GetKeyDown(KeyCode.T))
		{
			MovementRecorder mr = GameObject.FindObjectOfType<MovementRecorder>();
			mr.Execute();
			uiStatus.setStatus("Executing");
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			EndEffectorCollisionController cd = GameObject.FindObjectOfType<EndEffectorCollisionController>();
			if (cd.isSuctionActive())
			{
				cd.setSuction(false);
			}
			else
			{
				cd.setSuction(true);
			}
		}
		if (Input.GetKeyDown(KeyCode.L)) {
			UserActionRecorder actionRecorder = GameObject.FindObjectOfType<UserActionRecorder>();
			actionRecorder.PrintLog();
		}

	}
}
