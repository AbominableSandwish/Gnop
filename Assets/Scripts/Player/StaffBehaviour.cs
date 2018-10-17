using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

public class StaffBehaviour : NetworkBehaviour
{
    private const float MIN_DAMAGE = 0.5f;
    private const float MIN_HEIGHT = 1.5f;

    int idPlayer;
    Transform target;
    public Vector3 size;
    private float velocity = 20.0f;

    float counterTime;
    float counterTimeDamage;

    Color color_staff;

    private void Update()
    {
        if (target != null)
        {
            Vector2 direction = target.position - transform.position;
            GetComponent<Rigidbody2D>().velocity = direction * velocity;

            if (target.GetComponent<PlayerController>().defense)
            {
                if(GetComponent<SpriteRenderer>().color != Color.white)
                RpcChangeColor(Color.white);
                

                counterTimeDamage += Time.deltaTime;
                if (counterTimeDamage >= 0.25f)
                {
                    RpcUseDefense();
                    counterTimeDamage = 0.0f;
                }

                counterTime += Time.deltaTime;
            }
            else
            {
                if (GetComponent<SpriteRenderer>().color != Color.black)
                    RpcChangeColor(Color.black);

                counterTime = 0;
            }
        }   
    }

    [ClientRpc]
    void RpcChangeColor(Color _color)
    {
        GetComponent<SpriteRenderer>().color = _color;
    }


    [ClientRpc]
    void RpcUseDefense()
    {
        transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y - 0.01f);
    }


    [ClientRpc]
    public void RpcTakeDamage(float _boostDamage)
    {
        if(transform.localScale.y > MIN_HEIGHT)
        transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y - MIN_DAMAGE * _boostDamage);
    }


    [ClientRpc]
    public void RpcinitSize()
    {
        transform.localScale = size;
    }

    public void InitStaff(int _player, Transform _target)
    {
        this.idPlayer = _player;
        this.target = _target;
        this.target.GetComponent<PlayerController>().RpcSetPalette(gameObject);
        RpcChangeColorStaffPlayer(Color.white);
    }

    [ClientRpc]
    public void RpcChangeColorStaffPlayer(Color color)
    {
        color_staff = color;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = color_staff;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>());
        }
    }

    public bool GetDefense()
    {
        return target.GetComponent<PlayerController>().defense;
    }

    public float GetCounterTime()
    {
        return counterTime;
    }

    public int GetIdPlayer()
    {
        return idPlayer;
    }


}