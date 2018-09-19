using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BallManager : MonoBehaviour {
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            Vector2 direction = transform.position - collision.gameObject.transform.position;
            direction.Normalize();
            if (false)//collision.gameObject.GetComponent<PlayerController>().GetPowerBall())
            {
                transform.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x, direction.y / 2) * 60 * 2;
            }
            else
            {
                transform.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x, direction.y / 2) * 60;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            Camera.main.GetComponent<CameraController>().ShakeCamera(0.3f, 0.3f);
            if (collision.gameObject.tag == "Player1")
            {
                GameObject.Find("GameCore").GetComponent<GameCore>().Goal(GameCore.Player.PLAYER_2);
            }
            if (collision.gameObject.tag == "Player2")
            {
                GameObject.Find("GameCore").GetComponent<GameCore>().Goal(GameCore.Player.PLAYER_1);
            }
            Destroy(gameObject);
            //GameCore
        }
    }

    public void LaunchBall()
    {
        float value = Random.Range(0,50);
        if(Random.value > 25)
        {
            value = -1;
        }

        if(Random.value <= 25)
        {
            value = 1;
        }
        Vector2 direction = new Vector2(value, 0);
        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x, 0) * 60;
    }
}
