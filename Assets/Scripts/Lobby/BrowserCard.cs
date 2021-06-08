using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BrowserCard : MonoBehaviour
{
    public int cardID;
    public string cardName;
    public Card card;
    LobbyManager lobbyManager;

    private void Start()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
    }

    public void UpdateBrowserCard()
    {
        cardID = card.ID;
        cardName = card.name;
        gameObject.name = cardName;
        transform.Find("Name").GetComponent<TextMeshProUGUI>().text = cardName;
        gameObject.GetComponent<Image>().color = Card.colorDict[card.color];
    }

    public void addCard()
    {
        if(card is Hero)
        {
            lobbyManager.AddHerotoDeck(cardID);
        }
        else
        {
            lobbyManager.AddCardtoDeck(cardID);
        }
    }
}
