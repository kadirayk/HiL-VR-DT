using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMovementManager : MonoBehaviour
{

	public GameObject endEffector;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (Vector3.Distance(endEffector.transform.position, transform.position)>0.011)
		{
			transform.position = endEffector.transform.position;
		}
		
    }
}
