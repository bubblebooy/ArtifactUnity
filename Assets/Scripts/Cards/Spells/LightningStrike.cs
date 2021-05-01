using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : Spell
{
    public int damage = 4;

    //public override bool IsVaildPlay(GameObject target)
    //{
        
    //    if (base.IsVaildPlay(target) && 
    //        target.GetComponent<Unit>().caster == true &&
    //        target.GetComponent<Unit>().GetSide() == "PlayerSide")
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    public override void OnPlay()
    {
        base.OnPlay();
        // should add a function onto CardSlot or Unit that does this (gets combat target)
        Unit caster = gameObject.transform.parent.GetComponentInChildren<Unit>();

        int slotNumber = caster.transform.parent.GetSiblingIndex();

        bool player = caster.GetSide() == "PlayerSide";
        Transform targetSlot = caster.GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetChild(slotNumber + caster.arrow);
        Unit target = targetSlot.GetComponentInChildren<Unit>();

        if (target != null)
        {
            target.Damage(damage,true);
        }
        else
        {
            TowerManager tower = caster.GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
            tower.Damage(damage,true);
        }
        DestroyCard(); // this might take too long
    }
}
