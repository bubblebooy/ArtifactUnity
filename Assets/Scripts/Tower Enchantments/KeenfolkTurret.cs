using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeenfolkTurret : ActiveTowerEnchantment
{

    public int damage = 2;

    public override void OnActivate(List<GameObject> targets)
    {
        base.OnActivate(targets);
        targets[0].GetComponent<Unit>().Damage(damage, true);
    }

}
