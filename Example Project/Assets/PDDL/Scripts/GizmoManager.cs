using System.Collections;
using UnityEditor;
using UnityEngine;

public class GizmoManager : MonoBehaviour
{

	void OnDrawGizmos()
	{
		GameObject[] target = GameObject.FindGameObjectsWithTag("PDDLObject");
		if (target != null)
		{
			for (int i = 0; i < target.Length; i++)
			{
				Handles.Label(target[i].transform.position, target[i].name);
			}
		}
	}
}
