using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BracersOfSacrifice : ActiveAbility
{
    //Active 2, 1 Mana: Slay this hero and deal 4 damage to adjacent enemies.
    public override void OnActivate()
    {
        base.OnActivate();
        Unit[] AdjacentEnemies = card.GetAdjacentEnemies();
        foreach(Unit enemy in AdjacentEnemies)
        {
            if(enemy != null)
            {
                enemy.Damage(card, 4);
            }
        }
        card.health = 0; // set health 0 to proc deathShield
    }
}
