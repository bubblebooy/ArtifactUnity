using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sacrifice : ActiveTargetAbility
{
    // Active 2, 1 Mana: Slay another ally and draw a card
    // Restore 2 Mana.

    public override void OnActivate(List<GameObject> targets)
    {
        base.OnActivate(targets);
        targets[0].GetComponent<Unit>().DestroyCard();
        if (card.hasAuthority) { PlayerManager.CmdDrawCards(1); }
        card.ManaManager.RestoreMana(2, card.GetLane());
    }
}
