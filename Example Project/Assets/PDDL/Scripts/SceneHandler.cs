using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class SceneHandler : MonoBehaviour
{
	public SteamVR_LaserPointer laserPointer;

	void Awake()
	{
		Debug.Log("laser pointer awake");
		laserPointer.PointerIn += PointerInside;
		laserPointer.PointerOut += PointerOutside;
		laserPointer.PointerClick += PointerClick;
	}

	public void PointerClick(object sender, PointerEventArgs e)
	{
		if (e.target.name == "Button")
		{
			Debug.Log("Button was clicked");
		}
	}

	public void PointerInside(object sender, PointerEventArgs e)
	{
		if (e.target.name == "Button")
		{
			Button button = e.target.gameObject.GetComponent<Button>();
			button.Select();
		}
	}

	public void PointerOutside(object sender, PointerEventArgs e)
	{
		if (e.target.name == "Button")
		{
		
		}
	}
}