using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UIManager : NetworkBehaviour {

    [Header("Spawner")]
    [SerializeField] Transform SpawActionPlayer1;
    [SerializeField] Transform SpawActionPlayer2;
    [Header("Prefab")]
    [SerializeField] GameObject PrefabGood;
    [SerializeField] GameObject PrefabPerfect;

    [ClientRpc]
    public void RpcSpawnGoodAction(int player)
    {
        GameObject action;
        switch (player)
        {
            case 1:
                action = Instantiate(PrefabGood, SpawActionPlayer1);
                NetworkServer.Spawn(action);
                Destroy(action, 1f);
                break;
            case 2:
                action = Instantiate(PrefabGood, SpawActionPlayer2);
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
                action = Instantiate(PrefabPerfect, SpawActionPlayer1);
                NetworkServer.Spawn(action);
                Destroy(action, 1f);
                break;
            case 2:
                action = Instantiate(PrefabPerfect, SpawActionPlayer2);
                NetworkServer.Spawn(action);
                Destroy(action, 1f);
                break;
        }
    }
}
