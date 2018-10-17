using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameCore : NetworkBehaviour
{
    enum stateGame
    {
        INIT,
        LOBBY,
        COUNTER,
        IN_GAME,
        PAUSE,
        WIN
    }

    #region Variables
    stateGame game = stateGame.LOBBY;
    //Init Variables
    [Header("PrefabNeed")]
    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject prefabPalette;
    [SerializeField] GameObject panelScoreEnd;

    public Text textPointPlayer_1;
    public Text textPointPlayer_2;
    public TimerManager timeManager;

    public List<PlayerController> players;
    List<GameObject> staffs;

    GameObject ballUsing;

    bool play = false;

    float timeStart = 0;
    float Counter_time = 0;
    #endregion

    // Use this for initialization
    void Start()
    {
        staffs = new List<GameObject>(2);

        players = new List<PlayerController>();
        int i = 0;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.name = "Player" + i.ToString();
            players.Add(player.GetComponent<PlayerController>());  
        }
        //Cursor.visible = false;
    }

    [Server]
    public void AddPlayer(PlayerController _controller)
    {
        if (this.players == null)
            this.players = new List<PlayerController>();
        this.players.Add(_controller);
    }

    private void Update()
    {
        switch (game)
        {
            case stateGame.LOBBY:
                //Wait 2 Player before to init
                if (players.Count >= 2 && !play)
                {
                    Debug.Log("Init Player");
                    //Tnit a players

                    InitPlayer(0);
                    InitPlayer(1);               

                    play = true;
                    Debug.Log("Player Ready");
                    game = stateGame.INIT;

                }
                break;

            case stateGame.INIT:
                if (play)
                {
                    Debug.Log("Init Game");
                   
                    //Init Game
                    InitGame();

                    //Init Timer
                    timeStart = 3;
                    Counter_time = 0;
                    game = stateGame.COUNTER;
                }
                break;

            case stateGame.COUNTER:
                Counter_time += Time.deltaTime;
                if (Counter_time >= 1)
                {
                    Counter_time = 0;

                    timeManager.OnChangeTimer(timeStart--);
                    Debug.Log("Ready when " + timeStart);
                    if (timeManager.GetStartTime() == 0)
                    {

                        players[0].GetComponentInChildren<PlayerController>().DelockPlayer();
                        players[1].GetComponentInChildren<PlayerController>().DelockPlayer();

                        if (isServer)
                        {
                            ballUsing.GetComponent<BallManager>().RpcLaunchBall();
                            timeManager.RpcStopTimer();
                        }

                        game = stateGame.IN_GAME;
                    }
                }
                break;
        }
    }

    [Server] //Server Init
    private void InitGame()
    {
        InitPositionPlayers();
        ballUsing = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(ballUsing);

        GameObject.Find("Timer").GetComponent<TimerManager>().RpcLaunchTimer();
        Counter_time = 0;
        game = stateGame.COUNTER;
    }
    
    public void InitPlayer(int _idPlayer)
    {
        //Init and Spawn Staff Player
        //Players[id_player].name = "Player" + id_player.ToString();
        Debug.Log(_idPlayer);
        players[_idPlayer].GetComponent<PlayerController>().CmdCursorLock();
        if (_idPlayer == 0)
        {
            if(isServer)
                staffs.Add(Instantiate(prefabPalette, Vector3.left * 21, Quaternion.identity));
            InitTextScorePlayer(_idPlayer);
            players[_idPlayer].GetComponent<PlayerController>().InitScoreManager(textPointPlayer_1);
        }
        if (_idPlayer == 1)
        {
            if (isServer)
                staffs.Add(Instantiate(prefabPalette, Vector3.right * 21, Quaternion.identity));
            InitTextScorePlayer(_idPlayer);
            players[_idPlayer].GetComponent<PlayerController>().InitScoreManager(textPointPlayer_2);
        }

        if (isServer)
        {
            NetworkServer.Spawn(staffs[_idPlayer]);
            staffs[_idPlayer].GetComponent<StaffBehaviour>().InitStaff(_idPlayer, players[_idPlayer].transform);
        }
    }

    void InitTextScorePlayer(int _idPlayer)
    {
        switch (_idPlayer)
        {
            case 0:
                players[_idPlayer].GetComponent<ScoreManager>().textScorePlayer = textPointPlayer_1;
                break;
            case 1:
                players[_idPlayer].GetComponent<ScoreManager>().textScorePlayer = textPointPlayer_2;
                break;
        }
    }

    //Replace a positions at players and their Staffs
    public void InitPositionPlayers()
    {
        players[0].GetComponent<PlayerController>().RpcInitPositionPlayer(new Vector3(-21, 0, 0));
        players[1].GetComponent<PlayerController>().RpcInitPositionPlayer(new Vector3(21, 0, 0));
        staffs[0].GetComponent<StaffBehaviour>().RpcinitSize();
        staffs[1].GetComponent<StaffBehaviour>().RpcinitSize();
    }

    public void NextMatch()
    {
        players[0].GetComponentInChildren<PlayerController>().LockPlayer();
        players[1].GetComponentInChildren<PlayerController>().LockPlayer();
        game = stateGame.INIT;

    }

    [Server]
    public List<PlayerController> GetPlayers()
    {
        return players;
    }

    public GameObject GetPanelScoreEnd()
    {
        return panelScoreEnd;
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
