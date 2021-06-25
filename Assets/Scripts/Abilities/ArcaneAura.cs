using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneAura : CooldownAbility
{
    // When you play a spell or enchantment from any lane, restore 2 Mana. 1 round cooldown. Cross Lane
    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<CardPlayed_e>(CardPlayed));
    }

    void CardPlayed(CardPlayed_e e)
    {
        if (cooldown <= 0 )
        {
            Card playedCard = e.card.GetComponent<Card>();
            Unit caster = e.caster.GetComponent<Unit>();

            if (caster.GetSide() == card.GetSide() &&  (playedCard is Spell || playedCard is Enchantment))
            {
                card.ManaManager.RestoreMana(2, e.lane.GetComponent<LaneManager>());
                cooldown = baseCooldown;
            }
        }
    }
}
