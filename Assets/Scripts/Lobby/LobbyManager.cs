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
    private GameObject deckItemPrefab;
    [SerializeField]
    private GameObject deckAreaHeros;
    [SerializeField]
    private GameObject deckAreaCards;
    [SerializeField]
    private GameObject deckAreaItems;
    [SerializeField]
    private GameObject browserCardPrefab;
    [SerializeField]
    private GameObject browserArea;

    public bool debug = false;

    private void Start()
    {
        StartCoroutine(PopulateDeckBrowser());
        inputDeckCode.text = PlayerPrefs.GetString("DeckCode");
        GameObject.Find("Deck").GetComponent<LayoutElement>().minHeight = (GameObject.Find("LobbyCanvas").transform as RectTransform).sizeDelta.y;
    }

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
        List<string> heroList = HeroList();
        List<string> deckList = DeckList();
        List<string> itemList = ItemList();
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        if (!playerReady)
        {
            networkIdentity.GetComponent<LobbyPlayerManager>().CmdReady(heroList, deckList, itemList);
        }
        else
        {
            networkIdentity.GetComponent<LobbyPlayerManager>().CmdUnReady();
        }
        StartCoroutine(RebuildLayout());
    }
    public void UnReady()
    {
        if (playerReady)
        {
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            networkIdentity.GetComponent<LobbyPlayerManager>().CmdUnReady();
        }
    }

    public void EncodeDeck()
    {
        Deck deck = new Deck();
        deck.Name = "Artifact Unity";
        deck.Heroes = new List<HeroRef>();
        deck.Cards = new List<CardRef>();
        int turn = -1;
        foreach (DeckHero deckHero in deckAreaHeros.GetComponentsInChildren<DeckHero>())
        {
            HeroRef hero = new HeroRef();
            hero.Id = deckHero.cardID;
            hero.Turn = Mathf.Max(1, turn);
            deck.Heroes.Add(hero);
            turn += 1;
        }
        //deal with sig cards
        foreach (DeckCard deckCard in deckAreaCards.GetComponentsInChildren<DeckCard>())
        {
            CardRef card = new CardRef();
            card.Id = deckCard.cardID;
            card.Count = deckCard.count;
            deck.Cards.Add(card);
        }
        foreach (DeckItem deckItem in deckAreaItems.GetComponentsInChildren<DeckItem>())
        {
            CardRef card = new CardRef();
            card.Id = deckItem.itemID;
            card.Count = deckItem.count;
            deck.Cards.Add(card);
        }
        string deckCode = ArtifactDeckEncoder.EncodeDeck(deck);
        print(deckCode);
        GUIUtility.systemCopyBuffer = deckCode;
        inputDeckCode.text = deckCode;
    }

    public void DecodeDeck(string s)
    {
        //RTFACTJWES9LgCBJpoAfYSBS8Efb4CAViyAoE5AlB0BrAFagSRgwEgArkBiCNEQ0dfUGxheXRlc3REZWNrNg__
        //RTFACTJh4OJLkCDE+BxgQgHwEkuwIBEgMEEAEBIAEPCAo3AhkEAgEOAQoKBDsBBygJJwE7BAE5ARsJBRshAj66FUFydGlmYWN0IFVuaXR5
        foreach (Transform t in deckAreaHeros.transform){ Destroy(t.gameObject); }
        foreach (Transform t in deckAreaCards.transform) { Destroy(t.gameObject); }
        foreach (Transform t in deckAreaItems.transform) { Destroy(t.gameObject); }
        if (inputDeckCode.text.Length < "RTFACT".Length || inputDeckCode.text.Substring(0, "RTFACT".Length) != "RTFACT")
        {
            StartCoroutine(RebuildLayout());
            return;
        }
        PlayerPrefs.SetString("DeckCode", inputDeckCode.text);
        PlayerPrefs.Save();
        Deck deck = ArtifactDeckDecoder.ParseDeck(inputDeckCode.text);
        foreach(HeroRef deckHero in deck.Heroes.OrderBy(x => x.Turn))
        {
            DeckHero hero = Instantiate(deckHeroPrefab, deckAreaHeros.transform).GetComponent<DeckHero>();
            hero.cardID = deckHero.Id;
            hero.UpdateDeckCard();
        }
        //sig cards
        foreach (CardRef deckCard in deck.Cards)
        {
            string name = CardIDs.cards[deckCard.Id];
            if (CardList.itemDict.ContainsKey(name))
            {
                DeckItem item = Instantiate(deckItemPrefab, deckAreaItems.transform).GetComponent<DeckItem>();
                item.itemID = deckCard.Id;
                item.count = deckCard.Count;
                item.UpdateDeckItem();
            }
            else
            {
                DeckCard card = Instantiate(deckCardPrefab, deckAreaCards.transform).GetComponent<DeckCard>();
                card.cardID = deckCard.Id;
                card.count = deckCard.Count;
                card.UpdateDeckCard();
            }

        }
        StartCoroutine(RebuildLayout());
    }

    public List<string> DeckList()
    {
        List<string> deckList = new List<string>();
        foreach (DeckCard deckCard in deckAreaCards.GetComponentsInChildren<DeckCard>())
        {
            if (!CardList.cardDict.ContainsKey(deckCard.cardName))
            {
                Destroy(deckCard.gameObject);
                continue;
            }
            for (int i = 0; i < deckCard.count; i++)
            {
                deckList.Add(deckCard.cardName);
            }
        }
        return deckList;
    }
    public List<string> HeroList()
    {
        List<string> heroList = new List<string>();
        foreach (DeckHero deckHero in deckAreaHeros.GetComponentsInChildren<DeckHero>())
        {
            if (!CardList.heroDict.ContainsKey(deckHero.heroName))
            {
                Destroy(deckHero.gameObject);
                continue;
            }
            heroList.Add(deckHero.heroName);
        }
        return heroList;
    }
    public List<string> ItemList()
    {
        List<string> itemList = new List<string>();
        foreach (DeckItem deckItem in deckAreaItems.GetComponentsInChildren<DeckItem>())
        {
            if (!CardList.itemDict.ContainsKey(deckItem.itemName))
            {
                Destroy(deckItem.gameObject);
                continue;
            }
            for (int i = 0; i < deckItem.count; i++)
            {
                itemList.Add(deckItem.itemName);
            }
        }
        return itemList;
    }

    IEnumerator RebuildLayout()
    {
        for(int i = 0; i < 2; i++)
        {
            UnityEngine.UI.LayoutRebuilder.MarkLayoutForRebuild(deckAreaCards.transform.parent.GetComponent<RectTransform>());
            yield return null;
        }
    }

    public void AddHerotoDeck(int heroID)
    {
        DeckHero hero = Instantiate(deckHeroPrefab, deckAreaHeros.transform).GetComponent<DeckHero>();
        hero.cardID = heroID;
        hero.UpdateDeckCard();
        //sig cards
        StartCoroutine(RebuildLayout());
    }
    public void AddCardtoDeck(int cardID)
    {
        DeckCard[] deckCards = deckAreaCards.GetComponentsInChildren<DeckCard>();
        DeckCard card = deckCards.Where(c => c.cardID == cardID).FirstOrDefault();
        if(card == null)
        {
            card = Instantiate(deckCardPrefab, deckAreaCards.transform).GetComponent<DeckCard>();
            card.cardID = cardID;
        }
        card.IncreaseCount();
        card.UpdateDeckCard();
        StartCoroutine(RebuildLayout());
    }
    public void AddItemtoDeck(int itemID)
    {
        DeckItem[] deckItems = deckAreaCards.GetComponentsInChildren<DeckItem>();
        DeckItem item = deckItems.Where(c => c.itemID == itemID).FirstOrDefault();
        if (item == null)
        {
            item = Instantiate(deckItemPrefab, deckAreaItems.transform).GetComponent<DeckItem>();
            item.itemID = itemID;
        }
        item.IncreaseCount();
        item.UpdateDeckItem();
        StartCoroutine(RebuildLayout());
    }


    IEnumerator PopulateDeckBrowser()
    {
        do
        {
            yield return new WaitForSeconds(0.1f);
        } while (CardList.cardDict == null);

        foreach (GameObject card in CardList.heroDict.Values.Concat(
                                    CardList.cardDict.Values).Concat(
                                    CardList.itemDict.Values))
        {
            BrowserCard browserCard = Instantiate(browserCardPrefab, browserArea.transform).GetComponent<BrowserCard>();
            browserCard.card = card.GetComponent<Card>();
            browserCard.UpdateBrowserCard();
            browserCard.addCard();
        }
    }
}
