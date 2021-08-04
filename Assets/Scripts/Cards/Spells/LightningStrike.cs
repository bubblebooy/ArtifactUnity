using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : Spell
{
    public int damage = 4;

    public override void OnPlay(CardPlayed_e cardPlayed_e)
    {
        base.OnPlay(cardPlayed_e);
        // should add a function onto CardSlot or Unit that does this (gets combat target)
        Unit caster = gameObject.transform.parent.GetComponentInChildren<Unit>();

        int slotNumber = caster.transform.parent.GetSiblingIndex();

        bool player = caster.GetSide() == "PlayerSide";
        Transform targetSlot = caster.GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetChild(slotNumber + caster.arrow);
        Unit target = targetSlot.GetComponentInChildren<Unit>();

        if (target != null)
        {
            target.Damage(cardPlayed_e.caster.GetComponent<Unit>(), damage,true);
        }
        else
        {
            TowerManager tower = caster.GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
            tower.Damage(damage,true);
        }
        DestroyCard(); // this might take too long
    }
}
