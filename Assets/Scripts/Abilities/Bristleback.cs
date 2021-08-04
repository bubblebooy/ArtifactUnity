using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bristleback : Ability
{
    protected override void Awake()
    {
        base.Awake();
        card.GetComponentInParent<Unit>().DamageEvent += Damage;
    }

    protected override void OnDestroy()
    {
        if(card.GetComponentInParent<Unit>() != null)
        {
            card.GetComponentInParent<Unit>().DamageEvent -= Damage;
        }
        base.OnDestroy();
    }

    void Damage(Unit sourceUnit, ref int damage, bool piercing, bool physical)
    {
        if (!physical)
        {
            foreach (Unit adjEnemy in card.GetAdjacentEnemies())
            {
                adjEnemy?.Damage(card, 1);
            }
            damage = 1;
        }
    }
}
