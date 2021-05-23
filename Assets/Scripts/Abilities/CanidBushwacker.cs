using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanidBushwacker : Ability
{
    public override void OnPlay()
    {
        base.OnPlay();
        card.GameManager.GameUpdate();
        Unit combatTarget = card.GetCombatTarget();
        if (combatTarget != null)
        {
            card.Strike(card.GetCombatTarget(), card.attack, card.piercing);
        }
    }
}
