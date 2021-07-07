using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModifier : MonoBehaviour
{
    protected Unit unit;

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
    public bool stun = false;
    public bool caster = false;
    public bool piercing = false;
    public bool trample = false;
    public bool feeble = false;
    public bool damageImmunity = false; 
    public bool rooted = false;
    public bool untargetable = false;
    public bool deathShield = false;



    protected bool firstMod = true;

    public List<(System.Type, GameEventSystem.EventListener)> events = new List<(System.Type, GameEventSystem.EventListener)>();

    protected virtual void Awake()
    {
        unit = GetComponentInParent<Unit>();
    }

    public virtual void Clone(IModifier originalIModifier)
    {
        //unit = GetComponentInParent<Unit>();
        //inPlayEvents.Add(GameEventSystem.Register<RoundStart_e>(RoundStart));
        firstMod = false;

        attack = (originalIModifier as UnitModifier).attack;
        maxArmor = (originalIModifier as UnitModifier).maxArmor;
        maxHealth = (originalIModifier as UnitModifier).maxHealth;

        cleave = (originalIModifier as UnitModifier).cleave;
        siege = (originalIModifier as UnitModifier).siege;
        retaliate = (originalIModifier as UnitModifier).retaliate;
        decay = (originalIModifier as UnitModifier).decay;
        regeneration = (originalIModifier as UnitModifier).regeneration;
        bounty = (originalIModifier as UnitModifier).bounty;

        quickstrike = (originalIModifier as UnitModifier).quickstrike;
        disarmed = (originalIModifier as UnitModifier).disarmed;
        stun = (originalIModifier as UnitModifier).stun;
        caster = (originalIModifier as UnitModifier).caster;
        piercing = (originalIModifier as UnitModifier).piercing;
        trample = (originalIModifier as UnitModifier).trample;
        feeble = (originalIModifier as UnitModifier).feeble;
        damageImmunity = (originalIModifier as UnitModifier).damageImmunity;
        rooted = (originalIModifier as UnitModifier).rooted;
        untargetable = (originalIModifier as UnitModifier).untargetable;
        if ((originalIModifier as UnitModifier).deathShield) { SetDeathShield(); }
    }

    public virtual void ModifyCard()
    {
        unit = unit ?? GetComponentInParent<Unit>();
        unit.attack += attack;
        unit.maxArmor += maxArmor;
        unit.maxHealth += maxHealth;

        unit.cleave += cleave;
        unit.siege += siege;
        unit.retaliate += retaliate;
        unit.decay += decay;
        unit.regeneration += regeneration;
        unit.bounty += bounty;

        unit.quickstrike |= quickstrike;
        unit.disarmed |= disarmed;
        unit.stun |= stun;
        unit.caster |= caster;
        unit.piercing |= piercing;
        unit.trample |= trample;
        unit.feeble |= feeble;
        unit.damageImmunity |= damageImmunity;
        unit.rooted |= rooted;
        unit.untargetable |= untargetable;
        deathShield &= unit.deathShield;

        if (firstMod)
        {
            unit.armor += maxArmor;
            unit.health += maxHealth;
            firstMod = false;
        }
    }
    public void SetDeathShield()
    {
        deathShield = true;
        unit.deathShield = true;
    }

    protected void OnDestroy()
    {
        if (deathShield) { unit.deathShield = false; }
        GameEventSystem.Unregister(events);
        if(maxArmor > 0)
        {
            unit.maxArmor -= maxArmor;
            if (unit.armor > unit.maxArmor)
            {
                unit.armor = Mathf.Max(unit.maxArmor, unit.armor - maxArmor);
            }
        }
        else if(maxArmor < 0 && unit.armor != 0)
        {
            unit.armor -= maxArmor;
        }
        if(maxHealth > 0)
        {
            unit.maxHealth -= maxHealth;
            if (unit.health > unit.maxHealth)
            {
                unit.health = Mathf.Max(unit.maxHealth, unit.health - maxHealth);
            }
        }
        else if (maxHealth < 0)
        {
            unit.health -= maxHealth;
        }

    }
}
