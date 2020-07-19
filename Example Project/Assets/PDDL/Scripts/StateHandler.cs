using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This script is attached to Cube prefab, each cube registers itself during its instantiation 
 */
public class StateHandler : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		WorldState ws = GameObject.FindObjectOfType<WorldState>();
		ws.Register(gameObject);
		//UserActionRecorder actionRecorder = GameObject.FindObjectOfType<UserActionRecorder>();
		//actionRecorder.RegisterGrabbable(gameObject);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
