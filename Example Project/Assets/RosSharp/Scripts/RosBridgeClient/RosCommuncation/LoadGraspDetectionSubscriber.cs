using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RosSharp.RosBridgeClient
{
	[RequireComponent(typeof(RosConnector))]
	public class LoadGraspDetectionSubscriber : Subscriber<MessageTypes.Detection.DetectedObjects>
	{
		private bool isMessageReceived;
		private int numberOfObjects;

		protected override void Start()
		{
			base.Start();
		}
		private void Update()
		{
			if (isMessageReceived)
				ProcessMessage();
		}

		protected override void ReceiveMessage(MessageTypes.Detection.DetectedObjects detectedObjects)
		{
			numberOfObjects = detectedObjects.objects.Length;
			isMessageReceived = true;
		}

		private void ProcessMessage()
		{
			Debug.Log(numberOfObjects);
			isMessageReceived = false;
		}

	}
}