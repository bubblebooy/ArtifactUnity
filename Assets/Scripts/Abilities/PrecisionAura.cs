using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrecisionAura : Ability, IAura
{
    int damage = 1;
    public void ContinuousEffect()
    {
        foreach (Unit unit in GameObject.Find("Board").GetComponentsInChildren<Unit>())
        {
            if( unit != card && unit.GetSide() == card.GetSide())
            {
                unit.attack += 1;
            }
            unit.CardUIUpdate();
        }
    }
}
