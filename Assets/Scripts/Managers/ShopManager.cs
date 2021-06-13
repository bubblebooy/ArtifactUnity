using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

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
        GameObject item;
        item = PlayerManager.items[0];
        //Instantiate(item.transform.Find("CardFront"), slotTopTier.transform);
        //Instantiate(item.transform.Find("CardFront"), slotRandomRack.transform);
        //Instantiate(item.transform.Find("CardFront"), slotBargainBin.transform);
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

    public void Buy()
    {
        usedShop = true;
        UpdateShop();
    }

}
