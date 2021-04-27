using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sven : Hero
{
    public override void Combat(bool quick = false)
    {
        if (quick == quickstrike && !disarmed)
        {
            Unit target = GetCombatTarget();
            if (target != null)
            {
                int targetHealth = Strike(target, attack + cleave, piercing);
                if ((trample || target.feeble) && targetHealth < 0)
                {
                    bool player = GetSide() == "PlayerSide";
                    TowerManager tower = GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
                    tower.Damage(-1 * targetHealth, piercing);
                }
            }
            else
            {
                bool player = GetSide() == "PlayerSide";
                TowerManager tower = GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
                tower.Damage(attack, piercing);
            }
            Unit[] AdjacentEnemies = GetAdjacentEnemies();
            for (int i = -1; i <= 1; i++)
            {
                if (i == arrow) { continue; }
                target = AdjacentEnemies[i + 1];
                if (target != null)
                {
                    Strike(target, attack + cleave, piercing);
                }
            }

        }
    }
}
