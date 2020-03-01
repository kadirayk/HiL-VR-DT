using RosSharp.RosBridgeClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosSharp.RosBridgeClient
{
	class TFSubscriber : Subscriber<MessageTypes.Geometry.PoseStamped>
	{
		protected override void ReceiveMessage(MessageTypes.Geometry.PoseStamped message)
		{
			throw new NotImplementedException();
		}
	}
}
