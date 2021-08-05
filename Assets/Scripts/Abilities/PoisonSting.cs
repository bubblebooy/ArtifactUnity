using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSting : Ability
{
    // When Venomancer and allied Plague Wards deal physical damage to a unit or tower
    // give it +1 Decay until its next combat phase.

    // Possibly could be an Aura but I think it will be simpler otherwise

    protected override void Awake()
    {
        base.Awake();
        //PhantomAssassin = (PhantomAssassin)card;
        card.StrikeUnitEvent += StrikeUnit;
        card.StrikeTowerEvent += StrikeTower;
    }

    public void StrikeUnit(Unit target, ref int damage, ref bool piercing)
    {
        UnitModifier poisonSting = target.gameObject.AddComponent<UnitModifier>() as UnitModifier;
        poisonSting.decay = 1;
        poisonSting.opponentEffect = true;
        if (card.GameManager.GameState == "Combat" || card.GameManager.GameState == "Shop")
        {
            poisonSting.duration = 2;
        }
    }

    public void StrikeTower(TowerManager target, ref int damage, ref bool piercing)
    {
        TowerModifier poisonSting = target.gameObject.AddComponent<TowerModifier>() as TowerModifier;
        poisonSting.decay = 1;
        if (card.GameManager.GameState == "Combat" || card.GameManager.GameState == "Shop")
        {
            poisonSting.duration = 2;
        }
    }
}
