using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fissure : ActiveTargetAbility
{
    // Active 3, 1 Mana: Bounce allies from Earthshaker to a slot in this lane within range 3, then summon Rocks in their place.
    public override bool IsVaildTarget(GameObject target)
    {
        return base.IsVaildTarget(target) &&
            target.transform.GetSiblingIndex() != card.GetCardSlot().transform.GetSiblingIndex()  &&
            (Mathf.Abs(target.transform.GetSiblingIndex() - card.GetCardSlot().transform.GetSiblingIndex()) <= 3);
    }

    public override void OnActivate(List<GameObject> targets)
    {
        base.OnActivate(targets);

        int increment = card.GetCardSlot().transform.GetSiblingIndex() > targets[0].transform.GetSiblingIndex() ? -1 : 1;

        for (int i = card.GetCardSlot().transform.GetSiblingIndex(); i != targets[0].transform.GetSiblingIndex(); i += increment)
        {
            Transform cardSlot = card.GetCardSlot().transform.parent.GetChild(i + increment);
            if (card.hasAuthority)
            {
                card.PlayerManager.CmdSummon("Rock", Card.GetLineage(cardSlot));
            }
        }

    }
}
