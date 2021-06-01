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
    public override void Clone(Ability originalAbility)
    {
        base.Clone(originalAbility);
        missingHealth = (originalAbility as HeroicResolve).missingHealth;
    }
    public void Clone(IModifier originalIModifier)
    {
        print("I DONT THINK THIS SHOULD EVER BE CALLED");
        // dont think I need this on  Ability, IModifier. Ability should have its own clone
    }

    public void ModifyCard()
    {
        //card = GetComponentInParent<Unit>();
        card.attack += missingHealth;
    }

}

