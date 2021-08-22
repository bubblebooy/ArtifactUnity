using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BlinkStrike : ActiveTargetAbility
{
    // Active 3, 1 Mana: Move up to range 5. Anti-Mage strikes the new unit blocking it for 1. Cross Lane.

    public override bool IsVaildTarget(GameObject target)
    {
        if (card.rooted) { return false; }
        int targetDistance = 0;
        CardSlot targetCardSlot = target.GetComponent<CardSlot>();
        if (targetCardSlot != null)
        {
            // Get all the card slots
            CardSlot[] slots = card.GetLane().transform.parent.GetComponentsInChildren<CardSlot>();
            // Select only the player slots
            slots = slots.Where(slot => slot.hasAuthority).ToArray();
            targetDistance = Array.FindIndex(slots, slot => slot == targetCardSlot);
            targetDistance -= Array.FindIndex(slots, slot => slot == card.GetCardSlot());
            targetDistance = Mathf.Abs(targetDistance);
        }
        return base.IsVaildTarget(target) &&
            targetDistance <= 5;
    }

    public override void OnActivate(List<GameObject> targets)
    {
        base.OnActivate(targets);
        card.Move(targets[0].transform);
        Unit newUnitBlocking = card.GetCombatTarget();
        if (newUnitBlocking != null)
        {
            card.Strike(newUnitBlocking, 1);
        }
    }
}
