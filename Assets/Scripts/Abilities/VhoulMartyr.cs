using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VhoulMartyr : Ability
{
    //Death Effect: Give Vhoul Martyr's allies +1/0/+1 permanently.
    LaneManager lane;
    public override void DestroyCard()
    {
        lane = card.GetLane();
        GameEventSystem.Register<DeathEffects_e>(DeathEffect);
        base.DestroyCard();
    }

    void DeathEffect(DeathEffects_e e)
    {
        Unit[] allies = lane.transform
            .Find(card.hasAuthority ? "PlayerSide" : "EnemySide")
            .GetComponentsInChildren<Unit>();
        foreach(Unit ally in allies)
        {
            UnitModifier vhoulMartyr = ally.gameObject.AddComponent<UnitModifier>() as UnitModifier;
            vhoulMartyr.attack = 1;
            vhoulMartyr.maxHealth = 1;
            vhoulMartyr.temporary = false;
            //ally.health += 1;
            GameManager.updateloop = true;
        }
    }

}