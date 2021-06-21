using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System.Linq;

public class ShopManager : NetworkBehaviour
{
    [HideInInspector]
    public PlayerManager PlayerManager;
    [HideInInspector]
    public GameManager GameManager;
    [HideInInspector]
    public GoldManager GoldManager;

    [SerializeField]
    private Button buttonViewBoard;
    [SerializeField]
    private Button buttonContinue;
    [SerializeField]
    private Button buttonUpgrade;
    [SerializeField]
    private Button buttonTopTier;
    [SerializeField]
    private Button buttonRandomRack;
    [SerializeField]
    private Button buttonBargainBin;

    [SerializeField]
    private GameObject slotTopTier;
    [SerializeField]
    private GameObject slotRandomRack;
    [SerializeField]
    private GameObject slotBargainBin;

    private string itemTopTier;
    private string itemRandomRack;
    private string itemBargainBin;

    private int costTopTier;
    private int costRandomRack;
    private int costBargainBin;

    [SerializeField]
    private TextMeshProUGUI levelDisplay;
    [SerializeField]
    private TextMeshProUGUI goldDisplay;


    public int skipGold = 5;
    public int level = 1;
    private int maxLevel = 5;
    private bool usedShop = false;
    private bool hideShop = false;

    private List<GameObject> items;

    private System.Random rand;

    public override void OnStartClient()
    {
        gameObject.SetActive(false);
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void UpdateShop()
    {
        //Gold Count
        levelDisplay.text = $"Level {level}";
        buttonContinue.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = usedShop ? "Continue" : "+5 Skip";
        goldDisplay.text = GoldManager.gold.ToString();

        buttonTopTier.interactable = GoldManager.gold > costTopTier;
        buttonRandomRack.interactable = GoldManager.gold > costRandomRack;
        buttonBargainBin.interactable = GoldManager.gold > costBargainBin;
    }

    public void StartShopping()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();

        rand = new System.Random(Random.Range(0, System.Int32.MaxValue / 2) + (PlayerManager.isClientOnly ? 0 : System.Int32.MaxValue / 2)); // Random.Range(0, System.Int32.MaxValue) I dont want the seed syncd :-(

        GoldManager = GameObject.Find("PlayerGold").GetComponent<GoldManager>();

        items = PlayerManager.items.Where(item => item.GetComponent<IItem>().level <= level).ToList();

        RestockShop("Top Tier");
        RestockShop("Random Rack");
        RestockShop("Bargain Bin");

        UpdateShop();
    }

    
    public void RestockShop(string shop)
    {
        //https://steamcommunity.com/games/1269260/announcements/detail/2969520947916198805
        //In constructed, no random items are added to your deck.It's just your item deck, 
        //You will get random tier 1 items offered if you run out of items from your item deck.

        //The rules for the shop slots have been simplified slightly as well.
        //The left slot is an item from the highest tier available
        //The right slot is an item from the lowest tier available (previously it was any item you can afford)
        // --ignoring-- item costs can no longer increase as you scan from the left to the right slot.
        // --funtionaly it will be the same since you would roll the items then sort by cost--
        // --what if I want to add high tier low cost items?--
        List<GameObject> _items = items.ToList();
        GameObject item;

        if (items.Count == 0)
        {
            ItemListEmpty(shop);
            return;
        }

        switch (shop)
        {
            case "Top Tier":
                int maxLevel = items.Max(itm => itm.GetComponent<IItem>().level);
                _items = items.Where(itm => itm.GetComponent<IItem>().level == maxLevel).ToList();
                item = _items[rand.Next(_items.Count)];
                itemTopTier = item.name;
                items.Remove(item);
                RestockShop(itemTopTier, slotTopTier, out costTopTier);
                break;
            case "Random Rack":
                item = items[rand.Next(items.Count)];
                itemRandomRack = item.name;
                items.Remove(item);
                RestockShop(itemRandomRack, slotRandomRack, out costRandomRack);
                break;
            case "Bargain Bin":
                int minLevel = items.Min(itm => itm.GetComponent<IItem>().level);
                _items = items.Where(itm => itm.GetComponent<IItem>().level == minLevel).ToList();
                item = _items[rand.Next(_items.Count)];
                itemBargainBin = item.name;
                items.Remove(item);
                RestockShop(itemBargainBin, slotBargainBin, out costBargainBin);
                break;
            default:
                return;
        }
    }

