using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentOfCourage : Ability
{
    //Whenever Legion Commander strikes a unit and survives, heal Legion Commander 3.
    bool striked = false;

    protected override void Awake()
    {
        base.Awake();
        card.GetComponentInParent<Unit>().StrikeUnitEvent += StrikeUnit;
    }

    void StrikeUnit(Unit target, ref int damage, ref bool piercing)
    {
        striked = true;
    }

    public override void CardUpdate()
    {
        if (striked)
        {
            if(card.health > 0)
            {
                card.Heal(3);
            }
            striked = false;
        }
    }
}
