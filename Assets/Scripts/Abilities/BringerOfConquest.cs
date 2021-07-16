using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BringerOfConquest : Ability
{
    //If you have three empty slots, your lane creeps are all deployed into this lane.
    public override void OnPlay()
    {
        base.OnPlay();
        // Should only have to do this in the EndCombatPhase_e
        // But might move to Auras_e incase there is an errant update I am not considering
        inPlayEvents.Add(GameEventSystem.Register<EndCombatPhase_e>(EndCombatPhase));
    }

    void EndCombatPhase(EndCombatPhase_e e)
    {
        LaneManager lane = card.GetLane();
        int emptySlots = card.PlayerManager.Settings.values.variableSlots ? 3 :
            card.GetLane().transform
            .Find(card.GetSide())
            .Cast<Transform>()
            .Count(slot => slot.GetComponentInChildren<Unit>() == null); // would transform.ChildCount() == 0 work and be faster?

        if (emptySlots >= 3)
        {
            foreach (LaneManager l in card.GameManager.lanes)
            {
                if (card.hasAuthority)
                {
                    l.playerCreepSummonCount = 0;
                }
                else
                {
                    l.enemyCreepSummonCount = 0;
                }
            }
            if (card.hasAuthority)
            {
                lane.playerCreepSummonCount = 3;
            }
            else
            {
                lane.enemyCreepSummonCount = 3;
            }
        }
    }
}
