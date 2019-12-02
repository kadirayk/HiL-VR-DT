using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class StateListener : IListener
{
	IList<GameObject> Pushers = new List<GameObject>();

	public void Register(GameObject gameObject)
	{
		Pushers.Add(gameObject);
	}

	public void Unregister(GameObject gameObject)
	{
		Pushers.Remove(gameObject);
	}
}