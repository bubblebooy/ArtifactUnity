using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReapersScythe : ActiveTargetAbility
{
    //Active 2, 1 Mana: Deal damage to a unit equal to its missing health.
    public override void OnActivate(List<GameObject> targets)
    {
        base.OnActivate(targets);
        Unit target = targets[0].GetComponent<Unit>();
        int missingHealth = Mathf.Max(0, target.maxHealth - target.health);
        target.Damage(card, missingHealth, piercing: true);
    }
}
