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
                Strike(target, attack + cleave, piercing);
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
                print("CLEAVE");
                target = AdjacentEnemies[i + 1];
                if (target != null)
                {
                    print("Not Null");
                    Strike(target, attack + cleave, piercing);
                }
            }

        }
    }
}
