using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrid : MonoBehaviour
{

	private float cellSize = 0.1f;
	private GameObject[,,] gameObjectsInGrid = new GameObject[20, 20, 10]; // x,z,y
	public static Vector3[,,] grid = new Vector3[20, 20, 10]; // x,z,y
	public GameObject gridMarker;


	public Vector3 GetNearestPointOnGrid(Vector3 pos)
	{
		pos -= transform.position;
		int xCount = Mathf.RoundToInt(pos.x / cellSize);
		int yCount = Mathf.RoundToInt(pos.y / cellSize);
		int zCount = Mathf.RoundToInt(pos.z / cellSize);

		Vector3 result = new Vector3((float)xCount * cellSize, (float)yCount * cellSize, (float)zCount * cellSize);
		result += transform.position;
		return result;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		for (float x = 0; x < 2.1; x += cellSize)
		{
			for (float z = 0; z < 2.1; z += cellSize)
			{
				//for(float y=0; y<0.2; y += cellSize)
				//{
				var point = GetNearestPointOnGrid(new Vector3(x, 0f, z));
				Gizmos.DrawSphere(point, 0.01f);
				//}
			}
		}
	}

	public void AddMarker(Vector3 pos, string label)
	{
		gridMarker.GetComponentInChildren<TextMesh>().text = label;
		Instantiate(gridMarker, pos, Quaternion.identity);
	}

	void Awake()
	{
		for (int x = 0; x < 20; x++)
		{
			for (int z = 0; z < 20; z++)
			{
				for (int y = 0; y < 10; y++)
				{
					grid[x, z, y] = new Vector3(x * 0.1f, y * 0.1f + 0.01f, z * 0.1f);
				}
			}
		}
	}

	// Start is called before the first frame update
	void Start()
	{

		//AddMarker(grid[9, 8, 0], "A");
		//AddMarker(grid[10, 8, 0], "B");
		//AddMarker(grid[11, 8, 0], "C");
	}

	// Update is called once per frame
	void Update()
	{

	}
}
