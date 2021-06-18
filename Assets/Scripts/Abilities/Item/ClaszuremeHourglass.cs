using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaszuremeHourglass : CooldownAbility
{
    protected override void Awake()
    {
        base.Awake();
        inPlayEvents.Add(GameEventSystem.Register<AfterCombat_e>(AfterCombat));
    }

    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<AfterCombat_e>(AfterCombat));
    }

    public void AfterCombat(AfterCombat_e e)
    {
        if(cooldown <= 0)
        {
            card.Bounce();
            cooldown = baseCooldown;
        }
        
    }
}
