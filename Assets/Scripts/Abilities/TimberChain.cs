using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TimberChain : ActiveTargetAbility
{
    //Active 3, 1 Mana: Move up to range 5.
    //Deal 1 damage to each enemy passed on the way. Cross Lane

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

        CardSlot[] slots = card.GetLane().transform.parent.GetComponentsInChildren<CardSlot>();
        CardSlot[] targetSlots = slots.Where(slot => card.hasAuthority ? !slot.hasAuthority : slot.hasAuthority).ToArray();
        slots = slots.Where(slot => card.hasAuthority ? slot.hasAuthority : !slot.hasAuthority).ToArray();

        int targetIndex = Array.FindIndex(slots, slot => slot == targets[0].GetComponent<CardSlot>());
        int cardIndex = Array.FindIndex(slots, slot => slot == card.GetCardSlot());
        //print("cardIndex: " + cardIndex + "//" + "targetIndex: " + targetIndex);
        targetSlots = targetSlots.Skip(Mathf.Min(cardIndex, targetIndex))
            .Take(Mathf.Max(cardIndex - targetIndex, targetIndex - cardIndex) + 1).ToArray();
        if (cardIndex > targetIndex)
        {
            Array.Reverse(targetSlots);
        }
        foreach(Unit enemy in targetSlots.Select(slot => slot.GetComponentInChildren<Unit>()))
        {
            enemy?.Damage(1);
        }

        card.transform.SetParent(targets[0].transform, false);
    }
}
