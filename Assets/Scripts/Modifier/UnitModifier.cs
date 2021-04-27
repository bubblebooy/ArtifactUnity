using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitModifier : MonoBehaviour
{
    private Unit unit;

    public int attack;//, armor, health;
    public int maxArmor, maxHealth;

    public int cleave = 0;

    public bool quickstrike = false;
    public bool disarmed = false;
    public bool caster = false;
    public bool piercing = false;
    public bool trample = false;
    public bool feeble = false;

    private void Start()
    {
        unit = GetComponentInParent<Unit>();
    }

    public void ModifyCard()
    {
        unit = GetComponentInParent<Unit>();
        unit.attack += attack;
        //unit.armor += armor;
        //unit.health += health;
        unit.maxArmor += maxArmor;
        unit.maxHealth += maxHealth;

        unit.cleave += cleave;

        unit.quickstrike = quickstrike;
        unit.disarmed = disarmed;
        unit.caster = caster;
        unit.piercing = piercing;
        unit.trample = trample;
        unit.feeble = feeble;
    }

    public virtual void RoundStart()
    {
        Destroy(this);
    }
}
