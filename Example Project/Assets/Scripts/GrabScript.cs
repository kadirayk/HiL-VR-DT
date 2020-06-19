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
	// Trigger Grab
	public SteamVR_Input_Sources handType;
	public SteamVR_Action_Boolean grabAction;
	public float val;
	
	private bool isGrabbing;

	//static readonly string uri = "ws://131.234.122.236:9090";
	
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
				//Vector3 pos = GameObject.Find("LeftHand").transform.position;
				//Debug.Log("LeftHand x:" + pos.x + " y:" + pos.y + " z:" + pos.z);
				
			}
			else if (handType.ToString().Equals("RightHand")) {
				//Vector3 pos = GameObject.Find("RightHand").transform.position;
				//Debug.Log("RightHand x:" + pos.x + " y:" + pos.y + " z:" + pos.z);
				//Debug.Log("holding");
				
			}
		}

		if (grabAction.GetStateDown(handType))
		{
			isGrabbing = false;
		}
	}

	public bool GetGrab()
	{
		return grabAction.GetState(handType);
	}

}
