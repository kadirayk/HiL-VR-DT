using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InverseKinematics : MonoBehaviour
{
	public Transform l1_arm;
	public Transform l2_arm;
	public Transform l3_arm;
	public Transform hand;
	public Transform target;
	private float l1;
	private float l2;
	private float l3;
	private float x_target;
	private float y_target;
	private float z_target;


	//private Vector3 mOffset;
	//private float mZCoord;

	//void OnMouseDown()
	//{
	//	mZCoord = Camera.main.WorldToScreenPoint(target.transform.position).z;
	//	mOffset = target.transform.position - GetMouseWorldPos();
	//}

	//private Vector3 GetMouseWorldPos()
	//{
	//	Vector3 mousePoint = Input.mousePosition;

	//	mousePoint.z = mZCoord;

	//	return Camera.main.ScreenToWorldPoint(mousePoint);
	//}

	//void OnMouseDrag()
	//{
	//	target.transform.position = GetMouseWorldPos() + mOffset;
	//}

	public void DoIK()
	{
		x_target = target.position.x - l1_arm.position.x;
		y_target = target.position.y - l1_arm.position.y;
		z_target = target.position.z - l1_arm.position.z;

		double baseRotatorAngle = Math.Atan(x_target / z_target) * (180 / Math.PI);
		//l1_arm.Rotate(new Vector3(0, (float)baseRotatorAngle,0));
		l1_arm.transform.localRotation = Quaternion.Euler(0, (float)baseRotatorAngle, 0);
		Debug.Log("base:" + baseRotatorAngle);

		double e = Math.Sqrt(Math.Pow(y_target, 2) + Math.Pow(z_target, 2));
		double l3Angle = Math.Acos((Math.Pow(l2, 2) + Math.Pow(l3, 2) - Math.Pow(e, 2)) / (2 * l2 * l3)) * (180 / Math.PI);
		double c = Math.Atan(y_target / z_target) * (180 / Math.PI);
		double b = Math.Acos((Math.Pow(l2, 2) + Math.Pow(e, 2) - Math.Pow(l3, 2)) / (2 * l2 * e)) * (180 / Math.PI);
		double l2Angle = c + b;
		if (l2Angle < 0)
		{
			l2Angle = 90 + l2Angle;
		}
		else
		{
			l2Angle = 90 - l2Angle;
		}
		l3Angle = 90 - l3Angle;
		//l2_arm.Rotate(new Vector3((float)l2Angle, 0, 0));
		//l3_arm.Rotate(new Vector3((float)l3Angle, 0, 0));
		l2_arm.transform.localRotation = Quaternion.Euler((float)l2Angle, 0, 0);
		l3_arm.transform.localRotation = Quaternion.Euler((float)l3Angle, 0, 0);
		Debug.Log("l2:" + l2Angle + " l3:" + l3Angle);
	}


	// Start is called before the first frame update
	void Start()
	{
		l1 = Vector3.Distance(l1_arm.position, l2_arm.position);
		l2 = Vector3.Distance(l2_arm.position, l3_arm.position);
		l3 = Vector3.Distance(l3_arm.position, hand.position);
		//Debug.Log("l1=" + l1 + " l2=" + l2 + " l3=" + l3);
		//Debug.Log("x:" + x_target + " y:" + y_target + " z:" + z_target);
		//Debug.Log("fi:" + Math.Atan(x_target / z_target) * (180 / Math.PI));

		//double e = Math.Sqrt(Math.Pow(y_target,2) + Math.Pow(z_target,2));
		//double beta = Math.Acos((Math.Pow(l2,2) + Math.Pow(l3,2) - Math.Pow(e,2))/(2*l2*l3))*(180 / Math.PI);
		//double c = Math.Atan(y_target / z_target)* (180 / Math.PI);
		//double b = Math.Acos((Math.Pow(l2,2) + Math.Pow(e,2) - Math.Pow(l3,2)) / (2*l2*e))* (180 / Math.PI);
		//double alpha = c + b;
		////double q2 = Math.Acos((Math.Pow(z_target, 2) + Math.Pow(y_target, 2) - Math.Pow(l2, 2) - Math.Pow(l3, 2)) / (2*l2*l3));
		////q2 = q2 * (180 / Math.PI);
		////double q1 = Math.Atan(y_target / z_target) + Math.Atan((l3*Math.Sin(q2))/(l2+l3*Math.Cos(q2)));


		////q1 = q1 * (180 / Math.PI);
		////q1 = 90 - q1;
		////q2 = 90 - q2;
		////beta = 90 - beta;
		////alpha = 90 + alpha;
		//if (alpha < 0) {
		//	alpha = 90 + alpha;
		//} else {
		//	alpha = 90 - alpha;
		//}
		//beta = 90 - beta;

		//Debug.Log("alpha:" + alpha + " beta:" + beta);
		//l3_arm.Rotate(new Vector3((float)q2, 0, 0));
		//l2_arm.Rotate(new Vector3((float)q1, 0, 0));
		//double q2 = Math.Acos((Math.Pow(z_target, 2) + Math.Pow(y_target, 2) - Math.Pow(l2, 2) - Math.Pow(l3, 2)) / (2 * l2 * l3));
		//q2 = q2 * (180 / Math.PI);
		//double q1 = Math.Atan(y_target / z_target) + Math.Atan((l3 * Math.Sin(q2)) / (l2 + l3 * Math.Cos(q2)));
		//q1 = q1 * (180 / Math.PI);
		//Debug.Log("q2:" + q2);
		//Debug.Log("q1:" + q1);
		//double c2 = Math.Pow(x_target, 2) + Math.Pow(z_target, 2);
		//double e2 = c2 + Math.Pow((y_target - l1), 2); // l1 might be l2
		//double sigma = Math.Acos((e2-Math.Pow(l2,2)-Math.Pow(l3,2))/(2*l2*l3));
		//double alpha = Math.Atan((y_target - l1) / (Math.Sqrt(c2)));
		//double beta = Math.Acos((Math.Pow(l2,2)+e2-Math.Pow(l3,2)) / (2*l2*Math.Sqrt(e2)));
		//Debug.Log("s:" + sigma* (180 / Math.PI) + " a:" + (alpha+beta)* (180 / Math.PI));

	}

	// Update is called once per frame
	void Update()
	{

	}
}
