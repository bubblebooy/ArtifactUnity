using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalrusPunch : ActiveAbility
{
    //Active 3, 1 Mana: Double Tusk's atk this round.
    public override void OnActivate()
    {
        base.OnActivate();
        UnitModifier walrusPunch = card.gameObject.AddComponent<UnitModifier>() as UnitModifier;
        walrusPunch.attack = card.attack;
    }
}
