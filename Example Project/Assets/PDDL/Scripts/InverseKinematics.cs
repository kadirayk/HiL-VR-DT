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

	public void moveTarget(string dir)
	{
		if (dir.Equals("left"))
		{
			target.transform.position += new Vector3(-0.01f, 0f, 0f);
		}
		else if (dir.Equals("right"))
		{
			target.transform.position += new Vector3(0.01f, 0f, 0f);
		}
		else if (dir.Equals("up"))
		{
			target.transform.position += new Vector3(0f, 0.01f, 0f);
		}
		else if (dir.Equals("down"))
		{
			target.transform.position += new Vector3(0f, -0.01f, 0f);
		}
		else if (dir.Equals("fw"))
		{
			target.transform.position += new Vector3(0f, 0f, 0.01f);
		}
		else if (dir.Equals("bw"))
		{
			target.transform.position += new Vector3(0f, 0f, -0.01f);
		}
		DoIK();
	}

	// Start is called before the first frame update
	void Start()
	{
		l1 = Vector3.Distance(baseRotator.position, l2_arm.position);
		l2 = Vector3.Distance(l2_arm.position, l3_arm.position);
		l3 = Vector3.Distance(l3_arm.position, hand.position);
		/*GameObject cube = GameObject.Find("RedCube2");
		//float diff = cube.GetComponent<Renderer>().bounds.max.y - cube.GetComponent<Renderer>().bounds.min.y;
		float[] angles = GetAnglesForPosition(new Vector3(cube.GetComponent<Renderer>().bounds.center.x, cube.GetComponent<Renderer>().bounds.max.y - 0.01f, cube.GetComponent<Renderer>().bounds.center.z));
		Debug.Log("--b:" + angles[0] + " l2:" + angles[1] + " l3:" + angles[2]);
		l2_arm.transform.localRotation = Quaternion.Euler(angles[1], 0, 0);
		l3_arm.transform.localRotation = Quaternion.Euler(angles[2], 0, 0);
		hand.transform.localRotation = Quaternion.Euler((angles[1] + angles[2]) * -1, 0, 0);
		baseRotator.transform.localRotation = Quaternion.Euler(0, angles[0], 0);*/

		//target = cube.transform;
		//DoIK();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public float[] GetAnglesForPosition(Vector3 pos)
	{
		float[] angles = new float[3];
		angles[0] = GetAngleForBaseRotator(pos);
		float[] l2_l3Angles = GetAngleForL2AndL3(pos, 0f, 0f);
		l2_l3Angles = GetAngleForL2AndL3(pos, l2_l3Angles[0], l2_l3Angles[1]);
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

	private float gradToRadian(double grad)
	{
		return (float)((grad * Math.PI) / 180);
	}

	private float GetAngleForBaseRotator(Vector3 pos)
	{
		float x_target = pos.x - baseRotator.position.x;
		float z_target = pos.z - baseRotator.position.z;

		// find arctan of x and z
		// result of Atan is in radians; so convert it to degree
		return radianToGrad(Math.Atan(x_target / z_target));

	}

	private float[] GetAngleForL2AndL3(Vector3 pos, float l2_angle_for_correction, float l3_angle_for_correction)
	{
		// find the distance of target in X (left-right) Z (forward-backward) Y (up-down) axises to robot base
		float x_target = pos.x - baseRotator.position.x;
		float y_target = pos.y - baseRotator.position.y;
		float z_target = pos.z - baseRotator.position.z;

		// add offset to target to match end effector position
		//x_target -= endEffectorOffset_z;
		//y_target += endEffectorOffset_y;
		//z_target -= endEffectorOffset_z;
		float handRotation = 0f;
		if (l2_angle_for_correction == 0 && l3_angle_for_correction == 0)
		{
			handRotation = -l2_arm.transform.localRotation.eulerAngles.x - l3_arm.transform.localRotation.eulerAngles.x;
		}
		else
		{
			handRotation = -l2_angle_for_correction - l3_angle_for_correction;
		}

		if (handRotation < -180)
		{
			handRotation = 360 + handRotation;
		}
		float endEffector_z = (float)(endEffectorOffset_z / Math.Cos(gradToRadian(handRotation)));

		if (handRotation < 0)
		{
			float endEffector_y = (float)(endEffector_z * Math.Sin(gradToRadian(handRotation * -1)));
			y_target += endEffectorOffset_y - endEffector_y;
		}
		else
		{
			float endEffector_y = (float)(endEffector_z * Math.Sin(gradToRadian(handRotation)));
			y_target += endEffectorOffset_y + endEffector_y;
		}
		
		l3 = 0.147f + endEffector_z;

		//hypotenuse of z and x
		double hypo = Math.Sqrt(Math.Pow(x_target, 2) + Math.Pow(z_target, 2));

		// find l3Angle
		double e = Math.Sqrt(Math.Pow(y_target, 2) + Math.Pow(hypo, 2));
		float l3Angle = radianToGrad(Math.Acos((Math.Pow(l2, 2) + Math.Pow(l3, 2) - Math.Pow(e, 2)) / (2 * l2 * l3)));

		float c = radianToGrad(Math.Atan(y_target / hypo));
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
			suctionCup.transform.localRotation = Quaternion.Euler(0, -baseRotatorAngle, 0);
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
		// @@@@@%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%@ hypo of z_target and x_target


		// find the distance of target in X (left-right) Z (forward-backward) Y (up-down) axises to robot base
		float x_target = target.position.x - baseRotator.position.x;
		float y_target = target.position.y - baseRotator.position.y;
		float z_target = target.position.z - baseRotator.position.z;

		// add offset to target to match end effector position
		//x_target -= endEffectorOffset_z;
		//y_target += endEffectorOffset_y;
		//z_target -= endEffectorOffset_z;
		float handRotation = -l2_arm.transform.localRotation.eulerAngles.x - l3_arm.transform.localRotation.eulerAngles.x;
		if (handRotation < -180)
		{
			handRotation = 360 + handRotation;
		}
		float endEffector_z = (float)(endEffectorOffset_z / Math.Cos(gradToRadian(handRotation)));

		if (handRotation < 0)
		{
			float endEffector_y = (float)(endEffector_z * Math.Sin(gradToRadian(handRotation * -1)));
			y_target += endEffectorOffset_y - endEffector_y;
		}
		else
		{
			float endEffector_y = (float)(endEffector_z * Math.Sin(gradToRadian(handRotation)));
			y_target += endEffectorOffset_y + endEffector_y;
		}
		
		l3 = 0.147f + endEffector_z;

		//hypotenuse of z and x
		double hypo = Math.Sqrt(Math.Pow(x_target, 2) + Math.Pow(z_target, 2));

		// find l3Angle
		double e = Math.Sqrt(Math.Pow(y_target, 2) + Math.Pow(hypo, 2));
		float l3Angle = radianToGrad(Math.Acos((Math.Pow(l2, 2) + Math.Pow(l3, 2) - Math.Pow(e, 2)) / (2 * l2 * l3)));

		float c = radianToGrad(Math.Atan(y_target / hypo));
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
		// 
		//if (l2Angle >= l2_armMinAngle && l2Angle <= l2_armMaxAngle)
		{
			l2_arm.transform.localRotation = Quaternion.Euler(l2Angle, 0, 0);
		}
		//if (l3Angle >= l3_armMinAngle && l3Angle <= l3_armMaxAngle)
		{
			l3_arm.transform.localRotation = Quaternion.Euler(l3Angle, 0, 0);
		}
		// keep hand always parrallel to ground
		handRotation = -l2_arm.transform.localRotation.eulerAngles.x - l3_arm.transform.localRotation.eulerAngles.x;
		hand.transform.localRotation = Quaternion.Euler(handRotation, 0, 0);
	}

}
