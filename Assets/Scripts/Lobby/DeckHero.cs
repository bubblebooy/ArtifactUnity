using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckHero : MonoBehaviour
{
    public int cardID;
    public string heroName;
    public Hero hero;

    public void UpdateDeckCard()
    {
        heroName = CardIDs.cards[cardID];
        gameObject.name = heroName;
        transform.Find("Name").GetComponent<TextMeshProUGUI>().text = heroName;
        if (CardList.heroesDict.ContainsKey(heroName))
        {
            hero = CardList.cardDict[heroName].GetComponent<Hero>();
            gameObject.GetComponent<Image>().color = Card.colorDict[hero.color];
        }
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}
