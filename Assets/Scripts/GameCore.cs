using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameCore : MonoBehaviour {

    public GameObject BallPrefab;
    [SerializeField] GameObject PrefabPalette;

    GameObject[] Players;

    GameObject Player_1;
    GameObject Player_2;

    GameObject BallUsing;

    [Header("UI")]
    public Text TextPointPlayer_1;
    public Text TextPointPlayer_2;

    bool P1_isReady;
    bool P2_isReady;

    GameObject tmp;
         GameObject tmp2;

    public Text TimerStart;

    int pointPlayer1 = 0;
    int pointPlayer2 = 0;
    enum Game
    {
        LOBBY,
        PARTY,
        WIN
    }

    public bool Server;
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
    void Start() {
        Cursor.visible = false;

    }

    bool play = false;

    //[Server]
    private void Update()
    {
        
        if (Server)
        {
            if (Players.Length >= 2 && !play)
            {
                Debug.Log("InitGame");
                play = true;
                // Players[0].tag = "Player1";
                //Players[1].tag = "Player2";
                // Cursor.lockState = CursorLockMode.Locked;
                GameObject tmp = Instantiate(PrefabPalette, Vector3.left, Quaternion.identity);
                NetworkServer.Spawn(tmp);
                Debug.Log(NetworkServer.objects.Count);


                GameObject tmp2 = Instantiate(PrefabPalette, Vector3.right, Quaternion.identity);
                NetworkServer.Spawn(tmp2);
              //  Debug.Log(NetworkServer.objects.Count);


            }
        }
        else
        {
            Players = GameObject.FindGameObjectsWithTag("Player");
            if (Players.Length >= 2 && !play) {
                play = true;
                tmp = Instantiate(PrefabPalette, Vector3.left * 25, Quaternion.identity);
                tmp2 = Instantiate(PrefabPalette, Vector3.right * 25, Quaternion.identity);

                Player_1 = Players[0];
                Player_2 = Players[1];
                tmp.GetComponent<StaffManager>().SetPlayerFollow(Player_1.transform);
                tmp2.GetComponent<StaffManager>().SetPlayerFollow(Player_2.transform);
            }

        }

        switch (game)
        {
            case Game.LOBBY:
                
                if (Players.Length >= 2)
                {
                    Players[0].tag = "Player1";
                    Players[1].tag = "Player2";
                    Cursor.lockState = CursorLockMode.Locked;
                    InitMatch();
                    game = Game.PARTY;
                }
                else
                {
                    Players = GameObject.FindGameObjectsWithTag("Player");
                }
                break;

            case Game.PARTY:
                Counter_time += Time.deltaTime;
                if (Counter_time >= 1)
                {
                    Counter_time = 0;
                    Time_Start--;
                    TimerStart.text = Time_Start.ToString();
                    if (Time_Start == 0)
                    {
                        TimerStart.enabled = false;
                        Player_1.GetComponentInChildren<PlayerController>().DelockPlayer();
                        Player_2.GetComponentInChildren<PlayerController>().DelockPlayer();
                        BallUsing.GetComponent<BallManager>().LaunchBall();

                    }
                }
                break;
        }
    }


    //[Server]
    public void Goal(Player goalFor)
    {
        if (goalFor == Player.PLAYER_1)
        {
            pointPlayer1++;
            TextPointPlayer_1.text = pointPlayer1.ToString();
        }

        if (goalFor == Player.PLAYER_2)
        {
            pointPlayer2++;
            TextPointPlayer_2.text = pointPlayer2.ToString();
        }
        Player_1.GetComponentInChildren<PlayerController>().lockPlayer();
        Player_2.GetComponentInChildren<PlayerController>().lockPlayer();
        InitMatch();
    }

    //[Server]
    private void InitMatch()
    {
        //Players[1].GetComponent<PlayerController>().SetPalette(Instantiate(PrefabPalette));
        //NetworkServer.Spawn(Players[1].GetComponent<PlayerController>().GetPalette());
        // InitPalette();
        Player_1.GetComponent<PlayerController>().InitMove();
        Player_2.GetComponent<PlayerController>().InitMove();
        BallUsing = Instantiate(BallPrefab, Vector3.zero, Quaternion.identity);
        Time_Start = 3;
        TimerStart.enabled = true;
        TimerStart.text = Time_Start.ToString();


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
}
