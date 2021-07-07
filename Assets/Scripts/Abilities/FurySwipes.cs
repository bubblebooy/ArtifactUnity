using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FurySwipes : Ability
{
    //Whenever Ursa deals physical damage to a unit, give it -1 permanently.
    protected override void Awake()
    {
        base.Awake();
        //PhantomAssassin = (PhantomAssassin)card;
        card.StrikeUnitEvent += StrikeUnit;
        card.RetaliateEvent += Retaliate;
    }

    void StrikeUnit(Unit target, ref int damage, ref bool piercing)
    {
        ModifyUnit(target);
    }
    void Retaliate(Unit strikingUnit, ref int damage)
    {
        ModifyUnit(strikingUnit);
    }

    void ModifyUnit(Unit unit)
    {
        UnitModifier furySwipes = unit.gameObject.GetComponent<FurySwipesModifier>() ?? unit.gameObject.AddComponent<FurySwipesModifier>() as FurySwipesModifier;
        furySwipes.temporary = false;
        furySwipes.maxArmor += -1;
    }
}
