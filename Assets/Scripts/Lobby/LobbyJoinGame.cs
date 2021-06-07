using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class LobbyJoinGame : MonoBehaviour
{
    public SteamLobby steamLobby;
    public CSteamID lobbyID;

    public void Join()
    {
        steamLobby.JoinLobby(lobbyID);
    }
}
