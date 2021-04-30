using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EfficientKiller : Ability
{
    PhantomAssassin PhantomAssassin;

    protected override void Awake()
    {
        base.Awake();
        PhantomAssassin = (PhantomAssassin)card;
    }

    public int Strike(Unit target, int damage, bool piercing = false)
    {
        if (target is Hero)
        {
            return PhantomAssassin.baseStrike(target, 2 * damage, piercing);
            //target.Damage(2 * damage);
        }
        else
        {
            return PhantomAssassin.baseStrike(target, damage, piercing);
        }

    }
}
