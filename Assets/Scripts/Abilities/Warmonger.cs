using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warmonger : Ability
{
    // Pillager. +2 damage when striking a tower.
    protected override void Awake()
    {
        base.Awake();
        card.StrikeTowerEvent += StrikeTower;
    }


    public void StrikeTower(TowerManager target, ref int damage, ref bool piercing)
    {
        damage += 2;

        if(target.health > 0)
        {
            int pillage = 2;
            GoldManager EnemyGoldManager = GameObject.Find(card.hasAuthority ? "EnemyGold" : "PlayerGold")
                .GetComponent<GoldManager>();
            if(EnemyGoldManager.gold < 2)
            {
                pillage = EnemyGoldManager.gold;
            }

            EnemyGoldManager.gold = EnemyGoldManager.gold - pillage;
            GameObject.Find(card.hasAuthority ? "PlayerGold" : "EnemyGold")
                .GetComponent<GoldManager>()
                .gold += pillage;
        }
    }
}
