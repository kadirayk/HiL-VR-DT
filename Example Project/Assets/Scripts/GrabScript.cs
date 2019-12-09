using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System.Net.Http;
using System;


using RosSharp.RosBridgeClient;

using std_msgs = RosSharp.RosBridgeClient.MessageTypes.Std;

using std_srvs = RosSharp.RosBridgeClient.MessageTypes.Std;

using rosapi = RosSharp.RosBridgeClient.MessageTypes.Rosapi;

public class GrabScript : MonoBehaviour
{

	public SteamVR_Input_Sources handType;
	public SteamVR_Action_Boolean grabAction;
	public float val;
	
	private bool isGrabbing;

	static readonly string uri = "ws://131.234.122.236:9090";

	RosSocket rosSocket;

	void Start()
	{
		rosSocket = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(uri));
		Debug.Log("script running");
	}


	// Update is called once per frame
	void Update()
	{
		if (grabAction.GetState(handType)) {
			InverseKinematics ik = GameObject.FindObjectOfType<InverseKinematics>();
			ik.DoIK();
		}

		if (grabAction.GetStateDown(handType))
		{
			isGrabbing = true;
			Debug.Log("Grab from: " + handType);
			if (handType.ToString().Equals("LeftHand"))
			{

				//rosSocket.CallService<rosapi.GetParamRequest, rosapi.GetParamResponse>("/rosapi/get_param", ServiceCallHandler, new rosapi.GetParamRequest("/rosdistro", "default"));

				Vector3 pos = GameObject.Find("LeftHand").transform.position;
				Debug.Log("LeftHand x:" + pos.x + " y:" + pos.y + " z:" + pos.z);
				//HttpClient httpClient = new HttpClient();
				//var url = "http://131.234.122.236:3000/move-to-position?x=" + pos.x + "&y=" + pos.y + "&z=" + pos.z;
				//Debug.Log("calling: " + url);
				//var response = httpClient.GetAsync(url).Result;
				//var contents = response.Content.ReadAsStringAsync().Result;
				//Debug.Log(contents.ToString());
			}
			else if (handType.ToString().Equals("RightHand")) {
				//Vector3 pos = GameObject.Find("RightHand").transform.position;
				//Debug.Log("RightHand x:" + pos.x + " y:" + pos.y + " z:" + pos.z);
				Debug.Log("holding");
				
			}
			//HttpClient httpClient = new HttpClient();
			//var response = httpClient.GetAsync("http://131.234.122.236:3000/move-to-position").Result;
			//var contents = response.Content.ReadAsStringAsync().Result;
			//Debug.Log(contents.ToString());
		}

		if (grabAction.GetStateDown(handType))
		{
			
			//Vector3 pos = GameObject.Find("Cube1").transform.position;
			//Debug.Log("x:" + pos.x + " y:" + pos.y + " z:" + pos.z);
			isGrabbing = false;
		}
	}

	public bool GetGrab()
	{
		return grabAction.GetState(handType);
	}

	private static void ServiceCallHandler(rosapi.GetParamResponse message)

	{

		Debug.Log("ROS distro: " + message.value);

	}
}
