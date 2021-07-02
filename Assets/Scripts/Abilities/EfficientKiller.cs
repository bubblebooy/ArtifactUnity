using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EfficientKiller : Ability
{
    PhantomAssassin PhantomAssassin;

    protected override void Awake()
    {
        base.Awake();
        //PhantomAssassin = (PhantomAssassin)card;
        card.GetComponentInParent<Unit>().StrikeUnitEvent += StrikeUnit;
    }

    public void StrikeUnit(Unit target, ref int damage, ref bool piercing)
    {
        if (target is Hero)
        {
            damage = damage * 2;
        }

    }
}
