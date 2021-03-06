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
    public int decay = 0;
    public int regeneration = 0;
    public int bounty = 0;

    public bool quickstrike = false;
    public bool disarmed = false;
    public bool silenced = false;
    public bool stun = false;
    public bool caster = false;
    public bool piercing = false;
    public bool trample = false;
    public bool feeble = false;
    public bool damageImmunity = false;
    public bool rooted = false;
    public bool untargetable = false;

    protected bool firstMod = true;

    public override void Clone(Ability originalAbilityAbility)
    {
        base.Clone(originalAbilityAbility);
        firstMod = false;
        attack = (originalAbilityAbility as ModifierAbility).attack;
        maxArmor = (originalAbilityAbility as ModifierAbility).maxArmor;
        maxHealth = (originalAbilityAbility as ModifierAbility).maxHealth;

        cleave = (originalAbilityAbility as ModifierAbility).cleave;
        siege = (originalAbilityAbility as ModifierAbility).siege;
        retaliate = (originalAbilityAbility as ModifierAbility).retaliate;
        decay = (originalAbilityAbility as ModifierAbility).decay;
        regeneration = (originalAbilityAbility as ModifierAbility).regeneration;
        bounty = (originalAbilityAbility as ModifierAbility).bounty;

        quickstrike = (originalAbilityAbility as ModifierAbility).quickstrike;
        disarmed = (originalAbilityAbility as ModifierAbility).disarmed;
        silenced = (originalAbilityAbility as ModifierAbility).silenced;
        stun = (originalAbilityAbility as ModifierAbility).stun;
        caster = (originalAbilityAbility as ModifierAbility).caster;
        piercing = (originalAbilityAbility as ModifierAbility).piercing;
        trample = (originalAbilityAbility as ModifierAbility).trample;
        feeble = (originalAbilityAbility as ModifierAbility).feeble;
        damageImmunity = (originalAbilityAbility as ModifierAbility).damageImmunity;
        rooted = (originalAbilityAbility as ModifierAbility).rooted;
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
        card.decay += decay;
        card.regeneration += regeneration;
        card.bounty += bounty;

        card.quickstrike |= quickstrike; 
        card.disarmed |= disarmed;
        card.silenced |= silenced;
        card.stun |= stun; 
        card.caster |= caster; 
        card.piercing |= piercing; 
        card.trample |= trample; 
        card.feeble |= feeble; 
        card.damageImmunity |= damageImmunity;
        card.rooted |= rooted;
        card.untargetable |= untargetable;

        if (firstMod)
        {
            card.armor += maxArmor;
            card.health += maxHealth;
            firstMod = false;
        }
    }

    protected void OnStatRemoved()
    {
        OnArmorRemoved(maxArmor);
        OnHealthRemoved(maxHealth);
    }
    protected void OnArmorRemoved(int removedArmor)
    {
        card.maxArmor -= removedArmor;
        if (card.armor > card.maxArmor)
        {
            card.armor = Mathf.Max(card.maxArmor, card.armor - removedArmor);
        }
    }
    protected void OnHealthRemoved(int removedHealth)
    {
        card.maxHealth -= removedHealth;
        if (card.health > card.maxHealth)
        {
            card.health = Mathf.Max(card.maxHealth, card.health - removedHealth);
        }
    }


    protected override void OnDestroy()
    {
        OnStatRemoved();
        base.OnDestroy();
    }

}
