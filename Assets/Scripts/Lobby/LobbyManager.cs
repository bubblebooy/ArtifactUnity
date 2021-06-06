using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using ArtifactDeckCodeDotNet;

public class LobbyManager : MonoBehaviour
{
    public Button readyButton;
    public Button startButton;
    public GameObject status;

    public ReadyToggle playerReadyToggle;
    public ReadyToggle enemyReadyToggle;

    public bool playerReady = false;
    public bool enemyReady = false;

    public InputField inputDeckCode;
    public Text textDeck;

    public void StartGame()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        if(networkIdentity.GetComponent<PlayerManager>() == null)
        {
            networkIdentity.GetComponent<LobbyPlayerManager>().ChangePlayerManager();
        }
        else
        {
            NetworkManager networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            networkManager.ServerChangeScene("Main");
        }
    }

    public void ToggleReady()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        if (!playerReady)
        {
            networkIdentity.GetComponent<LobbyPlayerManager>().CmdReady();
        }
        else
        {
            networkIdentity.GetComponent<LobbyPlayerManager>().CmdUnReady();
        }

    }

    public void DecodeDeck(string s)
    {
        //RTFACTIlAPLbkCZwFUZXN0RGVjb2RlckRlY2s_
        //deck.text = inputDeckCode.text;
        Deck deck = ArtifactDeckDecoder.ParseDeck(inputDeckCode.text);
        textDeck.text = deck.Name;
        textDeck.text += "\n Heros : ";
        textDeck.text += deck.Heroes.Count;
        foreach(HeroRef hero in deck.Heroes)
        {
            textDeck.text += $"\n{CardIDs.cards[hero.Id]} Turn: {hero.Turn}";
        }
        textDeck.text += "\n Cards : ";
        //sig cards
        foreach (CardRef card in deck.Cards)
        {
            textDeck.text += $"\n{CardIDs.cards[card.Id]} Count: {card.Count}";
        }
    }
}
