using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Skewer : ActiveTargetAbility
{
    //Active 2, 1 Mana: Move Magnus to a slot within range 5. 
    //Pull the first enemy hero in that direction through other units towards Magnus new slot.
    //Magnus strikes it for 2 . Cross Lane
    public override bool IsVaildTarget(GameObject target)
    {
        if (card.rooted) { return false; }
        int targetDistance = 0;
        CardSlot targetCardSlot = target.GetComponent<CardSlot>();
        if(targetCardSlot != null)
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
            .Take(Mathf.Max(cardIndex - targetIndex, targetIndex - cardIndex)+1).ToArray();
        if(cardIndex > targetIndex)
        {
            Array.Reverse(targetSlots);
        }
        Hero firstEnemyHero = Array.Find(targetSlots, slot => slot.GetComponentInChildren<Hero>() != null)?.GetComponentInChildren<Hero>();
        CardSlot emptySlot = Array.FindLast(targetSlots, slot => slot.GetComponentInChildren<Unit>() == null);
        //print(firstEnemyHero);
        //print(emptySlot);
        if (firstEnemyHero != null && emptySlot != null)
        {
            firstEnemyHero.transform.SetParent(emptySlot.transform, false);
        }

        card.transform.SetParent(targets[0].transform, false);
    }
}


// WORKED BUT had idea for simpiler way/
/*
LaneManager targetLane = targetCardSlot.GetLane();
LaneManager cardLane = card.GetLane();
if (targetLane.transform.GetSiblingIndex() == cardLane.transform.GetSiblingIndex())
{
    targetDistance = Mathf.Abs(card.GetCardSlot().transform.GetSiblingIndex() - target.transform.GetSiblingIndex());
}
else
{
    if (cardLane.transform.GetSiblingIndex() > targetLane.transform.GetSiblingIndex())
    {
        targetDistance = card.GetCardSlot().transform.GetSiblingIndex();
        targetDistance += targetLane.GetComponentsInChildren<CardSlot>(true).Length / 2- target.transform.GetSiblingIndex();
    }
    else
    {
        targetDistance = cardLane.GetComponentsInChildren<CardSlot>(true).Length / 2 - card.GetCardSlot().transform.GetSiblingIndex();
        targetDistance += target.transform.GetSiblingIndex();
    }
    if (cardLane.GetComponent<LaneVariableSlots>()?.playerFull == false)
    {
        targetDistance -= 1;
    }
    if (targetLane.GetComponent<LaneVariableSlots>()?.playerFull == false)
    {
        targetDistance -= 1;
    }
    if(Mathf.Abs(targetLane.transform.GetSiblingIndex() - cardLane.transform.GetSiblingIndex()) > 2)
    {
        Transform midLane = targetLane.transform.parent.Find("MidLane");
        targetDistance += midLane.GetComponentsInChildren<CardSlot>().Length;
        if (midLane.GetComponent<LaneVariableSlots>()?.playerFull == false)
        {
            targetDistance -= 2;
        }
    }
}
*/
