using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient.MessageTypes.Detection;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp;
using SimpleJSON;

public class ObjectVisualizer : MonoBehaviour
{
	DetectedObjects detectedObjects;
	static readonly string demoFile = @"grasp_detection.json";
	public GameObject cubePrefab;

	// Start is called before the first frame update
	void Start()
	{
		string json = System.IO.File.ReadAllText(demoFile);
		JSONNode data = JSON.Parse(json);

		int i = 0;
		DetectedObject[] objs = new DetectedObject[10];
		foreach (JSONNode detectedObject in data["objects"])
		{
			DetectedObject obj = new DetectedObject();
			Point graspPoint = new Point();
			graspPoint.x = detectedObject["graspPoint"]["x"];
			graspPoint.y = detectedObject["graspPoint"]["y"];
			graspPoint.z = detectedObject["graspPoint"]["z"];
			obj.graspPoint = graspPoint;

			Point startPoint = new Point();
			startPoint.x = detectedObject["startPoint"]["x"];
			startPoint.y = detectedObject["startPoint"]["y"];
			startPoint.z = detectedObject["startPoint"]["z"];
			obj.startPoint = startPoint;

			Point endPoint = new Point();
			endPoint.x = detectedObject["endPoint"]["x"];
			endPoint.y = detectedObject["endPoint"]["y"];
			endPoint.z = detectedObject["endPoint"]["z"];
			obj.endPoint = endPoint;


			RosSharp.RosBridgeClient.MessageTypes.Geometry.Quaternion rotation = new RosSharp.RosBridgeClient.MessageTypes.Geometry.Quaternion();
			rotation.x = detectedObject["rotation"]["x"];
			rotation.y = detectedObject["rotation"]["y"];
			rotation.z = detectedObject["rotation"]["z"];
			rotation.w = detectedObject["rotation"]["w"];
			obj.rotation = rotation;

			float[] color = new float[3];
			color[0] = detectedObject["color"][0];
			color[1] = detectedObject["color"][1];
			color[2] = detectedObject["color"][2];
			obj.color = color;
			objs[i] = obj;
			i++;
		}
		detectedObjects = new DetectedObjects(objs, "1");
		i = 0;
		foreach (DetectedObject obj in detectedObjects.objects)
		{
			UnityEngine.Vector3 startPoint = TransformExtensions.Ros2Unity(new UnityEngine.Vector3((float)obj.startPoint.x, (float)obj.startPoint.y, (float)obj.startPoint.z));
			UnityEngine.Vector3 endPoint = TransformExtensions.Ros2Unity(new UnityEngine.Vector3((float)obj.endPoint.x, (float)obj.endPoint.y, (float)obj.endPoint.z));
			UnityEngine.Vector3 graspPoint = TransformExtensions.Ros2Unity(new UnityEngine.Vector3((float)obj.graspPoint.x, (float)obj.graspPoint.y, (float)obj.graspPoint.z));
			UnityEngine.Quaternion rotation = TransformExtensions.Ros2Unity(new UnityEngine.Quaternion((float)obj.rotation.x, (float)obj.rotation.y, (float)obj.rotation.z, (float)obj.rotation.w));
			Color color = new Color(obj.color[0], obj.color[1], obj.color[2]);


			GameObject table = GameObject.Find("Table");
			GameObject cube = Instantiate(cubePrefab, new UnityEngine.Vector3(0.9f - graspPoint.z, graspPoint.y + table.GetComponent<Renderer>().bounds.max.y - 0.0125f, 1.2f + graspPoint.x), UnityEngine.Quaternion.identity);
			cube.name = "cube_" + i;
			cube.transform.localScale = new UnityEngine.Vector3(0.025f, 0.025f, 0.025f);
			cube.GetComponent<Renderer>().material.color = color;
			i++;
		}

	}

	// Update is called once per frame
	void Update()
	{

	}
}
