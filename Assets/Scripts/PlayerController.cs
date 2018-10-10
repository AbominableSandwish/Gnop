using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    short id_player;
    //[SyncVar(hook = "OnChangeColorPlayer")]
    //public Color color_player;
    GameObject PaletteUsing;
    public GameObject PrefabPalette;

    int Height = 20;

    Vector3 CenterPosition;
    Rigidbody2D body;

    [SyncVar(hook = "OnChangePlayerIsLocking")]
    public bool PlayerIsLocking = false;

    [SyncVar(hook = "OnChangePlayerDefense")]
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
            if (!PlayerIsLocking)
            {
                //Action Move
                if (Input.GetAxis("Mouse Y") > 0)
                {
                    if (transform.position.y <= CenterPosition.y + Height / 2)
                    {
                        CmdChangePositionPlayer(new Vector3(0, Input.GetAxis("Mouse Y")));
                    }
                }

                if (Input.GetAxis("Mouse Y") < 0)
                {
                    if (transform.position.y >= CenterPosition.y - Height / 2)
                    {
                        CmdChangePositionPlayer(new Vector3(0, Input.GetAxis("Mouse Y")));
                    }
                }

                //Action Shield
                if (Input.GetMouseButton(0))
                {
                    Debug.Log("PressMouse");
                    if (Defense != true)
                        CmdChangePlayerDefense(true);
                }
                else
                {
                    if (Defense != false)
                        CmdChangePlayerDefense(false);
                }

                return;
            }
        }
    }

    //Refresh the position for player 
    [Command]
    private void CmdChangePositionPlayer(Vector3 position)
    {
        transform.position += position;
    }

    //Active or not a Shield on a player in the server first
    [Command]
    private void CmdChangePlayerDefense(bool defense)
    {
        Defense = defense;
    }


    //player can't to move
    public void DelockPlayer()
    {
        PlayerIsLocking = false;
    }

    //player can to move
    public void LockPlayer()
    {
        PlayerIsLocking = true;
    }

    public GameObject GetPalette()
    {
        return PaletteUsing;
    }


    [Command]
    public void CmdCursorLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    [Command]
    public void CmdCursorDelock()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    //Replace a player in a init position
    [ClientRpc]
    public void RpcInitPositionPlayer(Vector3 position)
    {
        CenterPosition = position;
        transform.position = position;
    }

    [ClientRpc]
    public void RpcSetPalette(GameObject Palette)
    {
        PaletteUsing = Palette;
    }

    public void SetIdPlayer(short id)
    {
        id_player = id;
        this.name = "Player" + id.ToString();
    }

    //Fonction for Sync a variable
    void OnChangePlayerDefense(bool defense)
    {
        Defense = defense;
    }

    public void OnChangePlayerIsLocking(bool _change)
    {
        PlayerIsLocking = _change;
    }

    private void OnDrawGizmos()
    {
        if (CenterPosition != Vector3.zero)
        {
            Gizmos.DrawLine(new Vector2(CenterPosition.x, CenterPosition.y - Height / 2), new Vector2(CenterPosition.x, CenterPosition.y + Height / 2));
            Gizmos.DrawIcon(new Vector2(CenterPosition.x, CenterPosition.y - Height / 2), "Bot");
            Gizmos.DrawIcon(new Vector2(CenterPosition.x, CenterPosition.y + Height / 2), "Top");
        }
    }

    public void ShowScoreEnd(string info)
    {
        CmdCursorDelock();
        GameObject Panel = GameObject.Find("GameCore").GetComponent<GameCore>().GetPanelScoreEnd();
        Panel.SetActive(true);
        GameObject.Find("Text_Score").GetComponent<Text>().text = info;
    }

    [Client]
    public void InitScoreManager(Text text)
    {
        if (isLocalPlayer)
        {
            GetComponent<ScoreManager>().InitTextScore(text);
        }
    }




    //void OnChangeColorPlayer(Color color)
    //{
    //    color_player = color;
    //}



    //public void InitCanvasColor_selection()
    //{
    //    GameObject tmp = Instantiate(Prefab_colorSelection);
    //    tmp.GetComponent<ButtonManager>().SetPlayerController(GetComponent<PlayerController>());
    //}

    //public void RecieveColorPlayer(Color color)
    //{
    //    Debug.Log(color);
    //    color_player = color;
    //    Rpclol(color);
    //}

    //[ClientRpc]
    //void Rpclol(Color color)
    //{
    //    GameObject.Find("GameCore").GetComponent<GameCore>().Changecolor(id_player, color);
    //}
}