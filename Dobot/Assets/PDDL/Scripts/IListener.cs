using UnityEngine;
using UnityEditor;

public interface IListener
{
	void Register(GameObject gameObject);
	void Unregister(GameObject gameObject);

}