using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatCleave : Ability
{
    public virtual void PreCombat(bool quick = false)
    {
        Sven sven = card as Sven;
        if (quick == card.quickstrike && !card.disarmed)
        {
            int cleave = card.attack + card.cleave;
            sven.BasePreCombat(quick);
            card.cleave = cleave;
        }
    }

    //public void Combat(bool quick = false)
    //{
    //    int attackTower = card.siege;
    //    if (quick == card.quickstrike && !card.disarmed)
    //    {
    //        Unit target = card.GetCombatTarget();
    //        if (target != null)
    //        {
    //            int targetHealth = card.Strike(target, card.attack + card.cleave, card.piercing);
    //            if ((card.trample || target.feeble) && targetHealth < 0)
    //            {
    //                attackTower += -1 * targetHealth;
    //            }
    //        }
    //        else
    //        {
    //            attackTower += card.attack;
    //        }
    //        if (attackTower > 0)
    //        {
    //            bool player = card.GetSide() == "PlayerSide";
    //            TowerManager tower = card.GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
    //            card.Strike(tower, attackTower, card.piercing);
    //        }
    //        Unit[] AdjacentEnemies = card.GetAdjacentEnemies();
    //        for (int i = -1; i <= 1; i++)
    //        {
    //            if (i == card.arrow) { continue; }
    //            target = AdjacentEnemies[i + 1];
    //            if (target != null)
    //            {
    //                card.Strike(target, card.attack + card.cleave, card.piercing);
    //            }
    //        }

    //    }
    //}
}
