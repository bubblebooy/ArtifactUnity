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

    [SerializeField]
    private TextMeshProUGUI levelDisplay;
    [SerializeField]
    private TextMeshProUGUI goldDisplay;

    public int level = 1;
    private int maxLevel = 5;
    private bool usedShop = false;
    private bool hideShop = false;

    public override void OnStartClient()
    {
        gameObject.SetActive(false);
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void UpdateShop()
    {
        //Gold Count
        levelDisplay.text = $"Level {level}";
        buttonContinue.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = usedShop ? "Continue" : "+5 Skip";
    }

    public void StartShopping()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();

        GameObject _item;
        GameObject cardFront;
        itemTopTier = PlayerManager.items[0].name;
        _item = Instantiate(CardList.itemDict[itemTopTier]);
        cardFront = _item.transform.Find("CardFront").gameObject;
        cardFront.transform.SetParent(slotTopTier.transform);
        (cardFront.transform as RectTransform).localScale = Vector3.one;
        (cardFront.transform as RectTransform).localPosition = Vector3.zero;
        (cardFront.transform as RectTransform).sizeDelta = Vector2.zero;
        Destroy(_item);

        itemRandomRack = PlayerManager.items[1].name;
        _item = Instantiate(CardList.itemDict[itemRandomRack]);
        cardFront = _item.transform.Find("CardFront").gameObject;
        cardFront.transform.SetParent(slotRandomRack.transform);
        (cardFront.transform as RectTransform).localScale = Vector3.one;
        (cardFront.transform as RectTransform).localPosition = Vector3.zero;
        (cardFront.transform as RectTransform).sizeDelta = Vector2.zero;
        Destroy(_item);

        itemBargainBin = PlayerManager.items[2].name;
        _item = Instantiate(CardList.itemDict[itemBargainBin]);
        cardFront = _item.transform.Find("CardFront").gameObject;
        cardFront.transform.SetParent(slotBargainBin.transform);
        (cardFront.transform as RectTransform).localScale = Vector3.one;
        (cardFront.transform as RectTransform).localPosition = Vector3.zero;
        (cardFront.transform as RectTransform).sizeDelta = Vector2.zero;
        Destroy(_item);
    }

    public void FinnishedShopping()
    {
        if (usedShop)
        {
            //Get Money Get Paid
        }
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        PlayerManager.CmdFinnishedShopping();
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
                break;
            case "Random Rack":
                item = itemRandomRack;
                slot = slotRandomRack.transform;
                break;
            case "Bargain Bin":
                item = itemBargainBin;
                slot = slotBargainBin.transform;
                break;
            default:
                return;
        }
        foreach (Transform transform in slot) { Destroy(transform.gameObject); }
        PlayerManager.CmdBuyItem(item);

        usedShop = true;
        UpdateShop();
    }

}
