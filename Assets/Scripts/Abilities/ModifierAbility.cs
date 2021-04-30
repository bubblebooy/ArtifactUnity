using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierAbility : Ability, IModifier
{

    public int attack;//, armor, health;
    public int maxArmor, maxHealth;

    public int cleave = 0;

    public bool quickstrike = false;
    public bool disarmed = false;
    public bool caster = false;
    public bool piercing = false;
    public bool trample = false;
    public bool feeble = false;

    public void ModifyCard()
    {
        card = GetComponentInParent<Unit>();
        card.attack += attack;
        //unit.armor += armor;
        //unit.health += health;
        card.maxArmor += maxArmor;
        card.maxHealth += maxHealth;

        card.cleave += cleave;

        card.quickstrike = quickstrike;
        card.disarmed = disarmed;
        card.caster = caster;
        card.piercing = piercing;
        card.trample = trample;
        card.feeble = feeble;
    }

}
