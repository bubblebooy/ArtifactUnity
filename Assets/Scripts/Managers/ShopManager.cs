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

        GoldManager = GameObject.Find("PlayerGold").GetComponent<GoldManager>();

        RestockShop("Top Tier");
        RestockShop("Random Rack");
        RestockShop("Bargain Bin");

        UpdateShop();
    }

    public void RestockShop(string shop)
    {
        switch (shop)
        {
            case "Top Tier":
                itemTopTier = PlayerManager.items[PlayerManager.items.Count-1].name;
                RestockShop(itemTopTier, slotTopTier, out costTopTier);
                break;
            case "Random Rack":
                itemRandomRack = PlayerManager.items[PlayerManager.items.Count - 2].name;
                RestockShop(itemRandomRack, slotRandomRack, out costRandomRack);
                break;
            case "Bargain Bin":
                itemBargainBin = PlayerManager.items[PlayerManager.items.Count - 3].name;
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

        _item = Instantiate(CardList.itemDict[item]);
        cardFront = _item.transform.Find("CardFront").gameObject;
        cardFront.transform.SetParent(slot.transform);
        (cardFront.transform as RectTransform).localScale = Vector3.one;
        (cardFront.transform as RectTransform).localPosition = Vector3.zero;
        (cardFront.transform as RectTransform).sizeDelta = Vector2.zero;
        cost = _item.GetComponent<IItem>().gold;
        Destroy(_item);
    }

    public void FinnishedShopping()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        //if (!usedShop)
        //{
        //    //Get Money Get Paid
        //    GoldManager.gold += skipGold;
        //}
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
                break;
            case "Random Rack":
                item = itemRandomRack;
                slot = slotRandomRack.transform;
                buttonRandomRack.interactable = false;
                break;
            case "Bargain Bin":
                item = itemBargainBin;
                slot = slotBargainBin.transform;
                buttonBargainBin.interactable = false;
                break;
            default:
                return;
        }
        foreach (Transform transform in slot) { Destroy(transform.gameObject); }
        PlayerManager.CmdBuyItem(item);
        usedShop = true;
    }

}
