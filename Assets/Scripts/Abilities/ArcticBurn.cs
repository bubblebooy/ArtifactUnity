using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ArcticBurn : ActiveTargetAbility
{
    //Active 2, 1 Mana: +3. Move up to range 3.
    //Give +2 Decay to each enemy passed. Cross Lane

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
            targetDistance <= 3;
    }

    public override void OnActivate(List<GameObject> targets)
    {
        base.OnActivate(targets);

        UnitModifier arcticBurnAttack = card.gameObject.AddComponent<UnitModifier>() as UnitModifier;
        arcticBurnAttack.attack = 3;

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
        foreach (Unit enemy in targetSlots.Select(slot => slot.GetComponentInChildren<Unit>()))
        {
            if (enemy != null)
            {
                UnitModifier arcticBurn = enemy.gameObject.AddComponent<UnitModifier>() as UnitModifier;
                arcticBurn.decay = 2;
            }  
        }

        card.Move(targets[0].transform);
    }
}