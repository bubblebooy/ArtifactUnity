using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ProwlerVanguard : Ability, IAura
{
    public int armor = 1;

    //private Unit[] auraUnits = Array.Empty<Unit>();
    private List<AuraModifier> auraMods = new List<AuraModifier>();

    public override void OnPlay()
    {
        inPlayEvents.Add(GameEventSystem.Register<Auras_e>(ContinuousEffect));
    }

    public void ContinuousEffect(Auras_e e)
    {
        ComponentGameObjectComparer comparer = new ComponentGameObjectComparer();
        Unit[] auraUnits = card.GetNeighbors().Where(x => x != null).ToArray();
        foreach (Unit unit in auraUnits.Except(auraMods , comparer))
        {
            AuraModifier modifier = unit.gameObject.AddComponent<AuraModifier>() as AuraModifier;
            modifier.maxArmor += armor;
            auraMods.Add(modifier);
            GameManager.updateloop = true;
        }
        foreach (AuraModifier modifier in auraMods.ToArray().Except(auraUnits, comparer))
        {
            Destroy(modifier);
            auraMods.Remove(modifier);
            GameManager.updateloop = true;
        }
        //Unit[] _auraUnits = card.GetNeighbors().Where(x => x != null).ToArray();
        //foreach (Unit unit in _auraUnits)
        //{
        //    if (unit == card) { continue; }
        //    if (unit != card && unit.GetSide() == card.GetSide())
        //    {
        //        unit.maxArmor += armor;
        //    }
        //}
        //foreach (Unit unit in _auraUnits.Except(auraUnits))
        //{
        //    if (unit == card) { continue; }
        //    unit.armor += armor;
        //}
        //foreach (Unit unit in auraUnits.Except(_auraUnits))
        //{
        //    if (unit == card) { continue; }
        //    unit.armor = Math.Min(unit.maxArmor, unit.armor);
        //}
        //auraUnits = _auraUnits;
    }

    protected override void OnDestroy()
    {
        LeavePlay();
        base.OnDestroy();
    }

    public override void Bounce()
    {
        LeavePlay();
        base.Bounce();
    }

    private void LeavePlay()
    {
        foreach (AuraModifier modifier in auraMods.ToList())
        {
            Destroy(modifier);
            auraMods.Remove(modifier);
            GameManager.updateloop = true;
        }
        //foreach (Unit unit in auraUnits)
        //{
        //    if (unit == card) { continue; }
        //    if (unit.armor != 0)
        //    {
        //        unit.armor += armor;
        //    }
        //}
    }

}
