using RosSharp.RosBridgeClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using RosSharp.RosBridgeClient;
using std_msgs = RosSharp.RosBridgeClient.MessageTypes.Std;
using std_srvs = RosSharp.RosBridgeClient.MessageTypes.Std;
using rosapi = RosSharp.RosBridgeClient.MessageTypes.Rosapi;
using UnityEngine;
using RosSharp.RosBridgeClient.MessageTypes.Dobot;

namespace RosSharp.RosBridgeClient
{
	public class ServiceCaller
	{
		static readonly string uri = "ws://131.234.122.236:9090";

		private RosSocket rosSocket;
		private bool isMessageReceived;
		private static ServiceCaller instance;
		private bool setPTPCmdReceived;
		private bool setEndEffectorSuctionCupReceived;
		private Vector3 dobotLoaderPose = new Vector3(0,0,0);
		private bool endEffectorSuction;

		private ServiceCaller()
		{

			rosSocket = new RosSocket(new RosBridgeClient.Protocols.WebSocketSharpProtocol(uri));

		}

		public static ServiceCaller getInstance()
		{
			if (instance == null)
			{
				instance = new ServiceCaller();
			}
			return instance;
		}


		public void SetEndEffectorSuctionCup(bool suck) {
			SetEndEffectorSuctionCupRequest request = new SetEndEffectorSuctionCupRequest(1, Convert.ToByte(suck), false);
			rosSocket.CallService<SetEndEffectorSuctionCupRequest, SetEndEffectorSuctionCupResponse>("/Dobot_Loader/SetEndEffectorSuctionCup", SetEndEffectorSuctionCupHandler, request);
			setEndEffectorSuctionCupReceived = false;
		}

		private void SetEndEffectorSuctionCupHandler(SetEndEffectorSuctionCupResponse message) {
			setEndEffectorSuctionCupReceived = true;
			//Debug.Log("SetEndEffectorSuctionCup success:" + message.queuedCmdIndex);
		}

		public bool SetEndEffectorSuctionCupReceived()
		{
			return setEndEffectorSuctionCupReceived;
		}


		public void SetPTPCmd(byte ptpMode, float x, float y, float z, float r, bool isQueued)
		{
			SetPTPCmdRequest request = new SetPTPCmdRequest(ptpMode, x, y, z, r, isQueued);
			rosSocket.CallService<SetPTPCmdRequest, SetPTPCmdResponse>("/Dobot_Loader/SetPTPCmd", SetPTPCmdResponseHandler, request);
			setPTPCmdReceived = false;
		}

		private void SetPTPCmdResponseHandler(SetPTPCmdResponse message)
		{
			setPTPCmdReceived = true;
			Debug.Log("SetPTPCmd success:" + message.queuedCmdIndex);
		}

		public bool SetPTPCmdReceived()
		{
			return setPTPCmdReceived;
		}


		public void GetEndEffectorSuctionCup()
		{
			rosSocket.CallService<GetEndEffectorSuctionCupRequest, GetEndEffectorSuctionCupResponse>("/Dobot_Loader/GetEndEffectorSuctionCup", GetEndEffectorSuctionCupHandler, new GetEndEffectorSuctionCupRequest());
		}

		private void GetEndEffectorSuctionCupHandler(GetEndEffectorSuctionCupResponse message) {
			endEffectorSuction = Convert.ToBoolean(message.suck);
		}
		
		public bool GetSuction()
		{
			return endEffectorSuction;
		}

		public void GetPose()
		{
			rosSocket.CallService<GetPoseRequest, GetPoseResponse>("/Dobot_Loader/GetPose", GetPoseResponseHandler, new GetPoseRequest());
		}

		private void GetPoseResponseHandler(GetPoseResponse message)
		{
			dobotLoaderPose = new Vector3(message.x, message.y, message.z);
		}

		public Vector3 Pose()
		{
			return dobotLoaderPose;
		}

		public void CallService()
		{
			rosSocket = new RosSocket(new RosBridgeClient.Protocols.WebSocketSharpProtocol(uri));
			rosSocket.CallService<rosapi.GetParamRequest, rosapi.GetParamResponse>("/rosapi/get_param", ServiceCallHandler, new rosapi.GetParamRequest("/rosdistro", "default"));
		}

		private void ServiceCallHandler(rosapi.GetParamResponse message)
		{
			Debug.Log("ROS distro: " + message.value);
		}

		public void Terminate()
		{
			if (rosSocket != null) { rosSocket.Close(); }
		}

	}
}
