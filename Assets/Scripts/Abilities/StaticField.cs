using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticField : Ability
{
    // Whenever you play a spell or enchantment from this lane, deal 1 piercing damage to Zeus's adjacent enemies.
    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<CardPlayed_e>(CardPlayed));
    }

    void CardPlayed(CardPlayed_e e)
    {
        Card playedCard = e.card.GetComponent<Card>();
        Unit caster = e.caster.GetComponent<Unit>();

        if (caster.GetSide() == card.GetSide() &&
            caster.GetLane() == card.GetLane() &&
            (playedCard is Spell || playedCard is Enchantment) &&
            !(playedCard is IItem))
        {
            foreach (Unit adjEnemy in card.GetAdjacentEnemies())
            {
                adjEnemy?.Damage(1, piercing:true);
            }
        }

    }
}
