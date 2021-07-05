using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AnchorSmash : CooldownAbility
{
    //When you play a non-item card, Tidehunter strikes its adjacent enemies for 1  and gives them -1.
    //If there are none, strike the enemy tower for 1. 1 round cooldown.\

    public override void OnPlay()
    {
        inPlayEvents.Add(GameEventSystem.Register<CardPlayed_e>(CardPlayedEvent));
    }

    void CardPlayedEvent(CardPlayed_e e)
    {
        if (cooldown <= 0)
        {
            if (e.lane == card.GetLane().gameObject &&
                e.caster.GetComponent<Unit>().GetSide() == card.GetSide())
            {
                Card c = e.card.GetComponent<Card>();
                if (!(c is IItem))
                {
                    AnchorSmashEffect();
                    cooldown = baseCooldown;
                }
            }
        }
    }

    void AnchorSmashEffect()
    {
        Unit[] enemies = card.GetAdjacentEnemies();
        enemies = enemies.Where(enemy => enemy != null).ToArray();
        if (enemies.Length > 0)
        {
            foreach(Unit enemy in enemies)
            {
                card.Strike(enemy, 1);
                UnitModifier anchorSmash = enemy.gameObject.AddComponent<UnitModifier>() as UnitModifier;
                anchorSmash.attack = -1;
            }
        }
        else
        {
            bool player = card.GetSide() == "PlayerSide";
            TowerManager tower = card.GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
            card.Strike(tower, 1);
        }
    }
}
