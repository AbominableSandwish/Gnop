using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UIManager : NetworkBehaviour {

    [SerializeField] Transform SpawActionPlayer1;
    [SerializeField] Transform SpawActionPlayer2;

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
                Destroy(action, 1f);
                break;
            case 2:
                action = Instantiate(PrefabGood, SpawActionPlayer2);
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
                Destroy(action, 1f);
                break;
            case 2:
                action = Instantiate(PrefabPerfect, SpawActionPlayer2);
                Destroy(action, 1f);
                break;
        }
    }
}
