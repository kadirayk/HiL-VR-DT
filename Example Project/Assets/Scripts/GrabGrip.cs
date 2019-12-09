using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class GrabGrip : MonoBehaviour
{
	public SteamVR_Input_Sources handType;
	public SteamVR_Action_Boolean grabAction;
	public float val;
	private CollisionDetection cd;

	void Start()
	{
		cd = GameObject.FindObjectOfType<CollisionDetection>();				
	}


	// Update is called once per frame
	void Update()
	{

		if (grabAction.GetStateDown(handType))
		{
			Debug.Log("drop in hand");
			cd.Drop();
		}
	}

	public bool GetGrab()
	{
		return grabAction.GetState(handType);
	}


}
