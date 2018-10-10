using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BallManager : NetworkBehaviour {

    //Variables
    private uint LastPlayerTouch;

    [SyncVar(hook = ("OnChangeVelocity"))]
    public float Velocity = 35;
    [SyncVar(hook =("OnChangeBoostVelocity"))]
    public float BoostVelocity = 1;
    [SyncVar(hook = ("OnChangeBoostAction"))]
    public float BoostAction = 1;

    private float Damage = 1;
    public ParticleSystem particules;

    Vector2 Direction;

    float counterTime;

    const short SCORE_WIN = 7;

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
    private void OnTriggerEnter2D(Collider2D collision)
    {

        //If touch a player
        if (collision.gameObject.layer == 9)
        {
            if (BoostVelocity < 1.5f)
                BoostVelocity += 0.025f;

            if (!collision.gameObject.GetComponent<StaffManager>().GetDefense())
            {
                //Effect Damage
                RpcChangeColorSystemParticule(Color.white);
                collision.gameObject.GetComponent<StaffManager>().RpcTakeDamage(BoostAction);
                BoostAction = 1;
            }
            else
            {
                //Effect for the shield
                if (collision.gameObject.GetComponent<StaffManager>().GetCounterTime() < 0.15f)
                {
                    //Effect Perfect
                    Debug.Log("Perfect!");
                    GameObject.Find("GameCore").GetComponent<UIManager>().RpcSpawnPerfectAction(collision.gameObject.GetComponent<StaffManager>().GetIdPlayer());
                    //Grow a damage and change color
                    BoostAction = BoostAction + 0.25f;
                    if (BoostAction == 1.25f)
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
                        //Effect Good
                        Debug.Log("Good");
                        GameObject.Find("GameCore").GetComponent<UIManager>().RpcSpawnGoodAction(collision.gameObject.GetComponent<StaffManager>().GetIdPlayer());
                        RpcChangeColorSystemParticule(Color.blue);
                    }
                }
            }


            //Relfect a ball and refresh a direction and position
            Vector2 new_direction = transform.position - collision.gameObject.transform.position;
            new_direction = new Vector2(new_direction.x, new_direction.y / 10);
            new_direction.Normalize();
            Direction = Vector2.Reflect(Direction, new_direction);
            RpcRefreshDirection(Direction);
            RpcRefreshPosition(transform.position);
            LastPlayerTouch = collision.gameObject.GetComponent<NetworkIdentity>().netId.Value;
        }

     
        //if Touch a Ground
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //if touch a walls in the time of 0.05f between;
            if (counterTime <= 0)
            {
                counterTime += 0.05f;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Direction, 2, 10);
                if (hit.collider != null)
                {
                    Debug.Log("Point of contact: " + hit.point);
                    Debug.DrawRay(hit.point, hit.normal * 3, Color.cyan, 10f);
                }

                //Relfect a ball and refresh a direction and position
                Direction = Vector2.Reflect(Direction, collision.gameObject.GetComponent<ReflexionManager>().normal);
                RpcRefreshDirection(Direction);
                RpcRefreshPosition(transform.position);
            }
        }



        //if Touch a Goal
        if (collision.gameObject.layer == 10)
        {
            
            Camera.main.GetComponent<CameraController>().ShakeCamera(0.3f, 0.3f);

            if (collision.gameObject.tag == "Goal1")
            {
                GameObject.Find("Player1").GetComponent<ScoreManager>().Goal();
                if(GameObject.Find("Player1").GetComponent<ScoreManager>().GetScore() >= SCORE_WIN)
                {
                    GameObject.Find("Player1").GetComponent<PlayerController>().ShowScoreEnd("Winner!");
                    GameObject.Find("Player0").GetComponent<PlayerController>().ShowScoreEnd("Looser!");
                }
                else
                {
                    GameObject.Find("GameCore").GetComponent<GameCore>().NextMatch();
                    Destroy(gameObject);
                }
            }
            if (collision.gameObject.tag == "Goal2")
            {
                GameObject.Find("Player0").GetComponent<ScoreManager>().Goal();
                if (GameObject.Find("Player0").GetComponent<ScoreManager>().GetScore() >= SCORE_WIN)
                {
                    GameObject.Find("Player0").GetComponent<PlayerController>().ShowScoreEnd("Winner!");
                    GameObject.Find("Player1").GetComponent<PlayerController>().ShowScoreEnd("Looser!");
                }
                else
                {
                    GameObject.Find("GameCore").GetComponent<GameCore>().NextMatch();
                    Destroy(gameObject);
                }
            }
            
            
        }
    }

    [ClientRpc]
    void RpcRefreshPosition(Vector3 Position) {
        transform.position = Position;
    }

    [ClientRpc]
    void RpcRefreshDirection(Vector2 direction)
    {
        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x, direction.y) * (Velocity * BoostVelocity);
    }

    //Systeme Particules
    [ClientRpc]
    void RpcChangeColorSystemParticule(Color color)
    {
        ParticleSystem.MainModule ma = particules.main;
        ma.startColor = color;
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
        Direction = new Vector2(value, 0);
        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(Direction.x, 0) * Velocity;
    }


    //Fonction For Sync a Variable
    void OnChangeVelocity(float velocity)
    {
        Velocity = velocity;
    }

    void OnChangeBoostVelocity(float boost)
    {
        BoostVelocity = boost;
    }
    void OnChangeBoostAction(float boost)
    {
        BoostAction = boost;
    }

    //Gizmos
    [Server]
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + Direction.x * 5, transform.position.y + Direction.y * 5, 0));
    }
}
