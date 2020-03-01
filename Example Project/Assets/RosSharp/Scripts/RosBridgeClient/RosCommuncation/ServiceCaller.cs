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


		public void SetPTPCmd(byte ptpMode, float x, float y, float z, float r, bool isQueued) {
			rosSocket = new RosSocket(new RosBridgeClient.Protocols.WebSocketSharpProtocol(uri));
			SetPTPCmdRequest request = new SetPTPCmdRequest(ptpMode, x, y, z, r, isQueued);
			rosSocket.CallService<SetPTPCmdRequest, SetPTPCmdResponse>("/Dobot_Loader/SetPTPCmd", SetPTPCmdResponseHandler, request);
		}

		private void SetPTPCmdResponseHandler(SetPTPCmdResponse message)
		{
			Debug.Log("SetPTPCmd success:"  + message.result);
			rosSocket.Close();
		}

		public void GetPose()
		{
			rosSocket = new RosSocket(new RosBridgeClient.Protocols.WebSocketSharpProtocol(uri));
			rosSocket.CallService<GetPoseRequest, GetPoseResponse>("/Dobot_Loader/GetPose", GetPoseResponseHandler, new GetPoseRequest());
		}

		private void GetPoseResponseHandler(GetPoseResponse message)
		{
			Debug.Log("GetPose response x: " + message.jointAngle[0] + " y:" + message.jointAngle[1] + " z:" + message.jointAngle[2] + " t:" + message.jointAngle[3]);
			rosSocket.Close();
		}

		public void CallService()
		{
			rosSocket = new RosSocket(new RosBridgeClient.Protocols.WebSocketSharpProtocol(uri));
			rosSocket.CallService<rosapi.GetParamRequest, rosapi.GetParamResponse>("/rosapi/get_param", ServiceCallHandler, new rosapi.GetParamRequest("/rosdistro", "default"));
		}

		private void ServiceCallHandler(rosapi.GetParamResponse message)
		{
			Debug.Log("ROS distro: " + message.value);
			rosSocket.Close();
		}

	}
}
