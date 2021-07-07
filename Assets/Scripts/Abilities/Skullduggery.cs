using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skullduggery : ActiveTargetAbility
{
    // Active 2, 1 Mana: Discard a non-item card in either player's hand.
    //That player draws another card, and it is revealed to you.
    //(drawn card skips the overdraw pile)

    public override bool IsVaildTarget(GameObject target)
    {
        return base.IsVaildTarget(target) && target.GetComponent<IItem>() == null;
    }

    public override void OnActivate(List<GameObject> targets)
    {
        base.OnActivate(targets);

        Transform hand = targets[0].transform.parent;
        Transform deck = (hand.name == "PlayerArea" ? GameObject.Find("PlayerDeck") : GameObject.Find("EnemyDeck")).transform;

        targets[0].GetComponent<Card>().revealed = true;
        targets[0].GetComponent<Card>().DestroyCard();

        if (deck.childCount == 0) { return; }
        GameObject drawnCard = deck.GetChild(deck.childCount - 1).gameObject;
        drawnCard.transform.SetParent(hand, false);
        drawnCard.transform.rotation = Quaternion.identity;

        if((deck.name == "EnemyDeck") == card.hasAuthority)
        {
            drawnCard.GetComponent<Card>().Reveal(revealedByOpponent: true);
        }

    }
}
