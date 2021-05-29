using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenestratingKobold : Ability
{
    //Play Effect: Bounce the enemy blocking Defenestrating Kobold.
    public override void OnPlay()
    {
        base.OnPlay();
        Unit enemyBlocking = card.GetCombatTarget();
        enemyBlocking?.Bounce();
    }
}
