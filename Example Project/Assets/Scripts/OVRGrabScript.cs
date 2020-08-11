using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRInput;
using AutoQUEST;

public class OVRGrabScript : MonoBehaviour
{
	InverseKinematics ik;
	public Controller controller;
	bool collisionWithSphere = false;
	HintsManager hintsManager;
	EndEffectorCollisionController endEffectorCollisionController;
	ModeManager modeManager;

	// Start is called before the first frame update
	void Start()
	{
		ik = GameObject.FindObjectOfType<InverseKinematics>();
		hintsManager = GameObject.FindObjectOfType<HintsManager>();
		endEffectorCollisionController = GameObject.FindObjectOfType<EndEffectorCollisionController>();
		modeManager = GameObject.FindObjectOfType<ModeManager>();
	}

	// Update is called once per frame
	void Update()
	{
		if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger, controller) || OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, controller))
		{
			if (collisionWithSphere) {

				ik.DoIK();

				if (modeManager.GetMode()==Mode.manual) {
					if (endEffectorCollisionController.isHoldingCube())
					{
						hintsManager.setStatus(HintsManager.HOLDING_CUBE);
					}
					else
					{
						hintsManager.setStatus(HintsManager.MOVING_THE_ARM);
					}
				}
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.name=="Sphere" || other.name == "magicianSuctionCup") {
			collisionWithSphere = true;
		} 
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.name == "Sphere" || other.name == "magicianSuctionCup")
		{
			collisionWithSphere = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.name == "Sphere" || other.name == "magicianSuctionCup")
		{
			collisionWithSphere = false;
		}
	}


}
