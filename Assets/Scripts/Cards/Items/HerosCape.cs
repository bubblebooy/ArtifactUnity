using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerosCape : Item
{
    public override void OnPlay()
    {
        base.OnPlay();
        Unit target = transform.GetComponentInParent<Unit>();
        target.health = target.maxHealth;
    }
}
