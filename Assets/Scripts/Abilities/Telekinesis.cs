using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Telekinesis : ActiveTargetAbility
{
    //Active 3, 1 Mana: Choose an enemy within range 3
    // Swap them to a new slot within range 3
    //Stun their new neighbors.Cross Lane

    public override bool IsVaildTarget(GameObject target)
    {
        CardSlot referenceCardSlot;
        CardSlot targetCardSlot;
        if ((selectedTargets?.Count ?? 0) == 0)
        {
            referenceCardSlot = card.GetAcrossCardSlot();
            targetCardSlot = target.transform.parent.GetComponent<CardSlot>();
        }
        else
        {
            referenceCardSlot = selectedTargets[0].transform.parent.GetComponent<CardSlot>();
            targetCardSlot = target.GetComponent<CardSlot>();
        }

        int targetDistance = 0;
        if (targetCardSlot != null)
        {
            // Get all the card slots
            CardSlot[] slots = referenceCardSlot.GetLane().transform.parent.GetComponentsInChildren<CardSlot>();
            // Select only the enemy slots
            slots = slots.Where(slot => !slot.hasAuthority).ToArray();
            targetDistance = Array.FindIndex(slots, slot => slot == targetCardSlot);
            targetDistance -= Array.FindIndex(slots, slot => slot == referenceCardSlot);
            targetDistance = Mathf.Abs(targetDistance);
        }
        return base.IsVaildTarget(target) &&
            targetDistance <= 3;
    }



    public override void OnActivate(List<GameObject> targets)
    {
        CardSlot cardSlot = targets[0].GetComponentInParent<CardSlot>();
        CardSlot targetSlot = targets[1].GetComponent<CardSlot>();

        Unit card = targets[0].GetComponent<Unit>();
        Unit targetCard = targets[1].GetComponentInChildren<Unit>();
        
        if(card.rooted != true && targetCard?.rooted != true)
        {
            targetCard?.transform.SetParent(cardSlot.transform, false);
            card.transform.SetParent(targetSlot.transform, false);
        }

        foreach ( Unit neighbor in card.GetNeighbors())
        {
            if(neighbor == null) { continue; }
            UnitModifier stun = neighbor.gameObject.AddComponent<UnitModifier>() as UnitModifier;
            stun.stun = true;
        }

        base.OnActivate(targets);
    }

}
