using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
	private bool isHolding = false;

	private bool shouldDrop = false;

	public bool isSuctionActive() {
		return !shouldDrop;
	}

	public void Drop() {
		//Debug.Log("drop called");
		shouldDrop = true;
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
			this.transform.GetChild(this.transform.childCount - 1).gameObject.GetComponent<Rigidbody>().isKinematic = false;
			this.transform.DetachChildren();
			
			//Debug.Log("is ki:" + this.transform.GetChild(this.transform.childCount - 1).gameObject.GetComponent<Rigidbody>().isKinematic);

			isHolding = false;
			shouldDrop = false;
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		//Debug.Log("col enter1 " + isHolding);
		//if (!isHolding)
		{
			//Debug.Log("col enter2");
			collision.gameObject.transform.parent = this.transform;
			collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
			isHolding = true;
			shouldDrop = false;
		}
	}
}
