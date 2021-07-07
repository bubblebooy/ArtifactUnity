using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlansWithinPlans : Ability
{
    // Whenever you reveal an opponent's card
    //give Imperia +atk equal to the revealed card's Mana cost until its next combat.
    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<CardRevealed_e>(CardRevealed));
    }

    void CardRevealed(CardRevealed_e e)
    {
       if( e.card.hasAuthority != card.hasAuthority && e.revealedByOpponent)
        {
            UnitModifier plansWithinPlans = card.gameObject.AddComponent<UnitModifier>() as UnitModifier;
            plansWithinPlans.attack = e.card.mana;
            // not sure if this is the best way to do this but is easy
            if (card.GameManager.GameState == "Combat" || card.GameManager.GameState == "Shop")
            {
                plansWithinPlans.duration = 2; 
            }
        }
    }
}
