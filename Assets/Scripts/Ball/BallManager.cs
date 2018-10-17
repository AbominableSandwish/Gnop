using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BallManager : NetworkBehaviour {

    //Variables
    private const short SCORE_WIN = 7;
    private const float SHAKE_POWER = 0.3f;
    private const float SHAKE_DURATION = 0.3f;

    private const float TIME_FOR_PERFECT_ACTION = 0.15f;
    private const float TIME_FOR_GOOD_ACTION = 0.3f;

    private UIManager uiManager;
    private GameCore gameCore;
    private uint lastPlayerTouch;

    [SyncVar(hook = ("OnChangeVelocity"))]
    public float velocity = 35;
    [SyncVar(hook =("OnChangeBoostVelocity"))]
    public float boostVelocity = 1;
    [SyncVar(hook = ("OnChangeBoostAction"))]
    public float boostAction = 1;

    public ParticleSystem particules;

    Vector2 direction;

    float counterTime;

    [Server]
    private void FixedUpdate()
    {
        if(counterTime > 0)
        counterTime -= Time.deltaTime;
    }

    [ClientRpc]
    void RpcBound(Vector3 _position)
    {
        transform.position = _position;
    }

    [Server]
    void Start()
    {
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();
        uiManager = GameObject.Find("GameCore").GetComponent<UIManager>();
    }

    [Server]
    private void OnTriggerEnter2D(Collider2D collision)
    {

        //If touch a player
        if (collision.gameObject.layer == LayerMask.NameToLayer("Players"))
        {
            if (boostVelocity < 1.5f)
                boostVelocity += 0.025f;

            if (!collision.gameObject.GetComponent<StaffBehaviour>().GetDefense())
            {
                //Effect Damage
                RpcChangeColorSystemParticule(Color.white);
                collision.gameObject.GetComponent<StaffBehaviour>().RpcTakeDamage(boostAction);
                boostAction = 1;
            }
            else
            {
                //Effect for the shield
                if (collision.gameObject.GetComponent<StaffBehaviour>().GetCounterTime() < TIME_FOR_PERFECT_ACTION)
                {
                    //Effect Perfect
                    Debug.Log("Perfect!");
                    uiManager.RpcSpawnPerfectAction(collision.gameObject.GetComponent<StaffBehaviour>().GetIdPlayer());
                    //Grow a damage and change color
                    boostAction = boostAction + 0.25f;
                    if (boostAction == 1.25f)
                        RpcChangeColorSystemParticule(Color.green);
                    if (boostAction == 1.50f)
                        RpcChangeColorSystemParticule(Color.yellow);
                    if (boostAction == 1.75f)
                        RpcChangeColorSystemParticule(Color.red);
                }
                else
                {
                    if (collision.gameObject.GetComponent<StaffBehaviour>().GetCounterTime() < TIME_FOR_GOOD_ACTION)
                    {
                        //Effect Good
                        Debug.Log("Good");
                        uiManager.RpcSpawnGoodAction(collision.gameObject.GetComponent<StaffBehaviour>().GetIdPlayer());
                        RpcChangeColorSystemParticule(Color.blue);
                    }
                }
            }


            //Relfect a ball and refresh a direction and position
            Vector2 new_direction = transform.position - collision.gameObject.transform.position;
            new_direction = new Vector2(new_direction.x, new_direction.y / 10);
            new_direction.Normalize();
            direction = Vector2.Reflect(direction, new_direction);
            RpcRefreshDirection(direction);
            RpcRefreshPosition(transform.position);
            lastPlayerTouch = collision.gameObject.GetComponent<NetworkIdentity>().netId.Value;
        }

     
        //if Touch a Ground
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //if touch a walls in the time of 0.05f between;
            if (counterTime <= 0)
            {
                counterTime += 0.05f;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 2, 10);
                if (hit.collider != null)
                {
                    Debug.Log("Point of contact: " + hit.point);
                    Debug.DrawRay(hit.point, hit.normal * 3, Color.cyan, 10f);
                }

                //Relfect a ball and refresh a direction and position
                direction = Vector2.Reflect(direction, collision.gameObject.GetComponent<ReflexionManager>().normal);
                RpcRefreshDirection(direction);
                RpcRefreshPosition(transform.position);
            }
        }



        //if Touch a Goal
        if (collision.gameObject.layer == LayerMask.NameToLayer("Goal"))
        {
            
            Camera.main.GetComponent<CameraController>().ShakeCamera(SHAKE_POWER, SHAKE_DURATION);

            if (collision.gameObject.tag == "Goal1")
            {
                GameObject playerWin = GameObject.Find("Player1");
                playerWin.GetComponent<ScoreManager>().Goal();
                if (playerWin.GetComponent<ScoreManager>().GetScore() >= SCORE_WIN)
                {
                    playerWin.GetComponent<PlayerController>().ShowScoreEnd("Winner!");
                    GameObject.Find("Player0").GetComponent<PlayerController>().ShowScoreEnd("Looser!");
                }
                else
                {
                    gameCore.NextMatch();
                    Destroy(gameObject);
                }
            }
            if (collision.gameObject.tag == "Goal2")
            {
                GameObject playerWin = GameObject.Find("Player0");
                playerWin.GetComponent<ScoreManager>().Goal();
                if (playerWin.GetComponent<ScoreManager>().GetScore() >= SCORE_WIN)
                {
                    playerWin.GetComponent<PlayerController>().ShowScoreEnd("Winner!");
                    GameObject.Find("Player1").GetComponent<PlayerController>().ShowScoreEnd("Looser!");
                }
                else
                {
                    gameCore.NextMatch();
                    Destroy(gameObject);
                }
            }
            
            
        }
    }

    [ClientRpc]
    void RpcRefreshPosition(Vector3 _position) {
        transform.position = _position;
    }

    [ClientRpc]
    void RpcRefreshDirection(Vector2 _direction)
    {
        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(_direction.x, _direction.y) * (velocity * boostVelocity);
    }

    //Systeme Particules
    [ClientRpc]
    void RpcChangeColorSystemParticule(Color _color)
    {
        ParticleSystem.MainModule ma = particules.main;
        ma.startColor = _color;
    }

    [ClientRpc]
    public void RpcLaunchBall()
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
        direction = new Vector2(value, 0);
        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x, 0) * velocity;
    }


    //Fonction For Sync a Variable
    void OnChangeVelocity(float _velocity)
    {
        this.velocity = _velocity;
    }

    void OnChangeBoostVelocity(float _boost)
    {
        boostVelocity = _boost;
    }
    void OnChangeBoostAction(float _boost)
    {
        boostAction = _boost;
    }

    //Gizmos
    [Server]
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + direction.x * 5, transform.position.y + direction.y * 5, 0));
    }
}
