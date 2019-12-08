using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
	private bool isHolding = false;



	void OnCollisionStay(Collision collision)
	{
		Debug.Log("col");
		if (!isHolding && Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log("hold it");
			collision.gameObject.transform.parent = this.transform;
			collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
			isHolding = true;
		}

		if (isHolding && Input.GetKeyDown(KeyCode.Space))
		{
			collision.gameObject.transform.parent.DetachChildren();
			collision.gameObject.GetComponent<Rigidbody>().isKinematic = false;
			isHolding = false;
		}
	}

	private void Update()
	{
		if (isHolding && Input.GetKeyDown(KeyCode.Space))
		{
			this.transform.GetChild(this.transform.childCount - 1).gameObject.GetComponent<Rigidbody>().isKinematic = false;
			this.transform.DetachChildren();
			isHolding = false;
	    }
	}

	void OnCollisionEnter(Collision collision)
	{
		if (!isHolding)
		{
			collision.gameObject.transform.parent = this.transform;
			collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
			isHolding = true;
		}
	}
}
