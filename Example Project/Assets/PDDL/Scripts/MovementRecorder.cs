using Assets.PDDL;
using RosSharp.RosBridgeClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Assets.PDDL.Scripts;

public class MovementRecorder : MonoBehaviour
{
	public Transform dobotLoaderBaseRotator;
	public Transform dobotLoaderl2_arm;
	public Transform dobotLoaderl3_arm;
	public Transform dobotLoaderHand;
	public Transform dobotLoaderSuctionCup;
	private CollisionDetection dobotLoaderCollisionDetection;
	public GameObject dobotLoaderEndEffector;
	private bool dobotLoaderSuctionState;

	public Transform dobotRailBaseRotator;
	public Transform dobotRaill2_arm;
	public Transform dobotRaill3_arm;
	public Transform dobotRailHand;
	public Transform dobotRailSuctionCup;
	private CollisionDetection dobotRailCollisionDetection;
	public GameObject dobotRailEndEffector;
	private bool dobotRailSuctionState;

	private ServiceCaller sc;


	private Queue<RoboticSystemState> recordedMovements = new Queue<RoboticSystemState>();
	private bool isRecording = false;
	private bool isReplaying = false;
	private bool isReplayFinished = true;
	public void StartRecording()
	{
		isRecording = true;
	}

	public void StopRecording()
	{
		isRecording = false;
		Debug.Log("command count:" + recordedMovements.Count);
	}

	public void Replay()
	{
		isReplaying = true;
		isReplayFinished = false;
	}

	public bool isReplayDone()
	{
		return isReplayFinished;
	}

	private void checkObjectsForNull()
	{
		if (dobotLoaderBaseRotator == null)
		{
			Debug.LogError("dobotLoaderbaseRotator null");
		}
		if (dobotLoaderl2_arm == null)
		{
			Debug.LogError("dobotLoaderl2_arm null");
		}
		if (dobotLoaderl3_arm == null)
		{
			Debug.LogError("dobotLoaderl3_arm null");
		}
		if (dobotLoaderHand == null)
		{
			Debug.LogError("dobotLoaderhand null");
		}
		if (dobotLoaderCollisionDetection == null)
		{
			Debug.LogError("dobotLoadercollisionDetection null");
		}
		if (dobotRailBaseRotator == null)
		{
			Debug.LogError("dobotRailbaseRotator null");
		}
		if (dobotRaill2_arm == null)
		{
			Debug.LogError("dobotRaill2_arm null");
		}
		if (dobotRaill3_arm == null)
		{
			Debug.LogError("dobotRaill3_arm null");
		}
		if (dobotRailHand == null)
		{
			Debug.LogError("dobotRailhand null");
		}
		if (dobotRailCollisionDetection == null)
		{
			Debug.LogError("dobotRailcollisionDetection null");
		}
	}

	public void SetRecordedMovements(Queue<RoboticSystemState> recordedMovements)
	{
		this.recordedMovements = recordedMovements;
	}

	public Queue<RoboticSystemState> GetRecordedMovements()
	{
		return recordedMovements;
	}

	void OnDestroy()
	{

	}

	// Start is called before the first frame update
	void Start()
	{
		GameObject dobotLoader = GameObject.Find("DobotLoader");
		foreach (Transform child in dobotLoader.GetComponentsInChildren<Transform>())
		{
			if (child.name.Equals("magicianSuctionCup"))
			{
				if (child.GetComponent<CollisionDetection>() != null)
				{
					dobotLoaderCollisionDetection = child.GetComponent<CollisionDetection>();
				}
			}
		}
		GameObject dobotRail = GameObject.Find("DobotRail");
		foreach (Transform child in dobotRail.GetComponentsInChildren<Transform>())
		{
			if (child.name.Equals("magicianSuctionCup"))
			{
				if (child.GetComponent<CollisionDetection>() != null)
				{
					dobotRailCollisionDetection = child.GetComponent<CollisionDetection>();
				}
			}

		}
		checkObjectsForNull();
		sc = ServiceCaller.getInstance();
	}

