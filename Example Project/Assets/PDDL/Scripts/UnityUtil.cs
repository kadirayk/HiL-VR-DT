using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.PDDL
{
	class UnityUtil
	{
		public static string PositionToString(Vector3 pos)
		{
			return "x:" + pos.x + " y:" + pos.y + " z:" + pos.z;
		}

		public static Vector3 DobotArmToVR(Vector3 vector3)
		{
			Vector3 scaled = vector3 / 1000;
			double angle = Math.Atan(scaled.x / scaled.y);
			return new Vector3((1 - (0.06f * (float)Math.Cos(angle) + scaled.y)), scaled.z + 0.840f, (1f + 0.06f * (float)Math.Sin(angle) + scaled.x));
		}

		public static Vector3 VRToDobotArm(Vector3 vector3)
		{
			Vector3 scaled = vector3 * 1000;
			scaled.x = 1000 - scaled.x;
			scaled.z = scaled.z - 1000;
			double angle = Math.Atan(scaled.z / scaled.x);
			//new Vector3(vector3.z, -vector3.x, vector3.y);
			Vector3 result;
			if (angle < 0)
			{
				result = new Vector3(scaled.z - 60 * (float)Math.Sin(angle*-1), scaled.x - 60 * (float)Math.Cos(angle), scaled.y - 840);
			}
			else {
				result = new Vector3(scaled.z - 60 * (float)Math.Sin(angle), scaled.x - 60 * (float)Math.Cos(angle), scaled.y - 840);
			}
			result.x = (int)Math.Round(result.x, 0);
			result.y = (int)Math.Round(result.y, 0);
			result.z = (int)Math.Round(result.z, 0);
			return result;
			//return new Vector3((1 - (0.06f * (float)Math.Cos(angle) + scaled.y)), scaled.z + 0.840f, (1f + 0.06f * (float)Math.Sin(angle) + scaled.x));
		}

	}
}
