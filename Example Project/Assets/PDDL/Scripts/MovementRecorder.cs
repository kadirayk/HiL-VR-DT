using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementRecorder : MonoBehaviour
{
	public Transform baseRotator;
	public Transform l2_arm;
	public Transform l3_arm;
	public Transform hand;
	public Transform target;
	public CollisionDetection collisionDetection;


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

	public void SetRecordedMovements(Queue<RobotArmState> recordedMovements) {
		this.recordedMovements = recordedMovements;
	}

	// Start is called before the first frame update
	void Start()
	{
		collisionDetection = GameObject.FindObjectOfType<CollisionDetection>();
		checkObjectsForNull();
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
				target.transform.position);
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
				if (!state.SuctionActive)
				{
					//Debug.Log("drop in replay");
					collisionDetection.Drop();
				}
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
