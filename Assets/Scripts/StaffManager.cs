using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

public class StaffManager : NetworkBehaviour
{
    int idPlayer;
    Transform target;
    public Vector3 Size;

    float Counter_time;
    float Counter_time_damage;

    Color color_staff;

    private void Update()
    {
        if (target != null)
        {
            Vector2 direction = target.position - transform.position;
            GetComponent<Rigidbody2D>().velocity = direction * 20;

            if (target.GetComponent<PlayerController>().Defense)
            {
                if(GetComponent<SpriteRenderer>().color != Color.white)
                RpcChangeColor(Color.white);
                

                Counter_time_damage += Time.deltaTime;
                if (Counter_time_damage >= 0.25f)
                {
                    RpcUseDefense();
                    Counter_time_damage = 0.0f;
                }

                Counter_time += Time.deltaTime;
            }
            else
            {
                if (GetComponent<SpriteRenderer>().color != Color.black)
                    RpcChangeColor(Color.black);

                Counter_time = 0;
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
        transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y - 0.02f);
    }


    [ClientRpc]
    public void RpcTakeDamage(float boost_damage)
    {
        if(transform.localScale.y > 1.5f)
        transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y - 0.5f*boost_damage);
    }


    [ClientRpc]
    public void RpcinitSize()
    {
        transform.localScale = Size;
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
        return target.GetComponent<PlayerController>().Defense;
    }

    public float GetCounterTime()
    {
        return Counter_time;
    }

    public int GetIdPlayer()
    {
        return idPlayer;
    }


}