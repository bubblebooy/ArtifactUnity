using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sven : Hero
{
    public override void Combat(bool quick = false)
    {
        GreatCleave GreatCleave = GetComponent<AbilitiesManager>().abilities.GetComponentInChildren<GreatCleave>();
        if (!GreatCleave.broken) // check if broken here 
        {
            GreatCleave.Combat(quick);
        }
        else
        {
            base.Combat(quick);
        }
        
    }
}
