using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceStaff : ActiveTargetAbility
{
    //+2atk Active 2, 1 Mana: Push a unit.
    public override void OnActivate(List<GameObject> targets)
    {
        base.OnActivate();
        Unit target = targets[0].GetComponent<Unit>();
        CardSlot cardSlot = target.GetComponentInParent<CardSlot>();
        Transform side = target.transform.parent.parent;
        int slotNumber = target.transform.parent.GetSiblingIndex();
        int numberOfSlots = side.GetComponentsInChildren<CardSlot>().Length;
        int rnd = (Random.value > 0.5) ? -1 : 1;
        int destination;
        if ((slotNumber + rnd >= 0 || slotNumber + rnd < numberOfSlots) &&
            side.GetChild(slotNumber + rnd).GetComponentInChildren<Unit>() == null)
        {
            destination = slotNumber + rnd;
        }
        else if ((slotNumber - rnd >= 0 || slotNumber - rnd < numberOfSlots) &&
            side.GetChild(slotNumber - rnd).GetComponentInChildren<Unit>() == null)
        {
            destination = slotNumber - rnd;
        }
        else { return; }

        target.Move(side.GetChild(destination));
        target.transform.position = side.GetChild(destination).position;
    }
}
