using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempestDouble : ActiveTargetAbility
{
    bool isTheDouble = false;
    int cloneTimer = 2;

    public override void Bounce()
    {
        if (isTheDouble && (card as Hero).respawn == 2) // respawn should be 2 if killed instead of bounced
        {
            (card as Hero).ForceDestroyCard();
        }
    }

    public override void OnActivate(List<GameObject> targets)
    {
        base.OnActivate(targets);

        CardSlot cardSlot = targets[0].GetComponent<CardSlot>();
        if (card.hasAuthority)
        {
            PlayerManager.CmdClone(card.gameObject, Card.GetLineage(targets[0].transform));
        }
    }

    public override void Clone(Ability originalAbility)
    {
        base.Clone(originalAbility);
        isTheDouble = true;
        events.Add(GameEventSystem.Register<EndCombatPhase_e>(CloneExpire));
    }

    void CloneExpire(EndCombatPhase_e e)
    {
        cloneTimer -= 1;
        if(isTheDouble && cloneTimer == 0)
        {
            (card as Hero).ForceDestroyCard();
        }
    }

    public override bool IsVaildPlay()
    {
        return !isTheDouble && base.IsVaildPlay();
    }
}
