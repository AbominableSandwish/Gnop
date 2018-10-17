using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TimerManager : NetworkBehaviour {

    [SyncVar(hook = "OnChangeTimer")]
    public float timeStart=3;


    public void OnChangeTimer(float time)
    {
        timeStart = time;
        GetComponent<Text>().text = timeStart.ToString();
    }

    [ClientRpc]
    public void RpcLaunchTimer()
    {
        timeStart = 3;
        GetComponent<Text>().enabled = true;
    }

    public float GetStartTime() {
        return timeStart;
    }

    [ClientRpc]
    public void RpcStopTimer()
    {
        GetComponent<Text>().enabled = false;
    }
}
