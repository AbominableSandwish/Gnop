using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    GameObject PaletteUsing;
    public GameObject PrefabPalette;


    bool PowerBall = false;

    [SyncVar(hook ="OnChangePlayerIsLocking")]
    public bool PlayerisLocking = false;

    int Width = 20;
    int Height = 20;
    Vector3 CenterPosition;

    Rigidbody2D body;

     [SyncVar(hook ="OnChangePlayerDefense")]
    public bool Defense = false;

    //UI
    float Counter_Time;
    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }


    public override void OnStartLocalPlayer()
    {
        GetComponent<SpriteRenderer>().color = Color.blue;
    }

    [Client]
    private void Update()
    {
        if (isLocalPlayer)
        {
            if (!PlayerisLocking)
            {
                if(transform.position != transform.position + new Vector3(0, Input.GetAxis("Mouse Y")))
                CmdChangePositionPlayer(new Vector3(0, Input.GetAxis("Mouse Y")));

                if (Input.GetMouseButton(0)) {
                    Debug.Log("PressMouse");
                    if(Defense != true)
                    CmdChangePlayerDefense(true);
                }
                else
                {
                    if(Defense != false)
                    CmdChangePlayerDefense(false);
                }

                return;
            }
        }
    }

    [Command]
    private void CmdChangePositionPlayer(Vector3 position)
    {
        transform.position += position;
    }

    [Command]
    private void CmdChangePlayerDefense(bool defense)
    {
        Defense = defense;
    }

    void OnChangePlayerDefense(bool defense)
    {
        Defense = defense;
    }

    public void OnChangePlayerIsLocking(bool _change)
    {
        PlayerisLocking = _change;
    }

    public bool GetPowerBall()
    {
        return PowerBall;
    }

    public void DelockPlayer()
    {

        PlayerisLocking = false;
    }

    public void lockPlayer()
    {

        PlayerisLocking = true;
    }

    [ClientRpc]
    public void RpcSetPalette(GameObject Palette) {
        PaletteUsing = Palette;
    }

    public GameObject GetPalette()
    {
        return PaletteUsing;
    }

    public void Init(GameObject Prefab)
    {

        if (isLocalPlayer) {
            PaletteUsing = Prefab;
            //  CmdInitPalette();
        }
        else
        {
            //    PaletteUsing = Instantiate(PrefabPalette);
        }


    }

    [Command]
    public void CmdCursorLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    [ClientRpc]
    public void RpcInitPositionPlayer(Vector3 position) {
        CenterPosition = position;
        transform.position = position;
    }

    private void OnDrawGizmos()
    {
        if (CenterPosition != null) {
            Gizmos.DrawLine(new Vector2(CenterPosition.x - Width / 2, CenterPosition.y - Height / 2), new Vector2(CenterPosition.x + Width / 2, CenterPosition.y - Height / 2));
            Gizmos.DrawLine(new Vector2(CenterPosition.x - Width / 2, CenterPosition.y - Height / 2), new Vector2(CenterPosition.x - Width / 2, CenterPosition.y + Height / 2));
            Gizmos.DrawLine(new Vector2(CenterPosition.x - Width / 2, CenterPosition.y + Height / 2), new Vector2(CenterPosition.x + Width / 2, CenterPosition.y + Height / 2));
            Gizmos.DrawLine(new Vector2(CenterPosition.x + Width / 2, CenterPosition.y - Height / 2), new Vector2(CenterPosition.x + Width / 2, CenterPosition.y + Height / 2));
        }
    }


}

