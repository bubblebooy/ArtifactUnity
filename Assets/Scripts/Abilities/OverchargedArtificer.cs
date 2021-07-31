using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverchargedArtificer : ModifierAbility
{
    int _maxArmor = 0;
    int _maxHealth = 0;

    public override void CardUpdate()
    {
        maxHealth = 0;
        attack = 0;
        maxArmor = 0;

        base.CardUpdate();

        foreach (Transform abilityTransform in card.GetComponent<AbilitiesManager>().abilities.transform)
        {
            CooldownAbility ability = abilityTransform.GetComponent<Ability>() as CooldownAbility;
            if (ability?.itemAbility == true)
            {
                ability.cooldown = Mathf.Min(1, ability.cooldown);
            }
        }

        foreach (Transform itemTransform in (card as Hero).items.transform)
        {
            Item item = itemTransform.GetComponent<Item>();

            switch (item.itemType)
            {
                case ItemType.Accessories:
                    maxHealth += 1;
                    break;
                case ItemType.Weapon:
                    attack += 1;
                    break;
                case ItemType.Armor:
                    maxArmor += 1;
                    break;
            }
        }

        if (maxHealth < _maxHealth)
        {
            OnHealthRemoved(_maxHealth - maxHealth);
        }
        else if (maxHealth > _maxHealth)
        {
            card.health += maxHealth - _maxHealth;
        }

        if (maxArmor < _maxArmor)
        {
            OnArmorRemoved(_maxArmor - maxArmor);
        }
        else if (maxArmor > _maxArmor)
        {
            card.armor += maxArmor - _maxArmor;
        }


        _maxArmor = maxArmor;
        _maxHealth = maxHealth;
    }
}
