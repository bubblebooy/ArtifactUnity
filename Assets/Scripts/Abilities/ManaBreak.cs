using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBreak : Ability
{
    //Whenever Anti-Mage deals physical damage to an enemy hero before combat, burn 1 mana.
    protected override void Awake()
    {
        base.Awake();
        //PhantomAssassin = (PhantomAssassin)card;
        card.StrikeUnitEvent += StrikeUnit;
        card.RetaliateEvent += Retaliate;
    }

    void StrikeUnit(Unit target, ref int damage, ref bool piercing)
    {
        BurnMana(target);
    }
    void Retaliate(Unit strikingUnit, ref int damage)
    {
        BurnMana(strikingUnit);
    }

    void BurnMana(Unit unit)
    {
        if(card.GameManager.GameState != "Combat" &&
            unit is Hero &&
            card.hasAuthority != unit.hasAuthority)
        {
            GameObject.Find(unit.GetSide() == "PlayerSide" ? "PlayerMana" : "EnemyMana")
                .GetComponent<ManaManager>()
                .Burn(1, unit.GetLane());
        }
    }
}
