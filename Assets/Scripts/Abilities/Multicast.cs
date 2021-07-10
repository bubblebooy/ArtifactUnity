using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multicast : CooldownAbility
{
    //When you cast a spell or enchantment from this lane it can be repeated this round
    // 2 round cooldown.
    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<CardPlayed_e>(CardPlayed));
    }

    void CardPlayed(CardPlayed_e e)
    {
        if (cooldown <= 0)
        {
            Card playedCard = e.card.GetComponent<Card>();
            Unit caster = e.caster.GetComponent<Unit>();

            if (caster.GetSide() == card.GetSide() &&
                caster.GetLane() == card.GetLane() &&
                (playedCard is Spell || playedCard is Enchantment) &&
                !(playedCard is IItem))
            {
                if (card.hasAuthority)
                {
                    card.PlayerManager.CloneToHand(playedCard.gameObject, ephemeral: true, revealed: true);
                }
                cooldown = baseCooldown;
            }
        }
    }
}
