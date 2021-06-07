using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using ArtifactDeckCodeDotNet;
using System.Linq;

public class LobbyManager : MonoBehaviour
{
    [Header("Ready System")]
    public Button readyButton;
    public Button startButton;
    public GameObject status;

    public ReadyToggle playerReadyToggle;
    public ReadyToggle enemyReadyToggle;

    public bool playerReady = false;
    public bool enemyReady = false;

    [Header("Deck")]
    //private List<GameObject> deck;
    //private List<GameObject> heroes;
    public InputField inputDeckCode;
    [SerializeField]
    private GameObject deckCardPrefab;
    [SerializeField]
    private GameObject deckHeroPrefab;
    [SerializeField]
    private GameObject deckArea;
    //public Text textDeck;

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
        //RTFACTJWES9LgCBJpoAfYSBS8Efb4CAViyAoE5AlB0BrAFagSRgwEgArkBiCNEQ0dfUGxheXRlc3REZWNrNg__
        foreach(Transform t in deckArea.transform)
        {
            if(t.gameObject.name != "InputDeckCode")
            {
                Destroy(t.gameObject);
            }
        }
        if(inputDeckCode.text.Length < "RTFACT".Length || inputDeckCode.text.Substring(0, "RTFACT".Length) != "RTFACT")
        {
            return;
        }
        Deck deck = ArtifactDeckDecoder.ParseDeck(inputDeckCode.text);
        foreach(HeroRef deckHero in deck.Heroes.OrderBy(x => x.Turn))
        {
            DeckHero hero = Instantiate(deckHeroPrefab, deckArea.transform).GetComponent<DeckHero>();
            hero.cardID = deckHero.Id;
            hero.UpdateDeckCard();
        }
        //sig cards
        foreach (CardRef deckCard in deck.Cards)
        {
            DeckCard card = Instantiate(deckCardPrefab, deckArea.transform).GetComponent<DeckCard>();
            card.cardID = deckCard.Id;
            card.count = deckCard.Count;
            card.UpdateDeckCard();
        }
    }
}
