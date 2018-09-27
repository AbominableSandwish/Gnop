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
            Debug.Log("ICI");
            return;
        }
 
        Score_Player++;
    }
    
    public void InitTextScore(Text _text) {
        Text_ScorePlayer = _text;
    }


    void OnChangeScore(int Score_Player)
    {
        Text_ScorePlayer.text = Score_Player.ToString();
    }

}
