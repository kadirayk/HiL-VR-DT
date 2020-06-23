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
	public Transform baseRotator;
	public Transform l2_arm;
	public Transform l3_arm;
	public Transform hand;
	public Transform target;
	public Transform suctionCup;
	public CollisionDetection collisionDetection;
	GameObject endEffector;
	private ServiceCaller sc;
	private bool suctionState;
	UIStatus uiStatus;
	private bool isAutoMode = false;

	Queue<RobotArmState> recordedMovements = new Queue<RobotArmState>();
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
		if (baseRotator == null)
		{
			Debug.LogError("baseRotator null");
		}
		if (l2_arm == null)
		{
			Debug.LogError("l2_arm null");
		}
		if (l3_arm == null)
		{
			Debug.LogError("l3_arm null");
		}
		if (hand == null)
		{
			Debug.LogError("hand null");
		}
		if (target == null)
		{
			Debug.LogError("target null");
		}
		if (collisionDetection == null)
		{
			Debug.LogError("collisionDetection null");
		}
	}

	public void SetAutomatedMode(bool mode) {
		isAutoMode = mode;
	}

	public void SetRecordedMovements(Queue<RobotArmState> recordedMovements)
	{
		this.recordedMovements = recordedMovements;
	}

	public Queue<RobotArmState> GetRecordedMovements() {
		return recordedMovements;
	}

	void OnDestroy()
	{
		
	}

	// Start is called before the first frame update
	void Start()
	{
		collisionDetection = GameObject.FindObjectOfType<CollisionDetection>();
		endEffector = GameObject.Find("magician_end_effector");
		checkObjectsForNull();
		sc = ServiceCaller.getInstance();
		uiStatus = GameObject.FindObjectOfType<UIStatus>();
	}

	// Update is called once per frame
	void Update()
	{
		if (isRecording)
		{
			RobotArmState state = new RobotArmState(
				baseRotator.transform.localRotation.eulerAngles.y,
				l2_arm.transform.localRotation.eulerAngles.x,
				l3_arm.transform.localRotation.eulerAngles.x,
				collisionDetection.isSuctionActive(),
				endEffector.transform.position);
			recordedMovements.Enqueue(state);
		}

		if (isReplaying)
		{
			if (recordedMovements.Count != 0)
			{
				RobotArmState state = recordedMovements.Dequeue();
				baseRotator.transform.localRotation = Quaternion.Euler(0, state.BaseAngle, 0);
				l2_arm.transform.localRotation = Quaternion.Euler(state.L2Angle, 0, 0);
				l3_arm.transform.localRotation = Quaternion.Euler(state.L3Angle, 0, 0);
				float handRotation = -l2_arm.transform.localRotation.eulerAngles.x - l3_arm.transform.localRotation.eulerAngles.x;
				hand.transform.localRotation = Quaternion.Euler(handRotation, 0, 0);
				suctionCup.transform.localRotation = Quaternion.Euler(0, -state.BaseAngle, 0);
				collisionDetection.setSuction(state.SuctionActive);
				
				//Vector3 dobotPose = UnityUtil.VRToDobotArm(state.EndEffectorPosition);
				//Debug.Log("in replay pose:" + UnityUtil.PositionToString(dobotPose));
				//sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
				//sc.SetEndEffectorSuctionCup(state.SuctionActive);
				Actuator actuator = new Actuator();
				isReplayFinished = false;
				if (!isAutoMode)
				{
					uiStatus.setStatus("Replaying Movements");
				}
			}
			else
			{
				isReplaying = false;
				isReplayFinished = true;
				if (!isAutoMode)
				{
					uiStatus.setStatus("Replaying Done");
				}
				
			}
		}
	}
}
