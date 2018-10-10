using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TimerManager : NetworkBehaviour {

    [SyncVar(hook = "OnChangeTimer")]
    public float Time_Start=3;


    public void OnChangeTimer(float time)
    {
        Time_Start = time;
        GetComponent<Text>().text = Time_Start.ToString();
    }

    [ClientRpc]
    public void RpcLaunchTimer()
    {
        Time_Start = 3;
        GetComponent<Text>().enabled = true;
    }

    public float GetStartTime() {
        return Time_Start;
    }

    [ClientRpc]
    public void RpcStopTimer()
    {
        GetComponent<Text>().enabled = false;
    }
}
