using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : ActiveTargetAbility
{
    // Active 3, 1 Mana: Deal 1 piercing damage to a unit and disarm it. Give it -2 for two rounds.
    public override void OnActivate(List<GameObject> targets)
    {

        base.OnActivate(targets);
        Unit unit = targets[0].GetComponent<Unit>();
        unit.Damage(card, 1, piercing: true);
        UnitModifier laserDisarm = unit.gameObject.AddComponent<UnitModifier>() as UnitModifier;
        laserDisarm.disarmed = true;
        UnitModifier laserArmor = unit.gameObject.AddComponent<UnitModifier>() as UnitModifier;
        laserArmor.maxArmor = -2;
        laserArmor.duration = 2;
    }
}
