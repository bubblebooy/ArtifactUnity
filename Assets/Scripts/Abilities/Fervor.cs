using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fervor : ChargesAbility
{
    //When Troll Warlord strikes a new target, set charges to 1. 
    //Whenever Troll Warlord strikes that target again, add a charge. 
    //Troll Warlord deals +1 damage per charge when striking that target. 
    //Death Effect: Remove all charges.
    Component lastTarget;

    protected override void Awake()
    {
        base.Awake();
        card.StrikeUnitEvent += StrikeUnit;
        card.StrikeTowerEvent += StrikeTower;
    }

    public void StrikeUnit(Unit target, ref int damage, ref bool piercing)
    {
        if (target == lastTarget)
        {
            damage = damage + charges;
            charges += 1;
        }
        else
        {
            charges = 1;
        }
        lastTarget = target;
    }

    public void StrikeTower(TowerManager target, ref int damage, ref bool piercing)
    {
        if (target == lastTarget)
        {
            damage = damage + charges;
            charges += 1;
        }
        else
        {
            charges = 1;
        }
        lastTarget = target;
    }

    public override void OnKilled()
    {
        charges = 0;
    }
}
