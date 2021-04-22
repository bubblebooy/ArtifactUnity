using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhantomAssassin : Hero // Unit : Card
{

    public override void Strike(Unit target, int damage)
    {
        if (target is Hero)
        {
            target.Damage(2 * damage);
        }
        else
        {
            target.Damage( damage);
        }
            
    }

}
