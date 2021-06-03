using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkersCall : ActiveAbility
{

    public override void OnActivate()
    {
        base.OnActivate();
        card.Battle(card.GetAdjacentEnemies());
    }
}
