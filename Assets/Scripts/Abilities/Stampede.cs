using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stampede : ActiveAbility
{
    //Active 3, 1 Mana: Give Centaur Warrunner and allies Trample.
    public override void OnActivate()
    {
        base.OnActivate();
        Transform side = card.transform.parent.parent;
        foreach (Unit unit in side.GetComponentsInChildren<Unit>())
        {
            UnitModifier stampede = unit.gameObject.AddComponent<UnitModifier>() as UnitModifier;
            stampede.trample = true;
        }
    }
}
