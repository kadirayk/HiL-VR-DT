using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public enum Mode
{
	automated,
	manual
}

public class ModeManager : MonoBehaviour
{

	[Header("Managed Objects")]
	public GameObject sphere;

	List<GameObject> gameObjects = new List<GameObject>();

	private Mode mode = Mode.manual;

	void Start()
	{

	}

	void Update()
	{
		updateForMode();
	}

	private void updateForMode()
	{
		if (this.mode == Mode.manual)
		{
			sphere.SetActive(true);
			foreach (GameObject obj in gameObjects)
			{
				if (obj.GetComponentInChildren<IgnoreHovering>() == null)
				{
					obj.AddComponent<IgnoreHovering>();
				}

			}
		}
		else
		{
			sphere.SetActive(false);
			foreach (GameObject obj in gameObjects)
			{
				if (obj.GetComponentInChildren<IgnoreHovering>() != null)
				{
					Destroy(obj.GetComponentInChildren<IgnoreHovering>());
				}
			}
		}
	}

	public void SetMode(Mode mode)
	{
		this.mode = mode;
	}

	public void Register(GameObject obj)
	{
		gameObjects.Add(obj);
	}

	public void Unregister(GameObject obj)
	{
		gameObjects.Remove(obj);
	}



}
