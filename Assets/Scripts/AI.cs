using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (GameObject.Find("Ball(Clone)"))
        {
            float PositionY = GameObject.Find("Ball(Clone)").transform.position.y;
            if (PositionY <= 3.5 && PositionY >= -3.5)
            {
                transform.position = new Vector2(transform.position.x, PositionY);
            }
        }
	}
}
