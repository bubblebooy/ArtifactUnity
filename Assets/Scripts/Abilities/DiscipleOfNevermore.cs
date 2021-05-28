using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DiscipleOfNevermore : Ability, IAura
{
    public int attack = 1;
    public int armor = -1;

    private Unit[] auraUnits = Array.Empty<Unit>();

    public override void OnPlay()
    {
        inPlayEvents.Add(GameEventSystem.Register<Auras_e>(ContinuousEffect));
    }

    public void ContinuousEffect(Auras_e e)
    {
        Unit[] _auraUnits = card.transform.parent.parent.gameObject.GetComponentsInChildren<Unit>();
        foreach (Unit unit in _auraUnits)
        {
            if( unit == card) { continue;  }
            if (unit != card && unit.GetSide() == card.GetSide())
            {
                unit.attack += attack;
                unit.maxArmor -= armor;
            }
        }
        foreach (Unit unit in _auraUnits.Except(auraUnits))
        {
            if (unit == card) { continue; }
            unit.armor -= armor;
        }
        foreach (Unit unit in auraUnits.Except(_auraUnits))
        {
            if (unit == card) { continue; }
            if (unit.armor != 0)
            {
                unit.armor += armor;
            }
        }
        auraUnits = _auraUnits;
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
        foreach (Unit unit in auraUnits)
        {
            if (unit == card) { continue; }
            if (unit.armor != 0)
            {
                unit.armor += armor;
            }
        }
    }

}
