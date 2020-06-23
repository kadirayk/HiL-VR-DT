using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
	private Vector3 mOffset;
	private float mZCoord;
	private string target;
	private InverseKinematics ik;

	private void Start()
	{
		target = transform.parent.name;
		if (target.Equals("DobotLoaderTargetIK"))
		{
			GameObject dobot = GameObject.Find("DobotLoader");
			ik = dobot.GetComponent<InverseKinematics>();
		}
		else if (target.Equals("DobotRailTargetIK"))
		{
			GameObject dobot = GameObject.Find("DobotRail");
			ik = dobot.GetComponent<InverseKinematics>();
		}

	}

	void OnMouseDown()
	{
		mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
		mOffset = gameObject.transform.position - GetMouseWorldPos();
	}

	private Vector3 GetMouseWorldPos()
	{
		Vector3 mousePoint = Input.mousePosition;

		mousePoint.z = mZCoord;

		return Camera.main.ScreenToWorldPoint(mousePoint);
	}

	void OnMouseDrag()
	{
		transform.position = GetMouseWorldPos() + mOffset;
		ik.DoIK();
	}

	private void OnMouseUp()
	{

	}
}
