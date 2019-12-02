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

	}
}
