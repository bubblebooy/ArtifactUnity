using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterCombatModifierAbility : ModifierAbility
{
    public int gainAttack;
    public int gainArmor;
    public int gainHealth;

    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<AfterCombat_e>(AfterCombat));
    }

    private void AfterCombat(AfterCombat_e e)
    {
        if(e.lane == null || (e.lane == card.GetLane() && card.GetLane().combated == false))
        {
            attack += gainAttack;
            maxArmor += gainArmor;
            card.armor += gainArmor;
            maxHealth += gainHealth;
            card.health += gainHealth;
        }
    }
}
