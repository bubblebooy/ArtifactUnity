using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierAbility : Ability, IModifier
{

    public int attack;
    public int maxArmor, maxHealth;

    public int cleave = 0;
    public int siege = 0;
    public int retaliate = 0;

    public bool quickstrike = false;
    public bool disarmed = false;
    public bool caster = false;
    public bool piercing = false;
    public bool trample = false;
    public bool feeble = false;
    public bool damageImmunity = false;
    public bool untargetable = false;

    public override void Clone(Ability originalAbilityAbility)
    {
        base.Clone(originalAbilityAbility);
        attack = (originalAbilityAbility as ModifierAbility).attack;
        maxArmor = (originalAbilityAbility as ModifierAbility).maxArmor;
        maxHealth = (originalAbilityAbility as ModifierAbility).maxHealth;

        cleave = (originalAbilityAbility as ModifierAbility).cleave;
        siege = (originalAbilityAbility as ModifierAbility).siege;
        retaliate = (originalAbilityAbility as ModifierAbility).retaliate;

        quickstrike = (originalAbilityAbility as ModifierAbility).quickstrike;
        disarmed = (originalAbilityAbility as ModifierAbility).disarmed;
        caster = (originalAbilityAbility as ModifierAbility).caster;
        piercing = (originalAbilityAbility as ModifierAbility).piercing;
        trample = (originalAbilityAbility as ModifierAbility).trample;
        feeble = (originalAbilityAbility as ModifierAbility).feeble;
        damageImmunity = (originalAbilityAbility as ModifierAbility).damageImmunity;
        untargetable = (originalAbilityAbility as ModifierAbility).untargetable;
}
    public void Clone(IModifier originalIModifier)
    {
        print("I DONT THINK THIS SHOULD EVER BE CALLED");
        // dont think I need this on  Ability, IModifier. Ability should have its own clone
    }

    public void ModifyCard()
    {
        card = GetComponentInParent<Unit>();
        card.attack += attack;
        card.maxArmor += maxArmor;
        card.maxHealth += maxHealth;

        card.cleave += cleave;
        card.siege += siege;
        card.retaliate += retaliate;

        card.quickstrike |= quickstrike; // ? true : card.quickstrike;
        card.disarmed |= disarmed; // ? true : card.disarmed; 
        card.caster |= caster; // ? true : card.caster; 
        card.piercing |= piercing; // ? true : card.piercing; 
        card.trample |= trample; // ? true : card.trample; 
        card.feeble |= feeble; // ? true : card.feeble; 
        card.damageImmunity |= damageImmunity;
        card.untargetable |= untargetable;
    }

}
