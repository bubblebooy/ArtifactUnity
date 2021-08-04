using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritSiphon : ModifierAbility
{
    // Round Start: If blocked, gain +2 Regeneration and deal 2 damage to the enemy blocking Death Prophet.
    public override void OnPlay()
    {
        inPlayEvents.Add(GameEventSystem.Register<PlayStart_e>(PlayStart));
    }

    void PlayStart(PlayStart_e e)
    {
        Unit combatTarget = card.GetCombatTarget();
        if (combatTarget != null)
        {
            combatTarget.Damage(2);
            regeneration = 2;
        }
        else
        {
            regeneration = 0;
        }
    }
}
