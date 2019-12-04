using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementRecorder : MonoBehaviour
{
	public Transform baseRotator;
	public Transform l2_arm;
	public Transform l3_arm;
	public Transform hand;
	public Transform target;

	Queue<RobotArmState> recordedMovements = new Queue<RobotArmState>();
	private bool isRecording = false;
	private bool isReplaying = false;
	public void StartRecording()
	{
		isRecording = true;
	}

	public void StopRecording()
	{
		isRecording = false;
	}

	public void Replay() {
		isReplaying = true;
	}

	// Start is called before the first frame update
	void Start()
	{

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
				target.transform.position);
			recordedMovements.Enqueue(state);
		}

		if (isReplaying) {
			if (recordedMovements.Count != 0)
			{
				RobotArmState state = recordedMovements.Dequeue();
				baseRotator.transform.localRotation = Quaternion.Euler(0, state.BaseAngle, 0);
				l2_arm.transform.localRotation = Quaternion.Euler(state.L2Angle, 0, 0);
				l3_arm.transform.localRotation = Quaternion.Euler(state.L3Angle, 0, 0);
				float handRotation = -l2_arm.transform.localRotation.eulerAngles.x - l3_arm.transform.localRotation.eulerAngles.x;
				hand.transform.localRotation = Quaternion.Euler(handRotation, 0, 0);
			}
			else
			{
				isReplaying = false;
			}
		}
	}
}
