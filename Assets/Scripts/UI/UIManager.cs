using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UIManager : NetworkBehaviour {

    [Header("Spawner")]
    [SerializeField] Transform spawActionPlayer1;
    [SerializeField] Transform spawActionPlayer2;
    [Header("Prefab")]
    [SerializeField] GameObject prefabGood;
    [SerializeField] GameObject prefabPerfect;

    [ClientRpc]
    public void RpcSpawnGoodAction(int player)
    {
        GameObject action;
        switch (player)
        {
            case 1:
                action = Instantiate(prefabGood, spawActionPlayer1);
                NetworkServer.Spawn(action);
                Destroy(action, 1f);
                break;
            case 2:
                action = Instantiate(prefabGood, spawActionPlayer2);
                NetworkServer.Spawn(action);
                Destroy(action, 1f);
                break;
        }
    }

    [ClientRpc]
    public void RpcSpawnPerfectAction(int player)
    {
        GameObject action;
        switch (player)
        {
            case 1:
                action = Instantiate(prefabPerfect, spawActionPlayer1);
                NetworkServer.Spawn(action);
                Destroy(action, 1f);
                break;
            case 2:
                action = Instantiate(prefabPerfect, spawActionPlayer2);
                NetworkServer.Spawn(action);
                Destroy(action, 1f);
                break;
        }
    }
}