	// Update is called once per frame
	void Update()
	{
		if (isRecording)
		{
			RobotArmState dobotLoaderState = new RobotArmState(
				dobotLoaderBaseRotator.transform.localRotation.eulerAngles.y,
				dobotLoaderl2_arm.transform.localRotation.eulerAngles.x,
				dobotLoaderl3_arm.transform.localRotation.eulerAngles.x,
				dobotLoaderCollisionDetection.isSuctionActive(),
				dobotLoaderEndEffector.transform.position);

			RobotArmState dobotRailState = new RobotArmState(
				dobotRailBaseRotator.transform.localRotation.eulerAngles.y,
				dobotRaill2_arm.transform.localRotation.eulerAngles.x,
				dobotRaill3_arm.transform.localRotation.eulerAngles.x,
				dobotRailCollisionDetection.isSuctionActive(),
				dobotRailEndEffector.transform.position);

			bool conveyorState = GameObject.FindObjectOfType<Conveyor>().isMoving();

			RoboticSystemState systemState = new RoboticSystemState(dobotLoaderState, dobotRailState, conveyorState);
			recordedMovements.Enqueue(systemState);

		}

		if (isReplaying)
		{
			if (recordedMovements.Count != 0)
			{
				RoboticSystemState systemState = recordedMovements.Dequeue();
				dobotLoaderBaseRotator.transform.localRotation = Quaternion.Euler(0, systemState.DobotLoaderState.BaseAngle, 0);
				dobotLoaderl2_arm.transform.localRotation = Quaternion.Euler(systemState.DobotLoaderState.L2Angle, 0, 0);
				dobotLoaderl3_arm.transform.localRotation = Quaternion.Euler(systemState.DobotLoaderState.L3Angle, 0, 0);
				float dobotLoaderHandRotation = -dobotLoaderl2_arm.transform.localRotation.eulerAngles.x - dobotLoaderl3_arm.transform.localRotation.eulerAngles.x;
				dobotLoaderHand.transform.localRotation = Quaternion.Euler(dobotLoaderHandRotation, 0, 0);
				dobotLoaderSuctionCup.transform.localRotation = Quaternion.Euler(0, -systemState.DobotLoaderState.BaseAngle, 0);
				if (!systemState.DobotLoaderState.SuctionActive)
				{
					dobotLoaderCollisionDetection.Drop();
				}


				dobotRailBaseRotator.transform.localRotation = Quaternion.Euler(0, systemState.DobotRailState.BaseAngle, 0);
				dobotRaill2_arm.transform.localRotation = Quaternion.Euler(systemState.DobotRailState.L2Angle, 0, 0);
				dobotRaill3_arm.transform.localRotation = Quaternion.Euler(systemState.DobotRailState.L3Angle, 0, 0);
				float dobotRailHandRotation = -dobotRaill2_arm.transform.localRotation.eulerAngles.x - dobotRaill3_arm.transform.localRotation.eulerAngles.x;
				dobotRailHand.transform.localRotation = Quaternion.Euler(dobotRailHandRotation, 0, 0);
				dobotRailSuctionCup.transform.localRotation = Quaternion.Euler(0, -systemState.DobotRailState.BaseAngle, 0);
				if (!systemState.DobotRailState.SuctionActive)
				{
					dobotRailCollisionDetection.Drop();
				}

				Conveyor conveyor = GameObject.FindObjectOfType<Conveyor>();
				conveyor.setState(systemState.ConveyorState);

				//Vector3 dobotPose = UnityUtil.VRToDobotArm(state.EndEffectorPosition);
				//Debug.Log("in replay pose:" + UnityUtil.PositionToString(dobotPose));
				//sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
				//sc.SetEndEffectorSuctionCup(state.SuctionActive);
				Actuator actuator = new Actuator();
				isReplayFinished = false;
			}
			else
			{
				isReplaying = false;
				isReplayFinished = true;
				GameObject textObj = GameObject.Find("CurrentState_Text");
				Text text = textObj.GetComponent<Text>();
				text.text = "Replaying Done";
				GameObject cube = GameObject.Find("RedCube1");
			}
		}
	}
}
