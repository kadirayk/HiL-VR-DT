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
		private MessageTypes.Detection.DetectedObjects detectedObj;
		private bool shouldUpdateVisualization;
		public GameObject cubePrefab;

		protected override void Start()
		{
			base.Start();
		}
		private void Update()
		{
			if (shouldUpdateVisualization && isMessageReceived)
			{
				ProcessMessage();
			}
		}

		public void updateObjectVisualization()
		{
			shouldUpdateVisualization = true;
		}

		protected override void ReceiveMessage(MessageTypes.Detection.DetectedObjects detectedObjects)
		{
			detectedObj = detectedObjects;
			isMessageReceived = true;
		}

		private void ProcessMessage()
		{
			int i = 0;
			foreach (MessageTypes.Detection.DetectedObject obj in detectedObj.objects)
			{
				UnityEngine.Vector3 startPoint = TransformExtensions.Ros2Unity(new UnityEngine.Vector3((float)obj.startPoint.x, (float)obj.startPoint.y, (float)obj.startPoint.z));
				UnityEngine.Vector3 endPoint = TransformExtensions.Ros2Unity(new UnityEngine.Vector3((float)obj.endPoint.x, (float)obj.endPoint.y, (float)obj.endPoint.z));
				UnityEngine.Vector3 graspPoint = TransformExtensions.Ros2Unity(new UnityEngine.Vector3((float)obj.graspPoint.x, (float)obj.graspPoint.y, (float)obj.graspPoint.z));
				UnityEngine.Quaternion rotation = TransformExtensions.Ros2Unity(new UnityEngine.Quaternion((float)obj.rotation.x, (float)obj.rotation.y, (float)obj.rotation.z, (float)obj.rotation.w));
				Color color = new Color(obj.color[0], obj.color[1], obj.color[2]);


				GameObject table = GameObject.Find("Table");
				GameObject cube = Instantiate(cubePrefab, new UnityEngine.Vector3(0.847f - graspPoint.z, graspPoint.y + table.GetComponent<Renderer>().bounds.max.y - 0.0125f, 1.18f + graspPoint.x), rotation);
				cube.name = "cube_" + i;
				cube.transform.localScale = new UnityEngine.Vector3(0.025f, 0.025f, 0.025f);
				cube.GetComponent<Renderer>().material.color = color;
				i++;

			}
			shouldUpdateVisualization = false;
			isMessageReceived = false;
		}

	}
}