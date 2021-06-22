using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallOfTheWild : ActiveTargetAbility
{
    //Active 2, Mana 1: Summon a Loyal Beast
    public override void OnActivate(List<GameObject> targets)
    {
        base.OnActivate(targets);

        CardSlot cardSlot = targets[0].GetComponent<CardSlot>();
        if (card.hasAuthority)
        {
            PlayerManager.CmdSummon("Loyal Beast", Card.GetLineage(targets[0].transform));
        }
    }
}
