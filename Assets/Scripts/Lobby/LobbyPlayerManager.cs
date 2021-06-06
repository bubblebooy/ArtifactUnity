using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LobbyPlayerManager : NetworkBehaviour
{
    public GameObject playerManagerPrefab;
    public GameObject playerManager;
    LobbyManager lobbyManager;


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (hasAuthority)
        {
            PlayerManager enemyManager = FindObjectOfType<PlayerManager>();
            if (enemyManager != null){ enemyManager.gameObject.name = "EnemyManager"; }
            CmdSpawnPlayerManager();
        }
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        if (hasAuthority)
        {
            lobbyManager.startButton.gameObject.SetActive(true);
            lobbyManager.readyButton.gameObject.SetActive(true);
        }
        if (hasAuthority)
        {
            lobbyManager.playerReadyToggle.Connected(true);
            if (isClientOnly)
            {
                lobbyManager.enemyReadyToggle.Connected(true);
                lobbyManager.readyButton.interactable = true;
            }
        }
        else
        {
            lobbyManager.enemyReadyToggle.Connected(true);
            lobbyManager.readyButton.interactable = true;
        }

    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (hasAuthority)
        {
            lobbyManager.startButton.gameObject.SetActive(false);
            lobbyManager.readyButton.gameObject.SetActive(false);
        }
        if (hasAuthority)
        {
            lobbyManager.playerReadyToggle.Connected(false);
        }
        else
        {
            lobbyManager.enemyReadyToggle.Connected(false);
        }
        lobbyManager.playerReady = false;
        lobbyManager.playerReadyToggle.Ready(false);
        lobbyManager.enemyReady = false;
        lobbyManager.enemyReadyToggle.Ready(false);
        lobbyManager.readyButton.interactable = false;
        lobbyManager.startButton.interactable = false;
    }
   

    [Command]
    public void CmdReady()
    {
        RpcReady();
    }
    [ClientRpc]
    public void RpcReady()
    {
        if (hasAuthority)
        {
            lobbyManager.playerReady = true;
            lobbyManager.playerReadyToggle.Ready(true);
        }
        else
        {
            lobbyManager.enemyReady = true;
            lobbyManager.enemyReadyToggle.Ready(true);
        }

        lobbyManager.startButton.interactable = lobbyManager.playerReady && lobbyManager.enemyReady;

    }

    [Command]
    public void CmdUnReady()
    {
        RpcUnReady();
    }
    [ClientRpc]
    public void RpcUnReady()
    {
        if (hasAuthority)
        {
            lobbyManager.playerReady = false;
            lobbyManager.playerReadyToggle.Ready(false);
        }
        else
        {
            lobbyManager.enemyReady = false;
            lobbyManager.enemyReadyToggle.Ready(false);
        }
        lobbyManager.startButton.interactable = lobbyManager.playerReady && lobbyManager.enemyReady;
    }


    [Command]
    public void CmdSpawnPlayerManager()
    {
        GameObject manager;
        manager = Instantiate(playerManagerPrefab);
        NetworkServer.Spawn(manager, connectionToClient);
        RpcSpawnPlayerManager(manager);
    }
    [ClientRpc]
    public void RpcSpawnPlayerManager(GameObject manager)
    {
        playerManager = manager;
        manager.name = hasAuthority ? "PlayerManager" : "EnemyManager";
    }

    public void ChangePlayerManager()
    {
        CmdChangePlayerManager();
    }

    [Command]
    public void CmdChangePlayerManager()
    {
        //RpcChangePlayerManager();
        foreach ( NetworkConnection connection in NetworkServer.connections.Values)
        {
            NetworkServer.Destroy(connection.identity.gameObject);
            NetworkServer.ReplacePlayerForConnection(connection,
                connection.identity.GetComponent<LobbyPlayerManager>().playerManager,
                true);
        }
        NetworkManager networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        networkManager.autoCreatePlayer = false;
        networkManager.ServerChangeScene("Main");
    }
}