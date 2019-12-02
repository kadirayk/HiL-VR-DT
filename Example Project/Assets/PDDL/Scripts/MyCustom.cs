//using UnityEngine;
//using RosSharp.RosBridgeClient;

//// The CameraLister object subscribes to a CompressedImage ROS topic and renders the image onto a 2D texture.
//// We have attached the texture to a plane floating next to the controller.

//namespace RosSharp.RosBridgeClient
//{
//	public class MyCustom : Subscriber<MessageTypes.Sensor.PointCloud2>
//	{


//		public MeshRenderer meshRenderer;

//		private Texture2D texture2D;
//		private byte[] imageData;
//		private bool isMessageReceived;

//		protected override void Start()
//		{
//			base.Start();
//			texture2D = new Texture2D(1, 1);
//			meshRenderer.material = new Material(Shader.Find("Standard"));
//		}
//		private void Update()
//		{
//			if (isMessageReceived)
//				ProcessMessage();
//		}

//		protected override void ReceiveMessage(MessageTypes.Sensor.PointCloud2 pointcloud)
//		{
//			PointCloud pc = new PointCloud(pointcloud);
//			RgbPoint3[] points = pc.Points;
//			foreach (RgbPoint3 point in points)
//			{
//				Debug.Log("x:" + point.x + " y:" + point.y + " z:" + point.z + " c:" + point.rgb);
//			}
//			//imageData = pointcloud.data;
//			//isMessageReceived = true;
//		}

//		private void ProcessMessage()
//		{
//			//texture2D.LoadImage(imageData);
//			//texture2D.Apply();
//			//meshRenderer.material.SetTexture("_MainTex", texture2D);
//			//isMessageReceived = false;
//		}

//	}



//	//// string of which arm to control. Valid values are "left" and "right"
//	//public string arm;

//	////websocket client connected to ROS network
//	//private WebsocketClient wsc;
//	////ROS topic that is streaming RGB video form Baxter's end effector
//	//string topic;
//	////frame rate of the video in Unity
//	//public int framerate = 15;

//	//Renderer rend;
//	//Texture2D texture;

//	//void Start()
//	//{
//	//	//rendering texture for the RGB feed
//	//	rend = GetComponent<Renderer>();
//	//	texture = new Texture2D(2, 2);
//	//	rend.material.mainTexture = texture;

//	//	wsc = GameObject.Find("WebsocketClient").GetComponent<WebsocketClient>();
//	//	topic = "cameras/" + arm + "_hand_camera/image_compressed/compressed";
//	//	topic = "kinect2_jetson/sd/image_color_rect/compressed";
//	//	wsc.Subscribe(topic, "sensor_msgs/CompressedImage", framerate);

//	//	InvokeRepeating("RenderTexture", .5f, 1.0f / framerate);
//	//}

//	//// Converts the CompressedImage message from base64 into a byte array, and loads the array into texture
//	//void RenderTexture()
//	//{
//	//	try
//	//	{
//	//		string message = wsc.messages[topic];
//	//		byte[] image = System.Convert.FromBase64String(message);
//	//		texture.LoadImage(image);
//	//	}
//	//	catch (System.FormatException)
//	//	{
//	//		Debug.Log("Compressed camera image corrupted in transmission");
//	//	}
//	//}

//}