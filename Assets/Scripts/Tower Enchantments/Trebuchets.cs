using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trebuchets : TowerEnchantment
{
    int damage = 2;
    public override void Combat()
    {
        base.Combat();
        bool player = GetSide() == "PlayerSide";
        TowerManager tower = GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
        tower.Damage(damage, true);
    }
}
