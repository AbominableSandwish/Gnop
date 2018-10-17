using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ScoreManager : NetworkBehaviour
{
    [SyncVar(hook = "OnChangeScore")]
    public int scorePlayer = 0;

    public Text textScorePlayer;
     
    public void Goal()
    {
        if (!isServer)
        {
            return;
        }

        OnChangeScore(scorePlayer+1);
        textScorePlayer.text = scorePlayer.ToString();
    }
    
    public void InitTextScore(Text _text)
    {
        textScorePlayer = _text;
    }

    void OnChangeScore(int score_Player)
    {
        scorePlayer = score_Player;
        textScorePlayer.text = scorePlayer.ToString();
    }

    public int GetScore()
    {
        return scorePlayer;
    }
}
