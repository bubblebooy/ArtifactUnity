using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Steamworks;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{
    public bool useFizzySteamworks = false;
    [SerializeField]
    private GameObject buttonCanvas = null;
    [SerializeField]
    private GameObject lobbyListPrefab = null;
    private GameObject lobbyList = null;
    [SerializeField]
    private GameObject joinGamePrefab = null;
    [SerializeField]
    private Transport fizzySteamworks;
    [SerializeField]
    private Transport defaultTransport;

    private CSteamID lobbyID;



    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEnter;
    protected Callback<LobbyMatchList_t> lobbyMatchList;

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
        lobbyMatchList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
    }

    public void OnStopClient()
    {
        buttonCanvas.SetActive(true);
        SteamMatchmaking.LeaveLobby(lobbyID);
    }

    public void HostLobby()
    {
        buttonCanvas.SetActive(false);
        
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, networkManager.maxConnections); //k_ELobbyTypeFriendsOnly
    }
    public void JoinLobby(CSteamID lobbyID)
    {
        SteamMatchmaking.JoinLobby(lobbyID);
        buttonCanvas.SetActive(false);
    }

    public void GetLobbiesList()
    {
        SteamMatchmaking.AddRequestLobbyListResultCountFilter(10);
        SteamMatchmaking.AddRequestLobbyListStringFilter("ArtifactGameMode", "Constructed", ELobbyComparison.k_ELobbyComparisonEqual);
        SteamMatchmaking.RequestLobbyList();
    }

    private void OnGetLobbiesList(LobbyMatchList_t callback)
    {
        lobbyList = Instantiate(lobbyListPrefab, buttonCanvas.transform);
        for (int i = 0; i < callback.m_nLobbiesMatching; i++)
        {
            GameObject joinGame = Instantiate(joinGamePrefab, lobbyList.transform);
            joinGame.transform.SetSiblingIndex(joinGame.transform.GetSiblingIndex()-1);
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            joinGame.GetComponent<LobbyJoinGame>().steamLobby = this;
            joinGame.GetComponent<LobbyJoinGame>().lobbyID = lobbyID;
            joinGame.GetComponentInChildren<Text>().text = SteamMatchmaking.GetLobbyData(lobbyID, "HostName");
        }  
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            buttonCanvas.SetActive(true);
            return;
        }

        networkManager.StartHost();
        lobbyID = new CSteamID(callback.m_ulSteamIDLobby);

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey,
            SteamUser.GetSteamID().ToString());

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "ArtifactGameMode",
            "Constructed");

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostName",
            SteamFriends.GetPersonaName());

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
        lobbyID = new  CSteamID(callback.m_ulSteamIDLobby);

    }


}