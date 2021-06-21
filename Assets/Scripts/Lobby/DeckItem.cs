using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckItem : MonoBehaviour
{ 
    public int itemID;
    public string itemName;
    public int count;
    public Card item;

    private LobbyManager lobbyManager;
    private void Start()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
    }

    public void UpdateDeckItem()
    {
        lobbyManager?.UnReady();
        if (count <= 0)
        {
            Destroy(gameObject);
            return;
        }
        itemName = CardIDs.cards[itemID];
        gameObject.name = itemName;
        transform.Find("Name").GetComponent<TextMeshProUGUI>().text = itemName;
        transform.Find("Count").GetComponent<TextMeshProUGUI>().text = count.ToString();
        if (CardList.itemDict.ContainsKey(itemName))
        {
            item = CardList.itemDict[itemName].GetComponent<Card>();
            gameObject.GetComponent<Image>().color = Card.colorDict[item.color];
        }
    }

    public void IncreaseCount()
    {
        if (count < 2)
        {
            count += 1;
            UpdateDeckItem();
        }

    }
    public void DecreaseCount()
    {
        count -= 1;
        UpdateDeckItem();
    }

    private void OnDestroy()
    {
        UnityEngine.UI.LayoutRebuilder.MarkLayoutForRebuild(transform.parent.parent.GetComponent<RectTransform>());
    }

}
