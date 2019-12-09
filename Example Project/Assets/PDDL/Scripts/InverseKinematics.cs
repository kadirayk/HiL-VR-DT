using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InverseKinematics : MonoBehaviour
{
	private float baseRotatorMinAngle = -90;
	private float baseRotatorMaxAngle = 90;
	private float l2_armMinAngle = 0;
	private float l2_armMaxAngle = 85;
	private float l3_armMinAngle = -10;
	private float l3_armMaxAngle = 95;
	public Transform baseRotator;
	public Transform l2_arm;
	public Transform l3_arm;
	public Transform hand;
	public Transform suctionCup;
	public Transform endEffector;
	public Transform target;
	private float l1;
	private float l2;
	private float l3;
	private float endEffectorOffset_z = 0.06f; // end effector is 0.006 away from the end of l3
	private float endEffectorOffset_y = 0.06f; // end effector is 0.006 lower from the end of l3

	// Start is called before the first frame update
	void Start()
	{
		l1 = Vector3.Distance(baseRotator.position, l2_arm.position);
		l2 = Vector3.Distance(l2_arm.position, l3_arm.position);
		l3 = Vector3.Distance(l3_arm.position, hand.position);

	}

	// Update is called once per frame
	void Update()
	{

	}

	public float[] GetAnglesForPosition(Vector3 pos) {
		float[] angles = new float[3];
		angles[0] = GetAngleForBaseRotator(pos);
		float[] l2_l3Angles = GetAngleForL2AndL3(pos);
		angles[1] = l2_l3Angles[0];
		angles[2] = l2_l3Angles[1];
		return angles;
	}

	public void DoIK()
	{
		HandleBaseRotator();
		HandleL2AndL3();
	}

	private float radianToGrad(double radian)
	{
		// 1rad * 180/pi
		return (float)(radian * (180 / Math.PI));
	}

	private float GetAngleForBaseRotator(Vector3 pos) {
		float x_target = pos.x - baseRotator.position.x;
		float z_target = pos.z - baseRotator.position.z;
		// find arctan of x and z
		// result of Atan is in radians; so convert it to degree
		return radianToGrad(Math.Atan(x_target / z_target));

	}

	private float[] GetAngleForL2AndL3(Vector3 pos)
	{
		// find the distance of target in X (left-right) Z (forward-backward) Y (up-down) axises to robot base
		float x_target = pos.x - baseRotator.position.x;
		float y_target = pos.y - baseRotator.position.y;
		float z_target = pos.z - baseRotator.position.z;

		// add offset to target to match end effector position
		y_target += endEffectorOffset_y;
		z_target -= endEffectorOffset_z;

		// find l3Angle
		double e = Math.Sqrt(Math.Pow(y_target, 2) + Math.Pow(z_target, 2));
		float l3Angle = radianToGrad(Math.Acos((Math.Pow(l2, 2) + Math.Pow(l3, 2) - Math.Pow(e, 2)) / (2 * l2 * l3)));

		float c = radianToGrad(Math.Atan(y_target / z_target));
		float b = radianToGrad(Math.Acos((Math.Pow(l2, 2) + Math.Pow(e, 2) - Math.Pow(l3, 2)) / (2 * l2 * e)));
		float l2Angle = c + b;

		// convert absolute angles to relative angles
		if (l2Angle < 0)
		{
			l2Angle = 90 + l2Angle;
		}
		else
		{
			l2Angle = 90 - l2Angle;
		}
		l3Angle = 90 - l3Angle;
		// apply rotations
		return new float[] { l2Angle, l3Angle };
	}




	private void HandleBaseRotator()
	{
		// find the distance of target in X (left-right) and Z(forward-backward) axises to robot base
		float x_target = target.position.x - baseRotator.position.x;
		float z_target = target.position.z - baseRotator.position.z;
		// find arctan of x and z
		// result of Atan is in radians; so convert it to degree
		float baseRotatorAngle = radianToGrad(Math.Atan(x_target / z_target));
		// change localRotation of the baseRotator
		if (baseRotatorAngle >= baseRotatorMinAngle && baseRotatorAngle <= baseRotatorMaxAngle)
		{
			baseRotator.transform.localRotation = Quaternion.Euler(0, baseRotatorAngle, 0);
		}
	}

	private void HandleL2AndL3()
	{
		//                       @@#,                                                     
		//                     *@    .#@@@%,                                              
		//                    .@. l3Angle   *&@@@*
		// 					 .@                    *&@@@*  l3
		//                   @                           .%@@@%.
		//                  @.                                  .#@@@%,                   
		//                 @*											*@@@&,             
		//                @*                                                  ,&@@@*
		//               @.                                                         ,%@@@ y_target 
		//          l2  @.                                                        ,@@@# @ 
		//            .@.                                                   ,@@@#       @ 
		//            @*											   *@@@#            @ 
		//           @                                           .&@@%.                 @ 
		//          @.                                      *&@@#                       @ 
		//        .@.                                 ,&@@%                             @ 
		//        @*                             ,@@@#                                  @ 
		//       @.                        ,&@@#.  e                                    @ 
		//      @*					   *@@@#                                            @ 
		//     @*  l2Angle=b+c  .&@@%.                                                  @ 
		//    @#           ,&@@#                                                        @ 
		//   @.  b   ,&@@%                                                              @ 
		//  @*  ,&@@   c                                                                @ 
		// @@@@@%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%@ z_target


		// find the distance of target in X (left-right) Z (forward-backward) Y (up-down) axises to robot base
		float x_target = target.position.x - baseRotator.position.x;
		float y_target = target.position.y - baseRotator.position.y;
		float z_target = target.position.z - baseRotator.position.z;

		// add offset to target to match end effector position
		y_target += endEffectorOffset_y;
		z_target -= endEffectorOffset_z;

		// find l3Angle
		double e = Math.Sqrt(Math.Pow(y_target, 2) + Math.Pow(z_target, 2));
		float l3Angle = radianToGrad(Math.Acos((Math.Pow(l2, 2) + Math.Pow(l3, 2) - Math.Pow(e, 2)) / (2 * l2 * l3)));

		float c = radianToGrad(Math.Atan(y_target / z_target));
		float b = radianToGrad(Math.Acos((Math.Pow(l2, 2) + Math.Pow(e, 2) - Math.Pow(l3, 2)) / (2 * l2 * e)));
		float l2Angle = c + b;

		// convert absolute angles to relative angles
		if (l2Angle < 0)
		{
			l2Angle = 90 + l2Angle;
		}
		else
		{
			l2Angle = 90 - l2Angle;
		}
		l3Angle = 90 - l3Angle;
		// apply rotations
		float handRotation = 0f;
		if (l2Angle >= l2_armMinAngle && l2Angle <= l2_armMaxAngle)
		{
			l2_arm.transform.localRotation = Quaternion.Euler(l2Angle, 0, 0);
		}
		if (l3Angle >= l3_armMinAngle && l3Angle <= l3_armMaxAngle)
		{
			l3_arm.transform.localRotation = Quaternion.Euler(l3Angle, 0, 0);
		}
		// keep hand always parrallel to ground
		handRotation = -l2_arm.transform.localRotation.eulerAngles.x - l3_arm.transform.localRotation.eulerAngles.x;
		hand.transform.localRotation = Quaternion.Euler(handRotation, 0, 0);
	}

}
