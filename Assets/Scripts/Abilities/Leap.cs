using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Leap : ActiveTargetAbility
{
    // Active 3, 1 Mana: Move Mirana up to range 5. Give Mirana and new neighbors +2. Cross Lane
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
        card.transform.SetParent(targets[0].transform, false);

        UnitModifier leap = card.gameObject.AddComponent<UnitModifier>() as UnitModifier;
        leap.attack = 2;

        foreach (Unit neighbor in card.GetNeighbors())
        {
            if (neighbor != null)
            {
                leap = neighbor.gameObject.AddComponent<UnitModifier>() as UnitModifier;
                leap.attack = 2;
            }
        }
    }
}
