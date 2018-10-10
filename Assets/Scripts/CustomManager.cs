using UnityEngine;

using UnityEngine.Networking;

using UnityEngine.Networking.Match;

using UnityEngine.UI;

public class CustomManager : NetworkManager
{
    // Server callbacks
    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("A client connected to the server: " + conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        NetworkServer.DestroyPlayersForConnection(conn);

        if (conn.lastError != NetworkError.Ok)
        {
            if (LogFilter.logError) { Debug.LogError("ServerDisconnected due to error: " + conn.lastError); }
        }

        PanelTimeout.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Debug.Log("A client disconnected from the server: " + conn);
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        NetworkServer.SetClientReady(conn);
        Debug.Log("Client is set to the ready state (ready to receive state updates): " + conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        var player = (GameObject)GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        player.GetComponent<PlayerController>().SetIdPlayer(playerControllerId);

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

        GameObject.Find("GameCore").GetComponent<GameCore>().CmdAddPlayer(player);

        Debug.Log("Client has requested to get his player added to the game with Id: " + playerControllerId);
    }

    //public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    //{

    //    if (player.gameObject != null)

    //        NetworkServer.Destroy(player.gameObject);

    //}

    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("Server network error occurred: " + (NetworkError)errorCode);
    }

    public override void OnStartHost()
    {
        Debug.Log("Host has started");
    }

    public override void OnStartServer()
    {
        Debug.Log("Server has started");
    }

    public override void OnStopServer()
    {
        Debug.Log("Server has stopped");
    }

    public override void OnStopHost()
    {
        Debug.Log("Host has stopped");
    }

    // Client callbacks

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        Debug.Log("Connected successfully to server, now to set up other stuff for the client...");
    }


    public GameObject PanelTimeout;

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        PanelTimeout.SetActive(true);
        //StopClient();

        if (conn.lastError != NetworkError.Ok)

        {

            if (LogFilter.logError) { Debug.LogError("ClientDisconnected due to error: " + conn.lastError); }

        }
    
        Debug.Log("Client disconnected from server: " + conn);
    }

    //For a Button ErrorTimeout
    public void ReturnMenu()
    {
        PanelTimeout.SetActive(false);
        GameObject.Find("Goal_0").GetComponent<SpriteRenderer>().color = Color.gray;
        GameObject.Find("Goal_1").GetComponent<SpriteRenderer>().color = Color.gray;
        GameObject.Find("TextPoint_Player1").GetComponent<Text>().text = "0";
        GameObject.Find("TextPoint_Player2").GetComponent<Text>().text = "0";
        Shutdown();
    }

    public void EndGame()
    {
        GameObject.Find("PanelScore").SetActive(false);
        GameObject.Find("Goal_0").GetComponent<SpriteRenderer>().color = Color.gray;
        GameObject.Find("Goal_1").GetComponent<SpriteRenderer>().color = Color.gray;
        GameObject.Find("TextPoint_Player1").GetComponent<Text>().text = "0";
        GameObject.Find("TextPoint_Player2").GetComponent<Text>().text = "0";
        Shutdown();
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("Client network error occurred: " + (NetworkError)errorCode);
    }

    public override void OnClientNotReady(NetworkConnection conn)
    {
        Debug.Log("Server has set client to be not-ready (stop getting state updates)");
    }

    public override void OnStartClient(NetworkClient client)
    {
        Debug.Log("Client has started");
    }

    public override void OnStopClient()
    {
        Debug.Log("Client has stopped");
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);

        Debug.Log("Server triggered scene change and we've done the same, do any extra work here for the client...");
    }

}
