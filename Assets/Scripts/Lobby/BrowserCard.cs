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

    public void UpdateBrowserCard()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
        cardID = card.ID;
        cardName = card.cardName;
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
        else if (card is IItem)
        {
            lobbyManager.AddItemtoDeck(cardID);
        }
        else
        {
            lobbyManager.AddCardtoDeck(cardID);
        }
    }
}
