using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overload : ModifierAbility
{
    // After you play a spell or enchantment from Storm Spirit's lane, give Storm Spirit +1 this round.
    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<CardPlayed_e>(CardPlayed));
        events.Add(GameEventSystem.Register<RoundStart_e>(RoundStart));
    }

    void CardPlayed(CardPlayed_e e)
    {

        Card playedCard = e.card.GetComponent<Card>();
        Unit caster = e.caster.GetComponent<Unit>();

        if (caster.GetSide() == card.GetSide() &&
            caster.GetLane() == card.GetLane() &&
            (playedCard is Spell || playedCard is Enchantment))
        {
            attack += 1;
        }
    }

    public virtual void RoundStart(RoundStart_e e)
    {
        attack = 0;
    }
}
