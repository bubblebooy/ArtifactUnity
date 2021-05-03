using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatCleave : Ability
{
    public void Combat(bool quick = false)
    {
        if (quick == card.quickstrike && !card.disarmed)
        {
            Unit target = card.GetCombatTarget();
            if (target != null)
            {
                int targetHealth = card.Strike(target, card.attack + card.cleave, card.piercing);
                if ((card.trample || target.feeble) && targetHealth < 0)
                {
                    bool player = card.GetSide() == "PlayerSide";
                    TowerManager tower = card.GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
                    card.Strike(tower, -1 * targetHealth, card.piercing);
                }
            }
            else
            {
                bool player = card.GetSide() == "PlayerSide";
                TowerManager tower = card.GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
                card.Strike(tower, card.attack, card.piercing);
            }
            Unit[] AdjacentEnemies = card.GetAdjacentEnemies();
            for (int i = -1; i <= 1; i++)
            {
                if (i == card.arrow) { continue; }
                target = AdjacentEnemies[i + 1];
                if (target != null)
                {
                    card.Strike(target, card.attack + card.cleave, card.piercing);
                }
            }

        }
    }
}
