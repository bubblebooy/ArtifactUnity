using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ravage : ActiveAbility
{
    //Active 4, 1 Mana: Stun enemy units in this lane within range 2.

    public override void OnActivate()
    {
        base.OnActivate();
        foreach(Unit enemy in card.GetAdjacentEnemies(2))
        {
            if(enemy is null) { continue;  }
            UnitModifier ravage = enemy.gameObject.AddComponent<UnitModifier>() as UnitModifier;
            ravage.stun = true;
        }
    }
}
