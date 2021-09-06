using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorrowedTime : ActiveAbility
{
    // Active 2, 1 Mana: Purge opponent's effects.
    // When Abaddon is dealt damage this round, heal that much instead.
    // May be used when stunned or silenced.
    bool activated = false;

    protected override void Awake()
    {
        base.Awake();
        card.GetComponentInParent<Unit>().DamageEvent += Damage;
        events.Add(GameEventSystem.Register<RoundStart_e>(RoundStart));
    }

    public override bool IsVaildPlay()
    {
        // surely there is a better way to do this
        bool stun = card.stun;
        bool silenced = card.silenced;
        card.stun = false;
        card.silenced = false;

        bool valid = base.IsVaildPlay();

        card.stun = stun;
        card.silenced = silenced;
        return valid;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        card.Purge(oppenentEffectsOnly: true, triggerAuthority: card.hasAuthority);
    }

    void Damage(Unit sourceUnit, ref int damage, bool piercing, bool physical)
    {
        card.Heal(damage);
        damage = 0;
        
    }

    public virtual void RoundStart(RoundStart_e e)
    {
        activated = false;
    }
}
