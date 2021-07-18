using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitedWeFall : Ability
{
    //If this Meepo dies, your Meepos in all lanes die.
    bool alive;

    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<UnitKilled_e>(UnitKilled));
        alive = true;
    }

    void UnitKilled(UnitKilled_e e)
    {
        if(alive && 
            e.card != card &&
            e.card.cardName == "Meepo" &&
            e.card.hasAuthority == card.hasAuthority)
        {
            alive = false;
            if (card.GameManager.GameState == "Combat")
            {
                inPlayEvents.Add(GameEventSystem.Register<AfterCombat_e>(AfterCombat));
            }
            else
            {
                card.DestroyCard();
            }
        }
    }

    void AfterCombat(AfterCombat_e e)
    {
        card.DestroyCard();
    }
}
