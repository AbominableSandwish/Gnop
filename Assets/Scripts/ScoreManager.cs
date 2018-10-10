using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ScoreManager : NetworkBehaviour
{
    [SyncVar(hook = "OnChangeScore")]
    public int Score_Player = 0;

    public Text Text_ScorePlayer;

    public void Goal()
    {
        if (!isServer)
        {
            return;
        }

        OnChangeScore(Score_Player+1);
        Text_ScorePlayer.text = Score_Player.ToString();
    }
    
    public void InitTextScore(Text _text)
    {
        Text_ScorePlayer = _text;
    }

    void OnChangeScore(int score_Player)
    {
        Score_Player = score_Player;
    }

    public int GetScore()
    {
        return Score_Player;
    }
}
