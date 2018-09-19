using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
public class StaffManager : MonoBehaviour
{
    Transform target;


    private void Update()
    {
        if(target != null)
        {
            Vector2 direction = target.position - transform.position;
            GetComponent<Rigidbody2D>().velocity = direction * 60;
        }   
    }

    //[Server]
    public void SetPlayerFollow(Transform player)
    {
        target = player;
    }
}