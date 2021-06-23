using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuillSpray : ActiveAbility
{
    //Active 3, 1 Mana: Deal 1 damage to adjacent enemies.
    public override void OnActivate()
    {
        base.OnActivate();
        foreach(Unit adjEnemy in card.GetAdjacentEnemies())
        {
            adjEnemy?.Damage(1);
        }
    }
}
