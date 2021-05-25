using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitModifier : MonoBehaviour, IModifier
{
    private Unit unit;

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

    public bool opponentEffect = false;
    public bool temporary = true;
    public int duration = 0;

    private void Awake()
    {
        unit = GetComponentInParent<Unit>();
    }

    public void ModifyCard()
    {
        unit = unit ?? GetComponentInParent<Unit>();
        unit.attack += attack;
        //unit.armor += armor;
        //unit.health += health;
        unit.maxArmor += maxArmor;
        unit.maxHealth += maxHealth;

        unit.cleave += cleave;
        unit.siege += siege;
        unit.retaliate += retaliate;

        unit.quickstrike |= quickstrike;
        unit.disarmed |= disarmed;
        unit.caster |= caster;
        unit.piercing |= piercing;
        unit.trample |= trample;
        unit.feeble |= feeble;
        unit.damageImmunity |= damageImmunity;
        unit.untargetable |= untargetable;
    }

    public virtual void RoundStart()
    {
        duration--;
        if (temporary && duration <= 0)
        {
            Destroy(this);
        }
    }
}
