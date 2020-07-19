using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class GizmoManager : MonoBehaviour
{

	void OnDrawGizmos()
	{
#if UNITY_EDITOR
		GameObject[] target = GameObject.FindGameObjectsWithTag("PDDLObject");
		if (target != null)
		{
			for (int i = 0; i < target.Length; i++)
			{
				Handles.Label(target[i].transform.position, target[i].name);
			}
		}
#endif
	}
}
