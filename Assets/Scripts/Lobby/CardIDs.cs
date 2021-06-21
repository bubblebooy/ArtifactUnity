using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardIDs : MonoBehaviour
{
    
    public static Dictionary<int, string > cards;
    [SerializeField]
    public List<cardID> cardList;

    [System.Serializable]
    public struct cardID
    {
        public int ID;
        public string Name;

    }

    private void OnValidate()
    {
        if(cardList.Count == 0)
        {
            GetIdsFromFile();
            GetIdsFromCards();
        }
    }

    private void Start()
    {
        cards = cardList.ToDictionary(cid => cid.ID, cid => cid.Name);
    }

    void GetIdsFromFile()
    {
        string[] lines = System.IO.File.ReadAllLines(@"C:\Users\bubbl\OneDrive\Documents\Unity Projects\Artifact 3.0\Assets\Scripts\Lobby\DeckCode\items_game.txt");
        foreach(string line in lines)
        {
            if (line.Contains("\"name\""))
            {
                string nameid = line.Replace("name", "");
                nameid = nameid.Replace('"', ' ');
                nameid = nameid.Trim();
                cardID id = new cardID();
                id.Name = nameid.Substring(0, nameid.LastIndexOf('-'));
                id.ID = int.Parse(nameid.Substring(nameid.LastIndexOf('-')+1));
                cardList.Add(id);
            }
        }
    }
    void GetIdsFromCards()
    {
        foreach(GameObject card in FindObjectOfType<CardList>().cards)
        {
            Card c = card.GetComponent<Card>();
            if(!cardList.Exists(x => x.ID == c.ID))
            {
                cardID id = new cardID();
                id.Name = c.cardName;
                id.ID = c.ID;
                cardList.Add(id);
            }
        }
        foreach (GameObject card in FindObjectOfType<CardList>().hero)
        {
            Card c = card.GetComponent<Card>();
            if (!cardList.Exists(x => x.ID == c.ID))
            {
                cardID id = new cardID();
                id.Name = c.cardName;
                id.ID = c.ID;
                cardList.Add(id);
            }
        }
        foreach (GameObject card in FindObjectOfType<CardList>().items)
        {
            Card c = card.GetComponent<Card>();
            if (!cardList.Exists(x => x.ID == c.ID))
            {
                cardID id = new cardID();
                id.Name = c.cardName;
                id.ID = c.ID;
                cardList.Add(id);
            }
        }
    }
}
