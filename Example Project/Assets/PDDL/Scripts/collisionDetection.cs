using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
	private bool isHolding = false;

	private bool shouldDrop = false;

	private bool isAutomatedMode = false;

	ValueTuple<GameObject, int> frameCountToDropObject;

	public bool isSuctionActive()
	{
		return !shouldDrop;
	}

	public void Drop()
	{
		//Debug.Log("drop called");
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
		if (isHolding && shouldDrop)
		{
			//Debug.Log("update col");
			GameObject obj = frameCountToDropObject.Item1; //this.transform.GetChild(this.transform.childCount - 1).gameObject;
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

	void OnCollisionEnter(Collision collision)
	{
		//Debug.Log("col enter1 " + isHolding);
		Debug.Log("collision enter");
		if (!isHolding)
		{
			//Debug.Log("col enter2");
			GameObject obj = collision.gameObject;
			frameCountToDropObject = new ValueTuple<GameObject, int>(obj, 0);
			obj.transform.parent = this.transform;
			obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
			obj.GetComponent<Rigidbody>().isKinematic = true;

			isHolding = true;
			shouldDrop = false;
		}
	}
}
