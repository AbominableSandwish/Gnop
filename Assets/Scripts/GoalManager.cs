using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoalManager : NetworkBehaviour {

    private Color color_goal = Color.gray;
    private float Time_goal;
	
	// Update is called once per frame
    [Server]
	void Update () {
		if(Time_goal > 0.0f)
        {
            if(Time_goal == 0.5f)
                RpcChangeColorGoal(Color.red);

            Time_goal -= Time.deltaTime;
        }
        else
        {
            Time_goal = 0.0f;

            if(GetComponent<SpriteRenderer>().color != color_goal)
            RpcChangeColorGoal(color_goal);
        }
	}

    [Server]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            Time_goal = 0.5f;
        }
    }

    [ClientRpc]
    public void RpcChangeColorGoal(Color color)
    {
        color_goal = color;
        GetComponent<SpriteRenderer>().color = color_goal;
    }
}
