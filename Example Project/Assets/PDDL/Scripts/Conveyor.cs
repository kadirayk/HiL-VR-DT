using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{

	private float movement_limit = 1.5f;
	public float speed = 500.0f;
	private bool isActive = false;



	public void StartMoving()
	{
		isActive = true;
	}

	public void StopMoving()
	{
		isActive = false;
	}

	public void setState(bool isActive)
	{
		this.isActive = isActive;
	}

	public bool isMoving()
	{
		return isActive;
	}


	void OnCollisionStay(Collision collision)
	{
		if (isActive)
		{
			Debug.Log("active:" + isActive);
			if (collision.gameObject.tag != "PDDLObject")
				return;

			Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>();
			if (rigidbody.position.x < movement_limit)
			{
				rigidbody.position += new Vector3(1, 0, 0) * speed * Time.deltaTime;
			}
			else
			{
				isActive = false;
			}
		}

	}

}
