using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    #region Variables
    GameCore gameCore;
    int id_player;
    //[SyncVar(hook = "OnChangeColorPlayer")]
    //public Color color_player;
    GameObject paletteUsing;
    public GameObject PrefabPalette;

    int height = 20;

    Vector3 centerPosition;
    Rigidbody2D body;

    [SyncVar(hook = "OnChangePlayerIsLocking")]
    public bool playerIsLocking = false;

    [SyncVar(hook = "OnChangePlayerDefense")]
    public bool defense = false;

    //UI
    float counterTime;
    #endregion


    // Use this for initialization
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();

    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<SpriteRenderer>().color = Color.blue;
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (!playerIsLocking)
            {
                //Action Move
                if (Input.GetAxis("Mouse Y") > 0)
                {
                    if (transform.position.y <= centerPosition.y + height / 2)
                    {
                        CmdChangePositionPlayer(new Vector3(0, Input.GetAxis("Mouse Y")));
                    }
                }

                if (Input.GetAxis("Mouse Y") < 0)
                {
                    if (transform.position.y >= centerPosition.y - height / 2)
                    {
                        CmdChangePositionPlayer(new Vector3(0, Input.GetAxis("Mouse Y")));
                    }
                }

                //Action Shield
                if (Input.GetMouseButton(0))
                {
                    Debug.Log("PressMouse");
                    if (defense != true)
                        CmdChangePlayerDefense(true);
                }
                else
                {
                    if (defense != false)
                        CmdChangePlayerDefense(false);
                }
                return;
            }
        }
    }

    //Refresh the position for player 
    [Command]
    private void CmdChangePositionPlayer(Vector3 _position)
    {
        transform.position += _position;
    }

    //Active or not a Shield on a player in the server first
    [Command]
    private void CmdChangePlayerDefense(bool _defense)
    {
        this.defense = _defense;
    }


    //player can't to move
    public void DelockPlayer()
    {
        playerIsLocking = false;
    }

    //player can to move
    public void LockPlayer()
    {
        playerIsLocking = true;
    }

    public GameObject GetPalette()
    {
        return paletteUsing;
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
    public void RpcInitPositionPlayer(Vector3 _position)
    {
        centerPosition = _position;
        transform.position = _position;
    }

    [ClientRpc]
    public void RpcSetPalette(GameObject _palette)
    {
        paletteUsing = _palette;
    }

    [Client]
    public void SetIdPlayer(int _id, GameCore _gamecore)
    {
        this.id_player = _id;
        this.gameCore = _gamecore;
        this.name = "Player" + _id.ToString();
        _gamecore.AddPlayer(this);
    }

    #region SyncVar
    //Fonction for Sync a variable
    void OnChangePlayerDefense(bool _defense)
    {
        this.defense = _defense;
    }

    public void OnChangePlayerIsLocking(bool _change)
    {
        playerIsLocking = _change;
    }
    #endregion

    private void OnDrawGizmos()
    {
        if (centerPosition != Vector3.zero)
        {
            Gizmos.DrawLine(new Vector2(centerPosition.x, centerPosition.y - height / 2), new Vector2(centerPosition.x, centerPosition.y + height / 2));
            Gizmos.DrawIcon(new Vector2(centerPosition.x, centerPosition.y - height / 2), "Bot");
            Gizmos.DrawIcon(new Vector2(centerPosition.x, centerPosition.y + height / 2), "Top");
        }
    }

    public void ShowScoreEnd(string _info)
    {
        CmdCursorDelock();
        GameObject Panel = GameObject.Find("GameCore").GetComponent<GameCore>().GetPanelScoreEnd();
        Panel.SetActive(true);
        GameObject.Find("Text_Score").GetComponent<Text>().text = _info;
    }

    public void InitScoreManager(Text _text)
    {
        Debug.Log(_text.name);
        GetComponent<ScoreManager>().InitTextScore(_text);
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