using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroicResolve : Ability, IModifier
{
    int missingHealth = 0;

    public override void CardUpdate()
    {
        base.CardUpdate();
        missingHealth = card.maxHealth - card.health;
    }

    public void ModifyCard()
    {
        card = GetComponentInParent<Unit>();
        card.attack += missingHealth;
    }

}

