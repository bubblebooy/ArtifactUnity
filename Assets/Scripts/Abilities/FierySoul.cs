using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FierySoul : CooldownAbility
{
    // When you play a spell or enchantment from Lina's lane,
    // deal damage to Lina's combat target equal to the number of charges on Fiery Soul and
    // add 1 charge to Fiery Soul.
    // 1 round cooldown.

    int charges = 2;

    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<CardPlayed_e>(CardPlayed));
    }

    public override void CardUpdate()
    {
        displayCooldown.text = charges.ToString();
    }

    void CardPlayed(CardPlayed_e e)
    {
        if (cooldown <= 0)
        {
            Card playedCard = e.card.GetComponent<Card>();
            Unit caster = e.caster.GetComponent<Unit>();

            if (caster.GetSide() == card.GetSide() &&
                caster.GetLane() == card.GetLane() &&
                (playedCard is Spell || playedCard is Enchantment))
            {
                Unit combatTarget = card.GetCombatTarget();
                if(combatTarget is null)
                {
                    bool player = card.GetSide() == "PlayerSide";
                    TowerManager tower = card.GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
                    tower.Damage(charges);
                }
                else
                {
                    combatTarget.Damage(card, charges);
                }
                charges += 1;
                cooldown = baseCooldown;
            }
        }
    }
}
