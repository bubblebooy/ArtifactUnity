using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerosCape : Item
{
    public override void OnPlay(CardPlayed_e cardPlayed_e)
    {
        base.OnPlay(cardPlayed_e);
        Unit target = transform.GetComponentInParent<Unit>();
        target.health = target.maxHealth;
    }
}
