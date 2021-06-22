using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoyalBeast : ModifierAbility
{

    // Minion: +2/+2 when neighbouring Beastmaster.
    public override void CardUpdate()
    {
        base.CardUpdate();
        firstMod = maxHealth != 2;
        if (maxHealth != 2)
        {
            OnStatRemoved();
        }
        attack = 0;
        maxHealth = 0;
        foreach (Unit unit in card.GetNeighbors())
        {
            if (unit?.cardName == "Beatmaster")
            {
                attack = 2;
                maxHealth = 2;
            }
        }
    }

}
