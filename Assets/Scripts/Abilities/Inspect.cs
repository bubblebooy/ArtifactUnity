using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inspect : ActiveAbility
{
    // Active 2, 1 Mana: Reveal two random cards in your opponents hand
    //Increase their cost by 1. Reveal them.
    //Quickcast
    public override void OnActivate()
    {
        base.OnActivate();
        Card[] hand = (card.hasAuthority ? GameObject.Find("EnemyArea") : GameObject.Find("PlayerArea"))
            .GetComponentsInChildren<Card>()
            .Where(card => !(card is IItem) && !card.revealed)
            .OrderBy(_ => Random.value).ToArray();
        for(int i = 0; i < 2 && i < hand.Length; i++)
        {
            hand[i].Reveal(true);
            hand[i].mana += 1;
        }
    }
}
