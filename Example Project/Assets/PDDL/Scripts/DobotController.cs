using Assets.PDDL.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DobotController : MonoBehaviour
{
	private GameObject BaseRotator; // magician_link_1
	private GameObject LowerArm; // magician_link_2
	private GameObject UpperArm; // magician_link_3
	private GameObject DobotHand; // magician_link_4
	private InverseKinematics ik;
	private MovementRecorder mr;
	private GameObject dobot;
	private CollisionDetection cd;
	private int captureCount = 0;

	Queue<RoboticSystemState> movements;

	// Start is called before the first frame update
	void Start()
	{
		activateDobot("DobotLoader");
	}

	private void activateDobot(string name)
	{
		GameObject dobot = GameObject.Find(name);
		ik = dobot.GetComponent<InverseKinematics>();
		mr = GameObject.FindObjectOfType<MovementRecorder>();

		foreach (Transform child in dobot.GetComponentsInChildren<Transform>())
		{
			if (child.name.Equals("magician_link_1"))
			{
				BaseRotator = child.gameObject;
			}
			else if (child.name.Equals("magician_link_2"))
			{
				LowerArm = child.gameObject;
			}
			else if (child.name.Equals("magician_link_3"))
			{
				UpperArm = child.gameObject;
			}
			else if (child.name.Equals("magician_link_4"))
			{
				DobotHand = child.gameObject;
			} else if (child.name.Equals("magicianSuctionCup")) {
				if (child.GetComponent<CollisionDetection>()!=null)
				{
					cd = child.GetComponent<CollisionDetection>();
				}
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			ScreenCapture.CaptureScreenshot("vrdobotss" + captureCount + ".png", 4);
			captureCount++;
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			activateDobot("DobotLoader");
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			activateDobot("DobotRail");
		}

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

		if (Input.GetKeyDown(KeyCode.S))
		{
			WorldState ws = GameObject.FindObjectOfType<WorldState>();
			ws.Initial();
		}

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

		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			ik.moveTarget("left");
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			ik.moveTarget("right");
		}
		if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			ik.moveTarget("up");
		}
		if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			ik.moveTarget("down");
		}
		if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			ik.moveTarget("fw");
		}
		if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			ik.moveTarget("bw");
		}

		if (Input.GetKeyDown(KeyCode.R))
		{
			mr.StartRecording();
			GameObject textObj = GameObject.Find("CurrentState_Text");
			Text text = textObj.GetComponent<Text>();
			text.text = "Recording Movements";
		}

		if (Input.GetKeyDown(KeyCode.P))
		{
			mr.StopRecording();
			movements = new Queue<RoboticSystemState>(mr.GetRecordedMovements());
			GameObject textObj = GameObject.Find("CurrentState_Text");
			Text text = textObj.GetComponent<Text>();
			text.text = "Stopped Recording";
		}

		if (Input.GetKeyDown(KeyCode.O))
		{
			mr.Replay();
			GameObject textObj = GameObject.Find("CurrentState_Text");
			Text text = textObj.GetComponent<Text>();
			text.text = "Replaying";
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			Actuator act = new Actuator();
			//act.execute(movements);
			GameObject textObj = GameObject.Find("CurrentState_Text");
			Text text = textObj.GetComponent<Text>();
			text.text = "Executing";
		}

		if (Input.GetKeyDown(KeyCode.C))
		{
			Conveyor conveyor = GameObject.FindObjectOfType<Conveyor>();
			conveyor.StartMoving();
		}

		if (Input.GetKeyDown(KeyCode.V))
		{
			Conveyor conveyor = GameObject.FindObjectOfType<Conveyor>();
			conveyor.StopMoving();
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			cd.Drop();
		}

	}
}
