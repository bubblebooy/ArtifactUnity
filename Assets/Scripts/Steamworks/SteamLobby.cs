using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Steamworks;
using Mirror;

public class SteamLobby : MonoBehaviour
{
    [SerializeField]
    private bool useFizzySteamworks = false;
    [SerializeField]
    private GameObject buttonCanvas = null;
    [SerializeField]
    private Transport fizzySteamworks;
    [SerializeField]
    private Transport defaultTransport;



    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEnter;

    private const string HostAddressKey = "HostAddress";

    private NetworkManager networkManager;

    [ExecuteInEditMode]
    #if UNITY_EDITOR
    private void OnValidate()
    {
        networkManager = GetComponent<NetworkManager>();
        SerializedObject serializedNetworkManager = new SerializedObject(networkManager);
        //print(serializedNetworkManager.FindProperty("transport").objectReferenceValue);

        if (useFizzySteamworks)
        {
            buttonCanvas.SetActive(true);

            fizzySteamworks.enabled = true;
            defaultTransport.enabled = false;
            serializedNetworkManager.FindProperty("transport").objectReferenceValue = fizzySteamworks;

            enabled = true;
        }
        else
        {
            buttonCanvas.SetActive(false);

            fizzySteamworks.enabled = false;
            defaultTransport.enabled = true;
            serializedNetworkManager.FindProperty("transport").objectReferenceValue = defaultTransport;

            enabled = false;
        }
        serializedNetworkManager.ApplyModifiedProperties();
        //if (Transport.activeTransport != useFizzySteamworks ? fizzySteamworks : defaultTransport)
        //{
        //    Debug.Log("Please set the Transport to " + (useFizzySteamworks ? fizzySteamworks : defaultTransport));
        //}
    }
    #endif


    private void Start()
    {
        networkManager = GetComponent < NetworkManager>();

        if (!SteamManager.Initialized) { return; }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    public void HostLobby()
    {
        buttonCanvas.SetActive(false);

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            buttonCanvas.SetActive(true);
            return;
        }

        networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey,
            SteamUser.GetSteamID().ToString());

        SteamFriends.ActivateGameOverlayInviteDialog(new CSteamID(callback.m_ulSteamIDLobby));
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        if(NetworkServer.active) { return;  }

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey);

        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();

        buttonCanvas.SetActive(false);

    }


}