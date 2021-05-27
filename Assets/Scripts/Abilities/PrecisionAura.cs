using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrecisionAura : Ability, IAura
{
    int damage = 1;

    public override void OnPlay()
    {
        inPlayEvents.Add(GameEventSystem.Register<Auras_e>(ContinuousEffect));
    }

    public void ContinuousEffect(Auras_e e)
    {
        foreach (Unit unit in GameObject.Find("Board").GetComponentsInChildren<Unit>())
        {
            if (unit != card && unit.GetSide() == card.GetSide())
            {
                unit.attack += damage;
            }
        }
        //print(card.transform.parent.name);
    }
}
