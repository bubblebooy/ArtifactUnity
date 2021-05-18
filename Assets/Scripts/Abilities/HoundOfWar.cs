using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoundOfWar : ModifierAbility
{
    public override void PlacedOnTopOf(Unit unit)
    {
        base.PlacedOnTopOf(unit);
        unit.CardUpdate(); // this gets rid of any Aura effects
        attack += unit.attack;
        maxHealth += unit.health;
        card.health += unit.health;
        
    }
}
