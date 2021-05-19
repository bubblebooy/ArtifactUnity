using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierAbility : Ability, IModifier
{

    public int attack;
    public int maxArmor, maxHealth;

    public int cleave = 0;
    public int siege = 0;

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
        card.maxArmor += maxArmor;
        card.maxHealth += maxHealth;

        card.cleave += cleave;
        card.siege += siege;

        card.quickstrike |= quickstrike; // ? true : card.quickstrike;
        card.disarmed |= disarmed; // ? true : card.disarmed; 
        card.caster |= caster; // ? true : card.caster; 
        card.piercing |= piercing; // ? true : card.piercing; 
        card.trample |= trample; // ? true : card.trample; 
        card.feeble |= feeble; // ? true : card.feeble; 
    }

}
