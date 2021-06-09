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
        if (CardList.heroDict.ContainsKey(heroName))
        {
            hero = CardList.heroDict[heroName].GetComponent<Hero>();
            gameObject.GetComponent<Image>().color = Card.colorDict[hero.color];
        }
    }

    public void Up()
    {
        int sibIndex = transform.GetSiblingIndex();
        if(sibIndex > 0)
        {
            transform.SetSiblingIndex(sibIndex - 1);
        }
    }
    public void Down()
    {
        int sibIndex = transform.GetSiblingIndex();
        transform.SetSiblingIndex(sibIndex + 1);
    }

    public void Remove()
    {
        //remove sig cards
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        UnityEngine.UI.LayoutRebuilder.MarkLayoutForRebuild(transform.parent.parent.GetComponent<RectTransform>());
    }
}
