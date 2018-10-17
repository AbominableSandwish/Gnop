using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoalManager : NetworkBehaviour
{

    private const float TIME_GOAL = 0.5f;
    private Color colorGoal = Color.gray;
    private float timeGoal;
	
	// Update is called once per frame
	void Update () {
		if(timeGoal > 0.0f)
        {
            if(timeGoal == TIME_GOAL)
                ChangeColorGoal(Color.red);

            timeGoal -= Time.deltaTime;
        }
        else
        {
            timeGoal = 0.0f;

            if(GetComponent<SpriteRenderer>().color != colorGoal)
                ChangeColorGoal(colorGoal);
        }
	}

    [Server]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            timeGoal = TIME_GOAL;
        }
    }

    public void ChangeColorGoal(Color color)
    {
        colorGoal = color;
        GetComponent<SpriteRenderer>().color = colorGoal;
    }
}
