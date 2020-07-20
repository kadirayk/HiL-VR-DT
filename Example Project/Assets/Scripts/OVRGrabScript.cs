using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRInput;
using AutoQUEST;

public class OVRGrabScript : MonoBehaviour
{
	InverseKinematics ik;
	public Controller controller;
	// Start is called before the first frame update
	void Start()
	{
		ik = GameObject.FindObjectOfType<InverseKinematics>();
	}

	// Update is called once per frame
	void Update()
	{
		if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger, controller) || OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, controller))
		{
			ik.DoIK();
		}
	}
}
