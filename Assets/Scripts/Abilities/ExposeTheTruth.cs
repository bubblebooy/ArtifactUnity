using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ExposeTheTruth : ModifierAbility
{
    //Pierce. Pierpont has +1 per revealed card in opponent's hand.
    public override void CardUpdate()
    {
        Card[] hand = (card.hasAuthority ? GameObject.Find("EnemyArea") : GameObject.Find("PlayerArea"))
            .GetComponentsInChildren<Card>()
            .Where(card => !(card is IItem) && card.revealed).ToArray();
        attack = hand.Length;
    }
}
