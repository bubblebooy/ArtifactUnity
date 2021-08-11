using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sadist : Ability
{
    // Passive Ability
    // Whenever an enemy unit dies, restore 1 mana.
    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<UnitKilled_e>(UnitKilled));
    }

    void UnitKilled(UnitKilled_e e)
    {
        Unit unitKilled = e.card as Unit;
        if (unitKilled.hasAuthority != card.hasAuthority &&
            unitKilled.GetLane() == card.GetLane())
        {
            card.ManaManager.RestoreMana(1, card.GetLane().GetComponent<LaneManager>());
        }
    }
}
