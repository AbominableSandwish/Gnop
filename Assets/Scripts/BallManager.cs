using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BallManager : NetworkBehaviour {

    private uint LastPlayerTouch;
    private float Velocity = 35;
    private float BoostVelocity = 1;
    private float BoostAction = 1;
    private float Damage = 1;
    public ParticleSystem particules;
    
    [Server]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            if (BoostVelocity < 1.5f)
                BoostVelocity += 0.025f;

            //RpcBound(Vector2.Lerp(transform.position, collision.transform.position, 0.6f));

            if (!collision.gameObject.GetComponent<StaffManager>().GetDefense())
            {

               RpcChangeColorSystemParticule(Color.white);
               collision.gameObject.GetComponent<StaffManager>().RpcTakeDamage(BoostAction);
               BoostAction = 1;
            }
            else
            {
                Debug.Log(collision.gameObject.GetComponent<StaffManager>().GetCounterTime());
                if (collision.gameObject.GetComponent<StaffManager>().GetCounterTime() < 0.1f)
                {
                    Debug.Log("Perfect!");
                    GameObject.Find("GameCore").GetComponent<UIManager>().RpcSpawnPerfectAction(collision.gameObject.GetComponent<StaffManager>().GetIdPlayer());
                    BoostAction += 0.25f;
                    if(BoostAction == 1.25f)
                        RpcChangeColorSystemParticule(Color.green);
                    if (BoostAction == 1.50f)
                        RpcChangeColorSystemParticule(Color.yellow);
                    if (BoostAction == 1.75f)
                        RpcChangeColorSystemParticule(Color.red);
                }
                else
                {
                    if (collision.gameObject.GetComponent<StaffManager>().GetCounterTime() < 0.3f)
                    {
                        Debug.Log("Good");
                        GameObject.Find("GameCore").GetComponent<UIManager>().RpcSpawnGoodAction(collision.gameObject.GetComponent<StaffManager>().GetIdPlayer());
                        RpcChangeColorSystemParticule(Color.blue);
                    }
                }
            }

            Vector2 direction = transform.position - collision.gameObject.transform.position;
            direction.Normalize();
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x, direction.y / 10) * (Velocity * BoostVelocity * BoostAction);
            LastPlayerTouch = collision.gameObject.GetComponent<NetworkIdentity>().netId.Value;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
           // RpcBound();
        }
    }

    [ClientRpc]
    void RpcChangeColorSystemParticule(Color color) {
        ParticleSystem.MainModule ma = particules.main;
        ma.startColor = color;
    }

    [ClientRpc]
    void RpcBound(Vector3 _position)
    {
        transform.position = _position;
    }

    [Server]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            Camera.main.GetComponent<CameraController>().ShakeCamera(0.3f, 0.3f);

            if (collision.gameObject.tag == "Goal1")
            {
                GameObject.Find("Player2").GetComponent<ScoreManager>().Goal();
            }
            if (collision.gameObject.tag == "Goal2")
            {
                GameObject.Find("Player1").GetComponent<ScoreManager>().Goal();
            }
            GameObject.Find("GameCore").GetComponent<GameCore>().NextMatch();
            Destroy(gameObject);
        }


       
    }

    [Server]
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
        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x, 0) * Velocity;
    }

    private void Update()
    {
    }
}
