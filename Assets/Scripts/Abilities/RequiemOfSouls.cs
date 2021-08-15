using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequiemOfSouls : ActiveAbility
{
    //Active 2, 1 Mana: Repeat for each charge on Necromastery: Deal 1 damage to a random enemy.

    [SerializeField]
    private Necromastery necromastery;

    public override void OnActivate()
    {
        base.OnActivate();

        int charges = necromastery.modifierAbilitySecondary.attack;

        Transform side = card.GetLane().transform
            .Find(card.GetSide() == "PlayerSide" ? "EnemySide" : "PlayerSide");
        Unit[] enemies = side.GetComponentsInChildren<Unit>();
        //print(enemies.Length);
        if (enemies.Length > 0)
        {
            int rnd;
            for (int i = 0; i < charges; i++)
            {
                card.GameManager.GameUpdate();
                rnd = Random.Range(0, enemies.Length);
                enemies[rnd].Damage(card, 1);
            }
        }

    }
}
