using RosSharp.RosBridgeClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace Assets.PDDL.Scripts
{
	class Actuator: MonoBehaviour
	{

		bool isExecuteActive = false;

		int timeout = 50; // 5 sec

		bool codeGenerate = false;

		Queue<KeyValuePair<string, Vector3>> commands;

		public void executeCommands(Queue<KeyValuePair<string, Vector3>> com) {
			commands = com;
			isExecuteActive = true;
		}

		void Update()
		{
			if (isExecuteActive) {
				StartCoroutine(executeCommandsCoroutine());
				isExecuteActive = false;
			}
		}

		IEnumerator executeCommandsCoroutine()
		{
			ImageSubscriber imgSub = GameObject.FindObjectOfType<ImageSubscriber>();
			imgSub.StartImageUpdate();

			StringBuilder str = new StringBuilder();
			while (commands.Count > 0)
			{
				if (codeGenerate)
				{
					KeyValuePair<string, Vector3> command = commands.Dequeue();
					str.Append(command.Key + "(" + command.Value.x + ", " + command.Value.y + ", " + command.Value.z + ")");
					str.Append("\n");
				}
				else
				{

					KeyValuePair<string, Vector3> command = commands.Dequeue();
					if (command.Key.Equals("pick-up"))
					{
						ServiceCaller sc = ServiceCaller.getInstance();
						Vector3 dobotPose = UnityUtil.VRToDobotArm(command.Value + new Vector3(0, +0.01f, 0));
						Debug.Log("pickup target:" + UnityUtil.PositionToString(dobotPose));
						sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
						sc.GetPose();
						Vector3 targetPose = sc.Pose();
						int timeCounter = 0;
						while (!isAtPosition(targetPose, dobotPose) && timeCounter < timeout)
						{
							sc.GetPose();
							yield return new WaitForSeconds(0.1f);
							targetPose = sc.Pose();
							timeCounter++;
						}
						dobotPose = UnityUtil.VRToDobotArm(command.Value + new Vector3(0, -0.01f, 0));
						Debug.Log("pickup target:" + UnityUtil.PositionToString(dobotPose));
						sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
						sc.GetPose();
						targetPose = sc.Pose();
						timeCounter = 0;
						while (!isAtPosition(targetPose, dobotPose) && timeCounter < timeout)
						{
							sc.GetPose();
							yield return new WaitForSeconds(0.1f);
							targetPose = sc.Pose();
							timeCounter++;
						}
						sc.SetEndEffectorSuctionCup(true);
						sc.GetEndEffectorSuctionCup();
						bool suction = false;
						timeCounter = 0;
						while (!suction && timeCounter < timeout)
						{
							sc.GetEndEffectorSuctionCup();
							yield return new WaitForSeconds(0.1f);
							suction = sc.GetSuction();
							timeCounter++;
						}

						dobotPose = UnityUtil.VRToDobotArm(command.Value + new Vector3(0, +0.06f, 0));
						Debug.Log("pickup target:" + UnityUtil.PositionToString(dobotPose));
						sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
						sc.GetPose();
						targetPose = sc.Pose();
						timeCounter = 0;
						while (!isAtPosition(targetPose, dobotPose) && timeCounter < timeout)
						{
							sc.GetPose();
							yield return new WaitForSeconds(0.1f);
							targetPose = sc.Pose();
							timeCounter++;
						}
					}
					else if (command.Key.Equals("place"))
					{
						ServiceCaller sc = ServiceCaller.getInstance();
						Vector3 dobotPose = UnityUtil.VRToDobotArm(command.Value + new Vector3(0, +0.06f, 0));
						Debug.Log("place target:" + UnityUtil.PositionToString(dobotPose));
						sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
						sc.GetPose();
						Vector3 targetPose = sc.Pose();
						int timeCounter = 0;
						while (!isAtPosition(targetPose, dobotPose) && timeCounter < timeout)
						{
							sc.GetPose();
							yield return new WaitForSeconds(0.1f);
							targetPose = sc.Pose();
							timeCounter++;
						}
						dobotPose = UnityUtil.VRToDobotArm(command.Value + new Vector3(0, -0.02f, 0));
						Debug.Log("pickup target:" + UnityUtil.PositionToString(dobotPose));
						sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
						sc.GetPose();
						targetPose = sc.Pose();
						timeCounter = 0;
						while (!isAtPosition(targetPose, dobotPose) && timeCounter < timeout)
						{
							sc.GetPose();
							yield return new WaitForSeconds(0.1f);
							targetPose = sc.Pose();
							timeCounter++;
						}
						sc.SetEndEffectorSuctionCup(false);
						sc.GetEndEffectorSuctionCup();
						bool suction = true;
						timeCounter = 0;
						while (suction && timeCounter < timeout)
						{
							sc.GetEndEffectorSuctionCup();
							yield return new WaitForSeconds(0.1f);
							suction = sc.GetSuction();
							timeCounter++;
						}
						dobotPose = UnityUtil.VRToDobotArm(command.Value + new Vector3(0, +0.06f, 0));
						Debug.Log("pickup target:" + UnityUtil.PositionToString(dobotPose));
						sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
						sc.GetPose();
						targetPose = sc.Pose();
						timeCounter = 0;
						while (!isAtPosition(targetPose, dobotPose) && timeCounter < timeout)
						{
							sc.GetPose();
							yield return new WaitForSeconds(0.1f);
							targetPose = sc.Pose();
							timeCounter++;
						}
					}
					else if (command.Key.Equals("stack"))
					{
						ServiceCaller sc = ServiceCaller.getInstance();
						Vector3 dobotPose = UnityUtil.VRToDobotArm(command.Value + new Vector3(0, +0.06f, 0));
						Debug.Log("stack target:" + UnityUtil.PositionToString(dobotPose));
						sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
						sc.GetPose();
						Vector3 targetPose = sc.Pose();
						int timeCounter = 0;
						while (!isAtPosition(targetPose, dobotPose) && timeCounter < timeout)
						{
							sc.GetPose();
							yield return new WaitForSeconds(0.1f);
							targetPose = sc.Pose();
							timeCounter++;
						}
						dobotPose = UnityUtil.VRToDobotArm(command.Value + new Vector3(0, -0.02f, 0));
						Debug.Log("pickup target:" + UnityUtil.PositionToString(dobotPose));
						sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
						sc.GetPose();
						targetPose = sc.Pose();
						timeCounter = 0;
						while (!isAtPosition(targetPose, dobotPose) && timeCounter < timeout)
						{
							sc.GetPose();
							yield return new WaitForSeconds(0.1f);
							targetPose = sc.Pose();
							timeCounter++;
						}
						sc.SetEndEffectorSuctionCup(false);
						sc.GetEndEffectorSuctionCup();
						bool suction = true;
						timeCounter = 0;
						while (suction && timeCounter < timeout)
						{
							sc.GetEndEffectorSuctionCup();
							yield return new WaitForSeconds(0.1f);
							suction = sc.GetSuction();
							timeCounter++;
						}
						dobotPose = UnityUtil.VRToDobotArm(command.Value + new Vector3(0, +0.06f, 0));
						Debug.Log("pickup target:" + UnityUtil.PositionToString(dobotPose));
						sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
						sc.GetPose();
						targetPose = sc.Pose();
						timeCounter = 0;
						while (!isAtPosition(targetPose, dobotPose) && timeCounter < timeout)
						{
							sc.GetPose();
							yield return new WaitForSeconds(0.1f);
							targetPose = sc.Pose();
							timeCounter++;
						}
					}
				}
			}

			Debug.Log("Code Generation:");
			Debug.Log(str.ToString());

			UIStatus uiStatus = GameObject.FindObjectOfType<UIStatus>();
			uiStatus.setStatus("Executing Done");
			imgSub.StopImageUpdate();
		}

		private bool isAtPosition(Vector3 v1, Vector3 v2)
		{
			return Vector3.Distance(v1, v2) < 1;
		}

	
		bool prevSuction = false;

		//public void OldExec(Queue<RobotArmState> movements)
		//{
		//	Debug.Log("execute item count:" + movements.Count);
		//	int step = 5;
		//	int item = 0;
		//	while (movements.Count > 0)
		//	{
		//		RobotArmState state = movements.Dequeue();
		//		if (item % step == 0)
		//		{
		//			ServiceCaller sc = ServiceCaller.getInstance();
		//			Vector3 dobotPose = UnityUtil.VRToDobotArm(state.EndEffectorPosition + new Vector3(0, -0.007f, 0));
		//			sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
		//			int timer = 0;
		//			while (!sc.SetPTPCmdReceived() && timer < timeout)
		//			{
		//				yield return new WaitForSeconds(0.1f);
		//				timer++;
		//			}
		//			if (state.SuctionActive != prevSuction)
		//			{
		//				prevSuction = state.SuctionActive;
		//				//sc.SetEndEffectorSuctionCup(state.SuctionActive);

		//				while (!sc.SetEndEffectorSuctionCupReceived())
		//				{
		//					yield return new WaitForSeconds(0.1f);
		//				}
		//			}

		//		}
		//		if (movements.Count == 1)
		//		{
		//			ServiceCaller sc = ServiceCaller.getInstance();
		//			Vector3 dobotPose = UnityUtil.VRToDobotArm(state.EndEffectorPosition);
		//			sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
		//			while (!sc.SetPTPCmdReceived())
		//			{
		//				yield return new WaitForSeconds(0.1f);
		//			}
		//		}
		//		item++;
		//	}
		//}

	}
}
