using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromastery : ChargesAbility
{
    //Shadow Fiend has +1 per charge. Whenever an adjacent enemy dies, add 1 charge
    // Death Effect: Halve the charges on Necromastery.
    public ModifierAbilitySecondary modifierAbilitySecondary;

    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<UnitKilled_e>(UnitKilled));
    }

    public override void CardUpdate()
    {
        charges = modifierAbilitySecondary.attack;
        base.CardUpdate();
    }

    void UnitKilled(UnitKilled_e e)
    {
        Unit unitKilled = e.card as Unit;
        if (unitKilled.hasAuthority != card.hasAuthority &&
            unitKilled.GetLane() == card.GetLane())
        {
            if( System.Array.Exists(unitKilled.GetAdjacentEnemies(), unit => unit == card))
            {
                modifierAbilitySecondary.attack += 1;
            }
        }
    }

    public override void OnKilled()
    {
        modifierAbilitySecondary.attack = System.Convert.ToInt32(Mathf.Ceil(modifierAbilitySecondary.attack / 2f));
    }
}
