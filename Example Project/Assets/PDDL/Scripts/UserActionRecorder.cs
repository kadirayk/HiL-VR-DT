using Assets.PDDL.Scripts;
using Assets.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System;
using System.Text;

[Serializable]
public class UserActionRecorder : MonoBehaviour
{
	public static readonly string OBJECT_GRABBED = "objectGrabbed";
	public static readonly string OBJECT_UNGRABBED = "objectUngrabbed";
	public static readonly string OBJECT_USED = "objectUsed";
	public static readonly string OBJECT_UNUSED = "objectUnused";
	public static readonly string HEAD_MOVEMENT = "headMovement";
	private static readonly string WORK_PATH = Configuration.getString("WORK_PATH");
	private static float VRCameraMinimumDetectionSpeed = 0.5f;
	private static float VRCameraMinimumDetectionAngularSpeed = 20f;

	[Header("VR Camera")]
	public Camera VRCamera;

	List<GameObject> grabbables = new List<GameObject>();
	List<GameObject> usables = new List<GameObject>();
	List<GameObject> grabbedObjects = new List<GameObject>();
	List<GameObject> usedObjects = new List<GameObject>();

	[SerializeField]
	List<UserActionEvent> userEvents = new List<UserActionEvent>();

	Vector3 PreviousVRCameraPosition;
	Quaternion PreviousVRCameraRotation;
	float VRCameraSpeed = 0f;
	float VRCameraAngularSpeed = 0f;

	// Start is called before the first frame update
	void Start()
	{
		PreviousVRCameraPosition = VRCamera.transform.position;
		PreviousVRCameraRotation = VRCamera.transform.rotation;
	}

	// Update is called once per frame
	void Update()
	{
		DetectGrabOrUngrab();
		DetectUseOrUnuse();

		DetectHeadMovement();
	}

	private void DetectHeadMovement()
	{
		// Detect For Speed Change
		float movementPerFrame = Vector3.Distance(PreviousVRCameraPosition, VRCamera.transform.position);
		VRCameraSpeed = movementPerFrame / Time.deltaTime;
		if (VRCameraSpeed>VRCameraMinimumDetectionSpeed)
		{
			UserActionEvent userEvent = new UserActionEvent(HEAD_MOVEMENT, "VRCamera", VRCamera.transform.position);
			userEvents.Add(userEvent);
		}
		PreviousVRCameraPosition = VRCamera.transform.position;

		// Detect For Angular Velocity Change
		Quaternion deltaRotation = VRCamera.transform.rotation * Quaternion.Inverse(PreviousVRCameraRotation);
		deltaRotation.ToAngleAxis(out var angle, out var axis);
		angle *= Mathf.Deg2Rad;
		VRCameraAngularSpeed = ((1.0f / Time.deltaTime) * angle * axis).magnitude;
		//Debug.Log("cam angular: " + deltaRotation);
		if (VRCameraAngularSpeed>VRCameraMinimumDetectionAngularSpeed)
		{
			UserActionEvent userEvent = new UserActionEvent(HEAD_MOVEMENT, "VRCamera", VRCamera.transform.position);
			userEvents.Add(userEvent);
		}
		PreviousVRCameraRotation = VRCamera.transform.rotation;

	}

	private void DetectUseOrUnuse()
	{
		foreach (GameObject obj in usables)
		{
			if (!usedObjects.Contains(obj))
			{
				Interactable interactable = obj.GetComponent<Interactable>();
				if (interactable != null && interactable.attachedToHand != null)
				{
					usedObjects.Add(obj);
					LogObjectGrabbed(obj);
				}
			}
			else
			{
				Interactable interactable = obj.GetComponent<Interactable>();
				if (interactable != null && interactable.attachedToHand == null)
				{
					usedObjects.Remove(obj);
					LogObjectUngrabbed(obj);
				}
			}
		}
	}

	private void DetectGrabOrUngrab()
	{
		foreach (GameObject obj in grabbables)
		{
			if (!grabbedObjects.Contains(obj))
			{
				Interactable interactable = obj.GetComponent<Interactable>();
				if (interactable != null && interactable.attachedToHand != null)
				{
					grabbedObjects.Add(obj);
					LogObjectGrabbed(obj);
				}
			}
			else
			{
				Interactable interactable = obj.GetComponent<Interactable>();
				if (interactable != null && interactable.attachedToHand == null)
				{
					grabbedObjects.Remove(obj);
					LogObjectUngrabbed(obj);
				}
			}
		}
	}

	private void LogObjectGrabbed(GameObject obj)
	{
		UserActionEvent userEvent = new UserActionEvent(OBJECT_GRABBED, obj.name, obj.transform.position);
		userEvents.Add(userEvent);

	}

	private void LogObjectUngrabbed(GameObject obj)
	{
		UserActionEvent userEvent = new UserActionEvent(OBJECT_UNGRABBED, obj.name, obj.transform.position);
		userEvents.Add(userEvent);

	}

	private void LogObjectUsed(GameObject obj)
	{
		UserActionEvent userEvent = new UserActionEvent(OBJECT_USED, obj.name, obj.transform.position);
		userEvents.Add(userEvent);
	}

	private void LogObjectUnused(GameObject obj)
	{
		UserActionEvent userEvent = new UserActionEvent(OBJECT_UNUSED, obj.name, obj.transform.position);
		userEvents.Add(userEvent);
	}


	public void RegisterGrabbable(GameObject obj)
	{
		grabbables.Add(obj);
		Debug.Log("grabbable: " + obj.name);
	}

	public void RegisterUsable(GameObject obj)
	{
		usables.Add(obj);
		Debug.Log("usable: " + obj.name);
	}

	public void PrintLog()
	{

		userEvents.Add(new UserActionEvent(OBJECT_GRABBED, "yellow_cube_0", new Vector3()));
		userEvents.Add(new UserActionEvent(OBJECT_UNGRABBED, "yellow_cube_0", new Vector3()));
		userEvents.Add(new UserActionEvent(OBJECT_USED, "manualButton", new Vector3()));
		userEvents.Add(new UserActionEvent(OBJECT_UNUSED, "manualButton", new Vector3()));

		StringBuilder output = new StringBuilder("{\n\t\"events\":[");
		foreach (UserActionEvent e in userEvents){
			output.Append(JsonUtility.ToJson(e, true));
			output.Append(",");
		}
		output.Remove(output.Length - 1, 1);
		output.Append("\n\t]\n}");
		
		System.IO.File.WriteAllText(WORK_PATH + @"UserActionEvents\eventLog.json", output.ToString());
	}
}
