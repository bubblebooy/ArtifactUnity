using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aftershock : Ability
{
    // Whenever you play a spell or enchantment from this lane, stun Earthshaker's enemies for one action.
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
            (playedCard is Spell || playedCard is Enchantment) &&
            card.GetLane() == caster.GetLane())
        {
            Transform enemySide = card.GetLane().transform.Find(card.GetSide() == "PlayerSide" ? "EnemySide" : "PlayerSide");
            foreach (Unit unit in enemySide.GetComponentsInChildren<Unit>())
            {
                UnitModifier stunned = unit.gameObject.AddComponent<UnitModifier>() as UnitModifier;
                stunned.stun = true;
                stunned.SetDestoryOnNextTurn();
            }
        }
    }
}
