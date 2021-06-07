using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckCard : MonoBehaviour
{

    public int cardID;
    public string cardName;
    public int count;
    public Card card;

    public void UpdateDeckCard()
    {
        if(count <= 0)
        {
            Destroy(gameObject);
            return;
        }
        cardName = CardIDs.cards[cardID];
        gameObject.name = cardName;
        transform.Find("Name").GetComponent<TextMeshProUGUI>().text = cardName;
        transform.Find("Count").GetComponent<TextMeshProUGUI>().text = count.ToString();
        if (CardList.cardDict.ContainsKey(cardName))
        {
            card = CardList.cardDict[cardName].GetComponent<Card>();
            gameObject.GetComponent<Image>().color = Card.colorDict[card.color];
        }
    }

    public void IncreaseCount()
    {
        if(count < 3)
        {
            count += 1;
            UpdateDeckCard();
        }
        
    }
    public void DecreaseCount()
    {
        count -= 1;
        UpdateDeckCard();
    }

}
