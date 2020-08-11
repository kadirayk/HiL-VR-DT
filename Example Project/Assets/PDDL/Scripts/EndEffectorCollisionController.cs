using Assets.PDDL;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndEffectorCollisionController : MonoBehaviour
{
	private bool isHolding = false;

	private bool shouldDrop = false;

	private bool suctionActive = false;

	private bool isAutomatedMode = false;

	ValueTuple<GameObject, int> frameCountToDropObject;

	private bool isCollisionWithCube = false;

	public bool isHoldingCube()
	{
		return isHolding;
	}

	public bool isCollidingWithCube()
	{
		return isCollisionWithCube;
	}
	public void setSuction(bool suct)
	{
		suctionActive = suct;
	}

	public bool isSuctionActive()
	{
		return suctionActive;
		//return !shouldDrop;
	}

	public void Drop()
	{
		Debug.Log("drop called");
		shouldDrop = true;
	}

	public void AutomatedMode(Boolean active)
	{
		isAutomatedMode = active;
	}

	void OnCollisionStay(Collision collision)
	{
		//Debug.Log("col");
		//if (!isHolding && Input.GetKeyDown(KeyCode.Space))
		//{
		//	Debug.Log("hold it");
		//	collision.gameObject.transform.parent = this.transform;
		//	collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
		//	isHolding = true;
		//}

		//if (isHolding && Input.GetKeyDown(KeyCode.Space))
		//{
		//	collision.gameObject.transform.parent.DetachChildren();
		//	collision.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		//	isHolding = false;
		//}

	}

	private void Update()
	{

		if (!suctionActive)
		{
			GameObject obj = frameCountToDropObject.Item1; //this.transform.GetChild(this.transform.childCount - 1).gameObject;
			if (obj == null)
			{
				return;
			}
			obj.GetComponent<Rigidbody>().isKinematic = false;
			//obj.GetComponent<Rigidbody>().useGravity = true;
			this.transform.DetachChildren();
			//Debug.Log("is ki:" + this.transform.GetChild(this.transform.childCount - 1).gameObject.GetComponent<Rigidbody>().isKinematic);
			frameCountToDropObject.Item2 += 1;
			isHolding = false;
			shouldDrop = false;
		}
		else if (isAutomatedMode && frameCountToDropObject.Item2 >= 1)
		{
			GameObject obj = frameCountToDropObject.Item1;
			//obj.GetComponent<Rigidbody>().useGravity = false;
			//obj.GetComponent<Rigidbody>().isKinematic = true;
		}
	}


	private void OnTriggerEnter(Collider other)
	{

		GameObject obj = other.gameObject;
		if (obj.name.StartsWith("cube_"))
		{
			isCollisionWithCube = true;
			UnityUtil.highLightStart(obj);
			StartCoroutine(UnityUtil.HapticFeedback(0.1f));

		}
	}

	private void OnTriggerStay(Collider other)
	{
		//Debug.Log("End Effector Trigger enter");
		if (suctionActive)
		{
			GameObject obj = other.gameObject;
			if (obj.name.StartsWith("cube_"))
			{
				//Debug.Log("collision obj:" + obj.name);
				frameCountToDropObject = new ValueTuple<GameObject, int>(obj, 0);
				obj.transform.parent = this.transform;
				obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
				obj.GetComponent<Rigidbody>().isKinematic = true;
				//Debug.Log("holding and suction active");
				isHolding = true;
				shouldDrop = false;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		GameObject obj = other.gameObject;
		if (obj.name.StartsWith("cube_"))
		{
			UnityUtil.highLightEnd(obj);
			isCollisionWithCube = false;
		}

	}

	void OnCollisionEnter(Collision collision)
	{
		//Debug.Log("col enter1 " + isHolding);
		// Disabled this code so that endeffector doesnt directly move cubes instead used OnTriggerEnter
		//Debug.Log("collision enter");
		//if (suctionActive)
		//{
		//	//Debug.Log("col enter2");

		//	GameObject obj = collision.gameObject;
		//	Debug.Log("collision obj:" + obj.name);
		//	frameCountToDropObject = new ValueTuple<GameObject, int>(obj, 0);
		//	obj.transform.parent = this.transform;
		//	obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
		//	obj.GetComponent<Rigidbody>().isKinematic = true;

		//	Debug.Log("holding and suction active");

		//	isHolding = true;
		//	shouldDrop = false;
		//}
	}
}
