using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TimerManager : NetworkBehaviour {

    [Header("UI")]
    Text TimerStart;

    [SyncVar(hook = "OnChangeTimer")]
    public float Time_Start=3;

    public void OnChangeTimer(float time)
    {
        Time_Start = time;
        TimerStart.text = Time_Start.ToString();
    }

    [ClientRpc]
    public void RpcLaunchTimer()
    {
        Time_Start = 3;
        TimerStart.enabled = true;
    }

    public void InitTextTimer(Text _text)
    {
        TimerStart = _text;
    }

    public float GetStartTime() {
        return Time_Start;
    }

    [ClientRpc]
    public void RpcStopTimer()
    {
        TimerStart.enabled = false;
    }
}
