using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroicResolve : ModifierAbility
{
    int missingHealth = 0;

    public override void CardUpdate()
    {
        base.CardUpdate();
        card = card ?? GetComponentInParent<Unit>();
        if (card?.inPlay == true)
        {
            attack = card.maxHealth - card.health;
        }
    }
    public override void Clone(Ability originalAbility)
    {
        base.Clone(originalAbility);
        missingHealth = (originalAbility as HeroicResolve).missingHealth;
    }

}

