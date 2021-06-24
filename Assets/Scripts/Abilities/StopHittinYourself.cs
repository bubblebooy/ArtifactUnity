using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopHittinYourself : ActiveAbility
{
    //Active 3, 1 Mana: Each unit with Keefe the Bold as their combat target strikes itself with +2.
    public override void OnActivate()
    {
        base.OnActivate();
        foreach(Unit enemy in card.GetAdjacentEnemies())
        {
            if(enemy?.GetCombatTarget() == card)
            {
                enemy.Strike(enemy, enemy.attack + 2, enemy.piercing);
            }
        }
    }
}
