using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflexionManager : MonoBehaviour {


    public Vector3 normal;
	// Use this for initialization
	void Start () {
        if (transform.position.y < 0)
            normal = transform.up;
        if (transform.position.y > 0)
            normal = -transform.up;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + normal * 5);
    }
}
