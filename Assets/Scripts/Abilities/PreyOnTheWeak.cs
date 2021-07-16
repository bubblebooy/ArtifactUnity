using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PreyOnTheWeak : Ability
{
    // Before your turn, summon a Hound of War to devour each allied non-Hound creep.

    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<TurnStart_e>(TurnStart));
    }

    void TurnStart(TurnStart_e e)
    {
        if(card.hasAuthority == card.PlayerManager.IsMyTurn)
        {
            Unit[] creeps = card.GetCardSlot().transform.parent
                .GetComponentsInChildren<Unit>()
                .Where(unit => !(unit is Hero) && unit.cardName != "Hound of War").ToArray();
            foreach (Unit creep in creeps)
            {
                if (card.hasAuthority)
                {
                    card.PlayerManager.CmdSummon("Hound of War", Card.GetLineage(creep.GetCardSlot().transform));
                }
            }
        }
    }

}
