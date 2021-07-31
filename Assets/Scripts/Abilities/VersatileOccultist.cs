using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VersatileOccultist : ActiveAbility
{
    //Active 2, 1 Mana: Reduce the Mana cost of two random cards in your hand by 1.
    //Change their color to blue.
    //Quickcast

    public override void OnActivate()
    {
        base.OnActivate();
        Card[] hand = (card.hasAuthority ? GameObject.Find("PlayerArea") : GameObject.Find("EnemyArea"))
            .GetComponentsInChildren<Card>()
            .Where(card => !(card is IItem) && card.mana > 0)
            .OrderBy(_ => Random.value).ToArray();
        for (int i = 0; i < 2 && i < hand.Length; i++)
        {
            hand[i].mana -= 1;
            hand[i].color = "blue";
            hand[i].OnValidate();
        }
    }
}
