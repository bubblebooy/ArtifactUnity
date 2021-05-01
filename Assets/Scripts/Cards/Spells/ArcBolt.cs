using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcBolt : Spell
{
    public int damage = 1;

    //public override bool IsVaildPlay(GameObject target)
    //{
    //    if (base.IsVaildPlay(target) &&
    //        target.GetComponent<Unit>().GetSide() == "EnemySide")
    //    { return true; }
    //    return false;
    //}

    public override void OnPlay()
    {
        base.OnPlay();
        // should add a function onto CardSlot or Unit that does this (gets combat target)
        //Unit caster = gameObject.transform.parent.GetComponentInChildren<Unit>();
        Unit target = transform.parent.GetComponent<Unit>();
        int slotNumber;
        slotNumber = target.transform.parent.GetSiblingIndex();
        target.Damage(damage, true);
        GameManager.GameUpdate();
        Transform side = target.transform.parent.parent;
        int numberOfSlots = side.GetComponentsInChildren<CardSlot>().Length;
        for (int i =0; i < 3; i++)
        {
            if(side.GetComponentsInChildren<Unit>().Length <= 1 && target != null) { break; }
            int rnd = (Random.value > 0.5) ? -1 : 1;
            int j = slotNumber + rnd;
            while (true)
            {
                if (j < 0 || j >= numberOfSlots)
                {
                    rnd = rnd * -1;
                    j = slotNumber + rnd;
                }
                target = side.GetChild(j).GetComponentInChildren<Unit>();
                if (target != null)
                {
                    slotNumber = target.transform.parent.GetSiblingIndex();
                    target.Damage(damage, true);
                    GameManager.GameUpdate();
                    break;
                }
                j = j + rnd;     
            }
        }


        DestroyCard(); // this might take too long
    }
}
