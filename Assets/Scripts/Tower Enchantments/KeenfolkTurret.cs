using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeenfolkTurret : ActiveTowerEnchantment
{

    public int damage = 2;

    public override bool IsVaildTarget(GameObject target)
    {
        return base.IsVaildTarget(target) &&
               target.GetComponent<Unit>() != null &&
               target.GetComponent<Unit>().GetLane() == LaneManager;
    }

    public override void OnPlay(GameObject target, GameObject secondaryTarget = null)
    {
        base.OnPlay(target, secondaryTarget);
        target.GetComponent<Unit>().Damage(damage, true);
    }

}
