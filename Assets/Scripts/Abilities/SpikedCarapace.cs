using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikedCarapace : Ability
{
    //Choose caster. When taking damage this round, reflect damage back,
    //stun its source until its next combat,
    //then purge enemy effects from this unit.
    public int duration = 0;

    protected override void Awake()
    {
        base.Awake();
        card.GetComponentInParent<Unit>().DamageEvent += Damage;
        events.Add(GameEventSystem.Register<RoundStart_e>(RoundStart));
    }

    protected override void OnDestroy()
    {
        if (card.GetComponentInParent<Unit>() != null)
        {
            card.GetComponentInParent<Unit>().DamageEvent -= Damage;
        }
        base.OnDestroy();
    }

    void Damage(Unit sourceUnit, ref int damage, bool piercing, bool physical)
    {
        if (sourceUnit != null)
        {
            sourceUnit.Damage(null, damage, piercing, physical);
            UnitModifier spikedCarapaceStun = sourceUnit.gameObject.AddComponent<UnitModifier>() as UnitModifier;
            spikedCarapaceStun.stun = true;
            if (card.GameManager.GameState == "Combat" || card.GameManager.GameState == "Shop")
            {
                spikedCarapaceStun.duration = 2;
            }
        }
        damage = 0;
        card.Purge(oppenentEffectsOnly: true, triggerAuthority: card.hasAuthority);
    }

    public virtual void RoundStart(RoundStart_e e)
    {
        duration--;
        if (temporary && duration <= 0)
        {
            Destroy(this);
        }
    }
}
