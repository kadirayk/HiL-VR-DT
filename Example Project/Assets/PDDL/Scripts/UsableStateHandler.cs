using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableStateHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		UserActionRecorder actionRecorder = GameObject.FindObjectOfType<UserActionRecorder>();
		actionRecorder.RegisterUsable(gameObject);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
