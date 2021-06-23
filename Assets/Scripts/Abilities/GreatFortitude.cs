using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GreatFortitude : ModifierAbility
{
    // Centaur Warrunner ignores negative effects.

    int _maxArmor = 0;
    int _maxHealth = 0;

    public override void CardUpdate()
    {
        base.CardUpdate();

        attack = 0;
        maxArmor = 0;
        maxHealth = 0;
        if (card.inPlay)
        {
            foreach (ModifierAbility modifier in card.GetComponentsInChildren<ModifierAbility>())
            {
                attack += modifier.attack < 0 ? -1 * modifier.attack : 0;
                maxArmor += modifier.maxArmor < 0 ? -1 * modifier.maxArmor : 0;
                maxHealth += modifier.maxHealth < 0 ? -1 * modifier.maxHealth : 0;
            }
            foreach (StatModifier modifier in card.GetComponentsInChildren<StatModifier>())
            {
                attack += modifier.attack < 0 ? -1 * modifier.attack : 0;
                maxArmor += modifier.maxArmor < 0 ? -1 * modifier.maxArmor : 0;
                maxHealth += modifier.maxHealth < 0 ? -1 * modifier.maxHealth : 0;
            }
        }
        if(maxHealth != _maxHealth)
        {
            card.health += maxHealth - _maxHealth;
        }
        if (maxArmor != _maxArmor && card.armor != 0)
        {
            card.armor += maxArmor - _maxArmor;
        }

        _maxArmor = maxArmor;
        _maxHealth = maxHealth;
    }
}
