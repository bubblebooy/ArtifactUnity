using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhantomAssassin : Hero // Unit : Card
{

    public override void Strike(Unit target, int damage, bool piercing = false)
    {
        if (target is Hero)
        {
            base.Strike(target, 2 * damage, piercing);
            //target.Damage(2 * damage);
        }
        else
        {
            base.Strike(target, damage, piercing);
        }
            
    }

}
