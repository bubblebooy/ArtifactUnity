using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearHead : ModifierAbility
{
    //Mazzie has infinite item slots.Mazzie has +1, +1, and +1 for each equipped item.

    int _maxArmor = 0;
    int _maxHealth = 0;


    public override void CardUpdate()
    {
        attack = 0;
        maxArmor = 0;
        maxHealth = 0;

        base.CardUpdate();
        foreach(Item item in card.GetComponentsInChildren<Item>(true))
        {
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
