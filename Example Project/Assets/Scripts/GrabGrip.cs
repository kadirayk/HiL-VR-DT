using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class GrabGrip : MonoBehaviour
{
	public SteamVR_Input_Sources handType;
	public SteamVR_Action_Boolean grabAction;
	public float val;
	private EndEffectorCollisionController cd;

	void Start()
	{
		cd = GameObject.FindObjectOfType<EndEffectorCollisionController>();				
	}

	void Update()
	{

		if (grabAction.GetStateDown(handType))
		{
			Debug.Log("drop in grip down");
			cd.Drop();
		}

		if (grabAction.GetStateUp(handType))
		{
			Debug.Log("drop in grip up");
			cd.Drop();
		}
	}

	public bool GetGrab()
	{
		return grabAction.GetState(handType);
	}


}
