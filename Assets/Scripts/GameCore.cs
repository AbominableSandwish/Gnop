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

    Text TextPointPlayer_1;
    Text TextPointPlayer_2;

    GameObject[] Players;

    GameObject Player_1;
    GameObject Player_2;

    GameObject BallUsing;

    

    bool p1IsReady;
    bool p2IsReady;

    GameObject tmp;
    GameObject tmp2;

    int pointPlayer1 = 0;
    int pointPlayer2 = 0;

    enum Game
    {
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
        Cursor.visible = false;

    }

    bool play = false;

    //Init Ui
    void InitUI()
    {
        //for a scoring
        TextPointPlayer_1 = GameObject.Find("TextPoint_Player1").GetComponent<Text>();
        TextPointPlayer_2 = GameObject.Find("TextPoint_Player2").GetComponent<Text>();
        
        //Timer before each next game
        GameObject.Find("Timer").GetComponent<TimerManager>().InitTextTimer(GameObject.Find("Timer").GetComponent<Text>());

        //Image action
    }

    private void Update()
    {
        //Wait 2 Player before to init
        if (!play)
        {
            Players = GameObject.FindGameObjectsWithTag("Player");
            if (Players.Length >= 2)
            {
                //Init Texts for scoring and timer
                InitUI();

                Debug.Log("Init Player");
                //Tnit a players
                InitPlayer();

                Player_1 = Players[0];
                Player_1.name = "Player1";

                Player_2 = Players[1];
                Player_2.name = "Player2";

                Player_1.GetComponent<ScoreManager>().InitTextScore(TextPointPlayer_1);
                Player_2.GetComponent<ScoreManager>().InitTextScore(TextPointPlayer_2);

                play = true;
                Debug.Log("Player Ready");
            }
        }

        switch (game)
        {
        
            case Game.LOBBY:
                if (play)
                {
                    Debug.Log("Init Game");
                    //Re
                    InitGame();
                    
                    //
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
                    if (GameObject.Find("Timer").GetComponent<TimerManager>().GetStartTime() < 0)
                    {
                        if(isServer)
                        GameObject.Find("Timer").GetComponent<TimerManager>().RpcLaunchTimer();

                        Player_1.GetComponentInChildren<PlayerController>().DelockPlayer();
                        Player_2.GetComponentInChildren<PlayerController>().DelockPlayer();
                        if(isServer)
                        BallUsing.GetComponent<BallManager>().LaunchBall();
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

    [Server]
    private void InitPlayer()
    {
        Player_1 = Players[0];
        Player_1.name = "Player1";
        
        Player_1.GetComponent<PlayerController>().CmdCursorLock();
        PrefabPalette.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.blue;
        tmp = Instantiate(PrefabPalette, Vector3.left * 25, Quaternion.identity);
       
        NetworkServer.Spawn(tmp);
        tmp.GetComponent<StaffManager>().InitStaff(1, Player_1.transform);

        Player_2 = Players[1];
        Player_2.name = "Player2";

        Player_2.GetComponent<PlayerController>().CmdCursorLock();
        PrefabPalette.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
        tmp2 = Instantiate(PrefabPalette, Vector3.right * 25, Quaternion.identity);
        
        NetworkServer.Spawn(tmp2);

        tmp2.GetComponent<StaffManager>().InitStaff(2, Player_2.transform);
    }

    //public void InitPalette()
    //{

    //    Players[0].GetComponent<PlayerController>().SetPalette(Instantiate(PrefabPalette));
    //    //Spawn the bullet on the Clients
    //   // CmdSpawnPalette();
    //}

    //[Command]
    //public void CmdSpawnPalette()
    //{
    //    
    //    NetworkServer.Spawn(Players[0].GetComponent<PlayerController>().GetPalette());
    //}


    public void InitPositionPlayers()
    {
        Player_1.GetComponent<PlayerController>().RpcInitPositionPlayer(new Vector3(-20, 0, 0));
        Player_2.GetComponent<PlayerController>().RpcInitPositionPlayer(new Vector3(20, 0, 0));
        tmp.GetComponent<StaffManager>().RpcinitSize();
        tmp2.GetComponent<StaffManager>().RpcinitSize();
    }

    public void NextMatch()
    {
        Player_1.GetComponentInChildren<PlayerController>().lockPlayer();
        Player_2.GetComponentInChildren<PlayerController>().lockPlayer();
        game = Game.LOBBY;
    }

}
