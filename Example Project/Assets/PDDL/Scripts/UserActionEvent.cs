using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.PDDL.Scripts
{
	[Serializable]
	public class UserActionEvent
	{
		public String type;
		public String targetId;
		public Vector3 targetPosition;
		public DateTime time;

		public UserActionEvent(String type, String targetId, Vector3 targetPosition)
		{
			this.type = type;
			this.targetId = targetId;
			this.targetPosition = targetPosition;
			time = DateTime.Now;
		}



	}
}
