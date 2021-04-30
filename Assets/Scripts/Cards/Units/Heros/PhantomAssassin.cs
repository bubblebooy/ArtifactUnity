using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhantomAssassin : Hero // Unit : Card
{

    public override int Strike(Unit target, int damage, bool piercing = false)
    {
        EfficientKiller EfficientKiller = GetComponent<AbilitiesManager>().abilities.GetComponentInChildren<EfficientKiller>();
        if (!EfficientKiller.broken) // check if broken here 
        {
            return EfficientKiller.Strike(target, damage, piercing);
        }
        else
        {
            return base.Strike(target, damage, piercing);
        }

    }


    public int baseStrike(Unit target, int damage, bool piercing = false)
    {
        return base.Strike(target, damage, piercing);
    }
}