    public void RestockShop(string item, GameObject slot, out int cost)
    {
        GameObject _item;
        GameObject cardFront;

        foreach (Transform transform in slot.transform) { Destroy(transform.gameObject); }

        _item = Instantiate(CardList.itemDict[item]);
        cardFront = _item.transform.Find("CardFront").gameObject;
        cardFront.transform.SetParent(slot.transform);
        (cardFront.transform as RectTransform).localScale = Vector3.one;
        (cardFront.transform as RectTransform).localPosition = Vector3.zero;
        (cardFront.transform as RectTransform).sizeDelta = Vector2.zero;
        cost = _item.GetComponent<IItem>().gold;
        Destroy(_item);
    }

    public void ItemListEmpty(string shop)
    {
        // Add rnd T1 here?
        switch (shop)
        {
            case "Top Tier":
                costTopTier = System.Int32.MaxValue;
                foreach (Transform transform in slotTopTier.transform) { Destroy(transform.gameObject); }
                break;
            case "Random Rack":
                costRandomRack = System.Int32.MaxValue;
                foreach (Transform transform in slotRandomRack.transform) { Destroy(transform.gameObject); }
                break;
            case "Bargain Bin":
                costBargainBin = System.Int32.MaxValue;
                foreach (Transform transform in slotBargainBin.transform) { Destroy(transform.gameObject); }
                break;
            default:
                break;
        }
        return;
    }

    public void FinnishedShopping()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        PlayerManager.CmdFinnishedShopping(usedShop ? 0 : skipGold);
        gameObject.SetActive(false);
        usedShop = false;
        if(level < maxLevel)
        {
            buttonUpgrade.interactable = true;
        }
        foreach (Transform transform in slotTopTier.transform)    { Destroy(transform.gameObject); }
        foreach (Transform transform in slotRandomRack.transform) { Destroy(transform.gameObject); }
        foreach (Transform transform in slotBargainBin.transform) { Destroy(transform.gameObject); }
    }

    public void ShowBoard()
    {
        Vector3 positionViewBoard = buttonViewBoard.transform.position;
        Vector3 positionConintue = buttonContinue.transform.position;
        gameObject.transform.position += (hideShop ? -1 : 1) * new Vector3(Screen.width, 0);
        buttonViewBoard.transform.position = positionViewBoard;
        buttonContinue.transform.position = positionConintue;
        hideShop = !hideShop;
        //gameObject.transform
    }

    public void Upgrade()
    {
        level += 1;
        buttonUpgrade.interactable = false;
        usedShop = true;

        items = PlayerManager.items.Where(item => item.GetComponent<IItem>().level <= level).ToList();

        RestockShop("Top Tier");
        RestockShop("Random Rack");
        RestockShop("Bargain Bin");

        UpdateShop();
    }

    public void Buy(string shop)
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        string item;
        Transform slot = null;
        switch (shop)
        {
            case "Top Tier":
                item = itemTopTier;
                slot = slotTopTier.transform;
                buttonTopTier.interactable = false;
                PlayerManager.items.Remove(CardList.itemDict[item]);
                RestockShop("Top Tier");
                break;
            case "Random Rack":
                item = itemRandomRack;
                slot = slotRandomRack.transform;
                buttonRandomRack.interactable = false;
                PlayerManager.items.Remove(CardList.itemDict[item]);
                RestockShop("Random Rack");
                break;
            case "Bargain Bin":
                item = itemBargainBin;
                slot = slotBargainBin.transform;
                buttonBargainBin.interactable = false;
                PlayerManager.items.Remove(CardList.itemDict[item]);
                RestockShop("Bargain Bin");
                break;
            default:
                return;
        }
        PlayerManager.CmdBuyItem(item);
        usedShop = true;
    }

}
