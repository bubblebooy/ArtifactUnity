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
    public int decay = 0;
    public int regeneration = 0;

    public bool quickstrike = false;
    public bool disarmed = false;
    public bool stun = false;
    public bool caster = false;
    public bool piercing = false;
    public bool trample = false;
    public bool feeble = false;
    public bool damageImmunity = false;
    public bool untargetable = false;
    public bool deathShield = false;

    public bool opponentEffect = false;
    public bool temporary = true;
    public int duration = 0;

    public List<(System.Type, GameEventSystem.EventListener)> inPlayEvents = new List<(System.Type, GameEventSystem.EventListener)>();

    private void Awake()
    {
        unit = GetComponentInParent<Unit>();
        inPlayEvents.Add(GameEventSystem.Register<RoundStart_e>(RoundStart));
    }

    public void Clone(IModifier originalIModifier)
    {
        //unit = GetComponentInParent<Unit>();
        //inPlayEvents.Add(GameEventSystem.Register<RoundStart_e>(RoundStart));

        opponentEffect = (originalIModifier as UnitModifier).opponentEffect;
        temporary = (originalIModifier as UnitModifier).temporary;
        duration = (originalIModifier as UnitModifier).duration;

        attack = (originalIModifier as UnitModifier).attack;
        maxArmor = (originalIModifier as UnitModifier).maxArmor;
        maxHealth = (originalIModifier as UnitModifier).maxHealth;

        cleave = (originalIModifier as UnitModifier).cleave;
        siege = (originalIModifier as UnitModifier).siege;
        retaliate = (originalIModifier as UnitModifier).retaliate;
        decay = (originalIModifier as UnitModifier).decay;
        regeneration = (originalIModifier as UnitModifier).regeneration;

        quickstrike = (originalIModifier as UnitModifier).quickstrike;
        disarmed = (originalIModifier as UnitModifier).disarmed;
        stun = (originalIModifier as UnitModifier).stun;
        caster = (originalIModifier as UnitModifier).caster;
        piercing = (originalIModifier as UnitModifier).piercing;
        trample = (originalIModifier as UnitModifier).trample;
        feeble = (originalIModifier as UnitModifier).feeble;
        damageImmunity = (originalIModifier as UnitModifier).damageImmunity;
        untargetable = (originalIModifier as UnitModifier).untargetable;
        if ((originalIModifier as UnitModifier).deathShield) { SetDeathShield(); }
    }

    public void ModifyCard()
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

        unit.quickstrike |= quickstrike;
        unit.disarmed |= disarmed;
        unit.stun |= stun;
        unit.caster |= caster;
        unit.piercing |= piercing;
        unit.trample |= trample;
        unit.feeble |= feeble;
        unit.damageImmunity |= damageImmunity;
        unit.untargetable |= untargetable;
        deathShield &= unit.deathShield;
    }
    public void SetDeathShield()
    {
        deathShield = true;
        unit.deathShield = true;
    }

    public virtual void RoundStart(RoundStart_e e)
    {
        duration--;
        if (temporary && duration <= 0)
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (deathShield) { unit.deathShield = false; }
        GameEventSystem.Unregister(inPlayEvents);
    }
}
