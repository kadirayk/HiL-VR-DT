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

    // Start is called before the first frame update
    void Start()
    {
		l1 = Vector3.Distance(l1_arm.position, l2_arm.position);
		l2 = Vector3.Distance(l2_arm.position, l3_arm.position);
		l3 = Vector3.Distance(l3_arm.position, hand.position);
		Debug.Log("l1=" + l1 + " l2=" + l2 + " l3=" + l3);
		x_target = target.position.x - l2_arm.position.x;
		y_target = target.position.y - l2_arm.position.y;
		z_target = target.position.z - l2_arm.position.z;
		Debug.Log("x:" + x_target + " y:" + y_target + " z:" + z_target);
		Debug.Log("fi:" + Math.Atan(x_target/z_target)* (180 / Math.PI));
    }

    // Update is called once per frame
    void Update()
    {
		double baseRotatorAngle = Math.Atan(x_target / z_target) * (180 / Math.PI);
    }
}
