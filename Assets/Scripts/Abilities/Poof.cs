using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Poof : ActiveTargetAbility
{
    //Active 2, 1 Mana: Move Meepo to a slot in any allied Meepo's lane
    // Deal 2 damage to the new adjacent enemies. Cross Lane
    //Quickcast
    public override bool IsVaildTarget(GameObject target)
    {
        if (card.rooted) { return false; }
       
        return base.IsVaildTarget(target) && 
            target.transform.parent.GetComponentInChildren<UnitedWeFall>() != null;
    }

    public override void OnActivate(List<GameObject> targets)
    {
        base.OnActivate(targets);
        card.transform.SetParent(targets[0].transform, false);
    }
}
