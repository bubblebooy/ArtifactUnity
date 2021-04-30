using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkersCall : ActiveAbility
{

    public override void OnActivate()
    {
        base.OnActivate();
        Unit[] enemies = card.GetAdjacentEnemies();
        for (int q = 0; q < 2; q++)
        {
            foreach (Unit enemy in enemies)
            {
                if (enemy == null) { continue; }
                bool quick = q == 0;
                if (card.quickstrike == quick && !card.disarmed)
                {
                    card.Strike(enemy, card.attack, card.piercing);
                }
                if (enemy.quickstrike == quick && !enemy.disarmed)
                {
                    enemy.Strike(card, enemy.attack, enemy.piercing);
                }
            }

            //if (card.health <= 0) { card.disarmed = true; }
            card.quickstrikeDead();
            foreach (Unit enemy in enemies)
            {
                enemy?.quickstrikeDead();
                //if (enemy == null) { continue; }
                //if (enemy.health <= 0) { enemy.disarmed = true; }
            }
        }
    }
}
