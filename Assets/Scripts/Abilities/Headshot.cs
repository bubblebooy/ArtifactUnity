using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Headshot : ActiveTargetAbility
{
    //Active 3, 1 Mana: Deal 4 damage to another unit within range 5. Cross Lane.
    public override bool IsVaildTarget(GameObject target)
    {
        int targetDistance = 0;
        CardSlot targetCardSlot = target.transform.parent.GetComponent<CardSlot>();
        targetCardSlot = targetCardSlot.hasAuthority ? targetCardSlot : targetCardSlot.GetAcrossCardSlot();
        if (targetCardSlot != null)
        {
            // Get all the card slots
            CardSlot[] slots = card.GetLane().transform.parent.GetComponentsInChildren<CardSlot>();
            // Select only the player slots
            slots = slots.Where(slot => slot.hasAuthority).ToArray();
            targetDistance = System.Array.FindIndex(slots, slot => slot == targetCardSlot);
            targetDistance -= System.Array.FindIndex(slots, slot => slot == card.GetCardSlot());
            targetDistance = Mathf.Abs(targetDistance);
        }
        return base.IsVaildTarget(target) &&
            targetDistance <= 5;
    }

    public override void OnActivate(List<GameObject> targets)
    {
        
        base.OnActivate(targets);
        targets[0].GetComponent<Unit>().Damage(card, 4);
    }
}
