//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class State : MonoBehaviour
//{
//	private static readonly int CUBE_COUNT = 9;

//	private static int[,,] GoalState = new int[CUBE_COUNT, CUBE_COUNT, CUBE_COUNT];

//	private List<Vector3> PositionList = new List<Vector3>();

//	// Start is called before the first frame update
//	void Start()
//    {
//		for (int i = 1; i <= CUBE_COUNT; i++) {
//			PositionList.Add(GameObject.Find("Cube"+i).transform.position);
//		}
			   		
//	}

//    // Update is called once per frame
//    void Update()
//    {
//		for (int i = 1; i <= CUBE_COUNT; i++)
//		{
//			Vector3 pos = GameObject.Find("Cube" + i).transform.position;
//			if (pos.x < 0.33 && pos.z < 0.33 && pos.y < 0.33) {
//				GoalState[0, 0, 0] = 1;
//			}
//			if (pos.x> 0.33 && pos.x < 0.66 && pos.z < 0.33 && pos.y < 0.33)
//			{
//				GoalState[0, 0, 0] = 1;
//			}
//		}
//	}
//}
