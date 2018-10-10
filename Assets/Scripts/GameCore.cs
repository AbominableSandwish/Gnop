using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameCore : NetworkBehaviour
{
    //Init Variables
    [Header("PrefabNeed")]
    [SerializeField] GameObject BallPrefab;
    [SerializeField] GameObject PrefabPalette;
    [SerializeField] GameObject Panel_ScoreEnd;

    Text TextPointPlayer_1;
    Text TextPointPlayer_2;

    List<GameObject> Players;
    List<GameObject> Staffs;

    GameObject BallUsing;

    bool play = false;

    enum Game
    {
        INIT,
        LOBBY,
        COUNTER,
        IN_GAME,
        PAUSE,
        WIN
    }
    Game game = Game.LOBBY;

    float Time_Start = 0;
    float Counter_time = 0;

    public enum Player
    {
        PLAYER_1,
        PLAYER_2,
        NONE
    }

    // Use this for initialization
    void Start()
    {
        Players = new List<GameObject>(2);
        Staffs = new List<GameObject>(2);
        //Cursor.visible = false;
    }

    [Command]
    public void CmdAddPlayer(GameObject new_player)
    {
        new_player.name = "Player" + Players.Count.ToString();
        Players.Add(new_player);
        Debug.Log("AddPlayer");
    }


    //Init Ui
    void InitUI()
    {
        //for a scoring
        TextPointPlayer_1 = GameObject.Find("TextPoint_Player1").GetComponent<Text>();
        TextPointPlayer_2 = GameObject.Find("TextPoint_Player2").GetComponent<Text>();
    }


    private void Update()
    {
        switch (game)
        {

            case Game.LOBBY:
                //Wait 2 Player before to init
                if (Players.Count >= 2 && !play)
                {
                    //Init Texts for scoring and timer
                    InitUI();
                    Debug.Log("Init Player");
                    //Tnit a players

                    InitPlayer(0);
                    InitPlayer(1);               

                    play = true;
                    Debug.Log("Player Ready");
                    game = Game.INIT;

                }
                break;

            case Game.INIT:
                if (play)
                {
                    Debug.Log("Init Game");
                   
                    //Init Game
                    InitGame();

                    //Init Timer
                    Time_Start = 3;
                    Counter_time = 0;
                    game = Game.COUNTER;
                }
                break;

            case Game.COUNTER:
                Counter_time += Time.deltaTime;
                if (Counter_time >= 1)
                {
                    Counter_time = 0;

                    GameObject.Find("Timer").GetComponent<TimerManager>().OnChangeTimer(Time_Start--);
                    Debug.Log("Ready when " + Time_Start);
                    if (GameObject.Find("Timer").GetComponent<TimerManager>().GetStartTime() == 0)
                    {

                        Players[0].GetComponentInChildren<PlayerController>().DelockPlayer();
                        Players[1].GetComponentInChildren<PlayerController>().DelockPlayer();

                        if (isServer)
                            BallUsing.GetComponent<BallManager>().RpcLaunchBall();
                        GameObject.Find("Timer").GetComponent<TimerManager>().RpcStopTimer();
                        game = Game.IN_GAME;
                    }
                }
                break;
        }
    }

    [Server] //Server Init
    private void InitGame()
    {
        InitPositionPlayers();
        BallUsing = Instantiate(BallPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(BallUsing);

        GameObject.Find("Timer").GetComponent<TimerManager>().RpcLaunchTimer();
        Counter_time = 0;
        game = Game.COUNTER;
    }


    public void InitPlayer(int id_player)
    {
        //Init and Spawn Staff Player
        Players[id_player].name = "Player" + id_player;

        Players[id_player].GetComponent<PlayerController>().CmdCursorLock();
        PrefabPalette.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.blue;
        if (id_player == 0)
        {
            Staffs.Add(Instantiate(PrefabPalette, Vector3.left * 21, Quaternion.identity));
            Players[id_player].GetComponent<ScoreManager>().Text_ScorePlayer = TextPointPlayer_1;
            Players[id_player].GetComponent<PlayerController>().InitScoreManager(TextPointPlayer_1);
        }
        if (id_player == 1)
        {
            Staffs.Add(Instantiate(PrefabPalette, Vector3.right * 21, Quaternion.identity));
            Players[id_player].GetComponent<ScoreManager>().Text_ScorePlayer = TextPointPlayer_2;
            Players[id_player].GetComponent<PlayerController>().InitScoreManager(TextPointPlayer_2);
        }

        NetworkServer.Spawn(Staffs[id_player]);
        Staffs[id_player].GetComponent<StaffManager>().InitStaff(id_player, Players[id_player].transform);
    }

    //Replace a positions at players and their Staffs
    public void InitPositionPlayers()
    {
        Players[0].GetComponent<PlayerController>().RpcInitPositionPlayer(new Vector3(-21, 0, 0));
        Players[1].GetComponent<PlayerController>().RpcInitPositionPlayer(new Vector3(21, 0, 0));
        Staffs[0].GetComponent<StaffManager>().RpcinitSize();
        Staffs[1].GetComponent<StaffManager>().RpcinitSize();
    }

    public void NextMatch()
    {
        Players[0].GetComponentInChildren<PlayerController>().LockPlayer();
        Players[1].GetComponentInChildren<PlayerController>().LockPlayer();
        game = Game.INIT;

    }

    public GameObject GetPanelScoreEnd()
    {
        return Panel_ScoreEnd;
    }

    //[Server]
    //public void Changecolor(short id, Color color)g
    //{
    //    Debug.Log(color);
    //    switch (id)
    //    {
    //        case 0:
    //            Player1_Color = color;
    //            GameObject.Find("Goal_0").GetComponent<GoalManager>().RpcChangeColorGoal(Player1_Color);
    //            Staffs[0].GetComponent<StaffManager>().RpcChangeColorStaffPlayer(Player1_Color);
    //            break;

    //        case 1:
    //            Player2_Color = color;
    //            GameObject.Find("Goal_1").GetComponent<GoalManager>().RpcChangeColorGoal(Player2_Color);
    //            Staffs[1].GetComponent<StaffManager>().RpcChangeColorStaffPlayer(Player2_Color);
    //            break;
    //    }           
    //}


}
