using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		WorldState ws = GameObject.FindObjectOfType<WorldState>();
		ws.Register(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
