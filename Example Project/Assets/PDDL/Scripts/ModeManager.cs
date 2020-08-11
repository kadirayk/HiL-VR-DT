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

	Dictionary<GameObject, OVRGrabbable> gameObjToGrabbable = new Dictionary<GameObject, OVRGrabbable>();

	private bool modeUpdated = false;

	void Start()
	{

	}

	void Update()
	{
		if (!modeUpdated)
		{
			updateForMode();
		}

		if (this.mode == Mode.manual)
		{
			foreach (GameObject obj in gameObjects)
			{
				Destroy(obj.GetComponent<OVRGrabbable>());

			}
		}
		else
		{
			foreach (GameObject obj in gameObjects)
			{
				obj.AddComponent<OVRGrabbable>();
			}
		}
	}



	private void updateForMode()
	{
		if (this.mode == Mode.manual)
		{
			sphere.SetActive(true);
			foreach (GameObject obj in gameObjects)
			{
				Destroy(obj.GetComponent<OVRGrabbable>());

			}
		}
		else
		{
			sphere.SetActive(false);
			foreach (GameObject obj in gameObjects)
			{
				obj.AddComponent<OVRGrabbable>();
			}
		}
		modeUpdated = true;
	}

	public void SetMode(Mode mode)
	{
		this.mode = mode;
		modeUpdated = false;
	}
	public Mode GetMode()
	{
		return this.mode;
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
