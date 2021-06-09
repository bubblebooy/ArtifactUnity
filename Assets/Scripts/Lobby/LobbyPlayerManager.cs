using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Linq;

public class LobbyPlayerManager : NetworkBehaviour
{
    public GameObject playerManagerPrefab;
    public GameObject playerManager;
    LobbyManager lobbyManager;
    SteamLobby steamLobby;

    private void Start()
    {
        steamLobby = FindObjectOfType<SteamLobby>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (hasAuthority)
        {
            GameObject enemyManager = FindObjectOfType<PlayerManager>()?.gameObject;
            if (enemyManager != null)
            {
                enemyManager.name = "EnemyManager";
            }
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

        if(steamLobby?.useFizzySteamworks == true)
        {
            steamLobby.OnStopClient();
        }
    }
   

    [Command]
    public void CmdReady(List<string> heroList, List<string> cardList)
    {
        RpcReady(heroList, cardList);
    }
    [ClientRpc]
    public void RpcReady(List<string> heroList, List<string> cardList)
    {
        if (hasAuthority)
        {
            lobbyManager.playerReady = true;
            lobbyManager.playerReadyToggle.Ready(true);
            playerManager.GetComponent<PlayerManager>().heroes = heroList.Select(name => CardList.heroDict[name]).ToList();
            playerManager.GetComponent<PlayerManager>().deck = cardList.Select(name => CardList.cardDict[name]).ToList();
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