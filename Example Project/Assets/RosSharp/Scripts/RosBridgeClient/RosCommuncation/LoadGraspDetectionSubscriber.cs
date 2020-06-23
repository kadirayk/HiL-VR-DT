using Assets.PDDL;
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
		private Dictionary<string, MessageTypes.Geometry.Point> graspPositions = new Dictionary<string, MessageTypes.Geometry.Point>();
		Dictionary<string, int> colorCounts = new Dictionary<string, int>();

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

		private string getColorName(Color color) {
			float red = color.r;
			float green = color.g;
			float blue = color.b;
			string colorName = "";
			string name = "";
			if (red - green > 0.12 && red - blue > 0.12)
			{
				colorName = "red";
			}
			else if (blue - red > 0.12 && blue - green > 0.12)
			{
				colorName = "blue";
			} else if (green - red >0.12 && green - blue >0.12) {
				colorName = "green";
			}
			else {
				colorName = "yellow";
			}
			if (colorCounts.ContainsKey(colorName))
			{
				int count = colorCounts[colorName];
				name = "cube_" + colorName + "_" + ++count;
				colorCounts[colorName] = count;
			}
			else {
				name = "cube_" + colorName + "_" + 0;
				colorCounts[colorName] = 0;
			}
			
			return name;
		}

		private void ProcessMessage()
		{
			int i = 0;
			foreach (MessageTypes.Detection.DetectedObject obj in detectedObj.objects)
			{
				//UnityEngine.Vector3 startPoint = TransformExtensions.Ros2Unity(new UnityEngine.Vector3((float)obj.startPoint.x, (float)obj.startPoint.y, (float)obj.startPoint.z));
				//UnityEngine.Vector3 endPoint = TransformExtensions.Ros2Unity(new UnityEngine.Vector3((float)obj.endPoint.x, (float)obj.endPoint.y, (float)obj.endPoint.z));
				MessageTypes.Geometry.Point grasp = obj.graspPoint;
				UnityEngine.Vector3 graspPoint = new UnityEngine.Vector3((float)obj.graspPoint.x, (float)obj.graspPoint.y, (float)obj.graspPoint.z);
				UnityEngine.Quaternion rotation = TransformExtensions.Ros2Unity(new UnityEngine.Quaternion((float)obj.rotation.x, (float)obj.rotation.y, (float)obj.rotation.z, (float)obj.rotation.w));
				Color color = new Color(obj.color[0], obj.color[1], obj.color[2]);


				GameObject table = GameObject.Find("Table");
				//GameObject cube = Instantiate(cubePrefab, new UnityEngine.Vector3(0.847f - graspPoint.z, graspPoint.y + table.GetComponent<Renderer>().bounds.max.y - 0.0125f, 1.18f + graspPoint.x), rotation);
				GameObject cube = Instantiate(cubePrefab, UnityUtil.DobotArmToVR(graspPoint) + new Vector3(0,-0.0125f,0), rotation);
				cube.name = getColorName(color); // "cube_" + i;
				cube.transform.localScale = new UnityEngine.Vector3(0.025f, 0.025f, 0.025f);
				cube.GetComponent<Renderer>().material.color = color;
				graspPositions[cube.name] = grasp;
				i++;

			}
			shouldUpdateVisualization = false;
			isMessageReceived = false;
		}

		public MessageTypes.Geometry.Point getGraspPoint(string name)
		{
			return graspPositions[name];
		}

	}
}