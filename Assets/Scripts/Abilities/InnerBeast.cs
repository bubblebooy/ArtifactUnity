using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerBeast : ModifierAbility
{

    // Beastmaster has +2 per neighboring Loyal Beast.
    public override void CardUpdate()
    {
        base.CardUpdate();
        if (card.inPlay)
        {
            attack = 0;
            foreach (Unit unit in card.GetNeighbors())
            {
                if (unit?.cardName == "Loyal Beast")
                {
                    attack += 2;
                }
            }
        }
    }

}
