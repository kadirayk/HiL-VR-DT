using RosSharp.RosBridgeClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.PDDL.Scripts
{
	class Actuator
	{
		int timeout = 50; // 5 sec

		public void executeCommands(Queue<KeyValuePair<string, Vector3>> commands)
		{
			while (commands.Count > 0)
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
						System.Threading.Thread.Sleep(100);
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
						System.Threading.Thread.Sleep(100);
						targetPose = sc.Pose();
						timeCounter++;
					}
					sc.SetEndEffectorSuctionCup(true);
					sc.GetEndEffectorSuctionCup();
					bool suction = false;
					timeCounter = 0;
					while (!suction && timeCounter < timeout )
					{
						sc.GetEndEffectorSuctionCup();
						System.Threading.Thread.Sleep(100);
						suction = sc.GetSuction();
						timeCounter++;
					}

					dobotPose = UnityUtil.VRToDobotArm(command.Value + new Vector3(0, +0.02f, 0));
					Debug.Log("pickup target:" + UnityUtil.PositionToString(dobotPose));
					sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
					sc.GetPose();
					targetPose = sc.Pose();
					timeCounter = 0;
					while (!isAtPosition(targetPose, dobotPose) && timeCounter < timeout)
					{
						sc.GetPose();
						System.Threading.Thread.Sleep(100);
						targetPose = sc.Pose();
						timeCounter++;
					}
				}else if (command.Key.Equals("place"))
				{
					ServiceCaller sc = ServiceCaller.getInstance();
					Vector3 dobotPose = UnityUtil.VRToDobotArm(command.Value + new Vector3(0, +0.02f, 0));
					Debug.Log("place target:" + UnityUtil.PositionToString(dobotPose));
					sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
					sc.GetPose();
					Vector3 targetPose = sc.Pose();
					int timeCounter = 0;
					while (!isAtPosition(targetPose, dobotPose) && timeCounter < timeout)
					{
						sc.GetPose();
						System.Threading.Thread.Sleep(100);
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
						System.Threading.Thread.Sleep(100);
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
						System.Threading.Thread.Sleep(100);
						suction = sc.GetSuction();
						timeCounter++;
					}
					dobotPose = UnityUtil.VRToDobotArm(command.Value + new Vector3(0, +0.03f, 0));
					Debug.Log("pickup target:" + UnityUtil.PositionToString(dobotPose));
					sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
					sc.GetPose();
					targetPose = sc.Pose();
					timeCounter = 0;
					while (!isAtPosition(targetPose, dobotPose) && timeCounter < timeout)
					{
						sc.GetPose();
						System.Threading.Thread.Sleep(100);
						targetPose = sc.Pose();
						timeCounter++;
					}
				}
				else if (command.Key.Equals("stack"))
				{
					ServiceCaller sc = ServiceCaller.getInstance();
					Vector3 dobotPose = UnityUtil.VRToDobotArm(command.Value + new Vector3(0, +0.02f, 0));
					Debug.Log("stack target:" + UnityUtil.PositionToString(dobotPose));
					sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
					sc.GetPose();
					Vector3 targetPose = sc.Pose();
					int timeCounter = 0;
					while (!isAtPosition(targetPose, dobotPose) && timeCounter < timeout)
					{
						sc.GetPose();
						System.Threading.Thread.Sleep(100);
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
						System.Threading.Thread.Sleep(100);
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
						System.Threading.Thread.Sleep(100);
						suction = sc.GetSuction();
						timeCounter++;
					}
					dobotPose = UnityUtil.VRToDobotArm(command.Value + new Vector3(0, +0.03f, 0));
					Debug.Log("pickup target:" + UnityUtil.PositionToString(dobotPose));
					sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
					sc.GetPose();
					targetPose = sc.Pose();
					timeCounter = 0;
					while (!isAtPosition(targetPose, dobotPose) && timeCounter < timeout)
					{
						sc.GetPose();
						System.Threading.Thread.Sleep(100);
						targetPose = sc.Pose();
						timeCounter++;
					}
				}
			}
		}

		private bool isAtPosition(Vector3 v1, Vector3 v2)
		{
			return Vector3.Distance(v1, v2) < 1;
		}

	
		bool prevSuction = false;
		public void execute(Queue<RobotArmState> movements)
		{
			Debug.Log("execute item count:" + movements.Count);
			int step = 5;
			int item = 0;
			while (movements.Count > 0)
			{
				RobotArmState state = movements.Dequeue();
				if (item % step == 0)
				{
					ServiceCaller sc = ServiceCaller.getInstance();
					Vector3 dobotPose = UnityUtil.VRToDobotArm(state.EndEffectorPosition + new Vector3(0, -0.007f, 0));
					sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
					int timer = 0;
					while (!sc.SetPTPCmdReceived() && timer < timeout)
					{
						System.Threading.Thread.Sleep(100);
						timer++;
					}
					if (state.SuctionActive != prevSuction)
					{
						prevSuction = state.SuctionActive;
						//sc.SetEndEffectorSuctionCup(state.SuctionActive);

						while (!sc.SetEndEffectorSuctionCupReceived())
						{
							System.Threading.Thread.Sleep(100);
						}
					}

				}
				if (movements.Count == 1)
				{
					ServiceCaller sc = ServiceCaller.getInstance();
					Vector3 dobotPose = UnityUtil.VRToDobotArm(state.EndEffectorPosition);
					sc.SetPTPCmd(1, dobotPose.x, dobotPose.y, dobotPose.z, 0, false);
					while (!sc.SetPTPCmdReceived())
					{
						System.Threading.Thread.Sleep(100);
					}
				}
				item++;
			}
		}

	}
}
