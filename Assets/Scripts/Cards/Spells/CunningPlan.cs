using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CunningPlan : MultiTargetSpell
{
    public override bool IsVaildTarget(GameObject target)
    {
        return base.IsVaildTarget(target) &&
            transform.parent.parent.gameObject != target &&
            target.GetComponent<CardSlot>().GetSide() == transform.parent.gameObject.GetComponent<Unit>().GetSide();
    }

    //public over bool IsVaildTarget(GameObject target)
    //{
    //    Unit targetUnit = target.GetComponentInChildren<Unit>();
    //    if (transform.parent.gameObject == target) { return false; }
    //    // not sure if I should use isVaildPlay here. but in this case I think it should work
    //    return PlayerManager.IsMyTurn &&
    //           GameManager.GameState == "Play" &&
    //           ManaManager.mana >= mana &&
    //           target.tag == "Card Slot" &&
    //           transform.parent.parent.gameObject != target &&
    //           target.GetComponent<CardSlot>().GetLane() == transform.parent.gameObject.GetComponent<Unit>().GetLane() &&
    //           target.GetComponent<CardSlot>().GetSide() == transform.parent.gameObject.GetComponent<Unit>().GetSide() &&
    //           (targetUnit?.hasAuthority != false || targetUnit?.untargetable != true);
    //}


    public override void OnActivate(List<GameObject> targets)
    {
        CardSlot cardSlot = gameObject.GetComponentInParent<CardSlot>();
        CardSlot targetSlot = targets[0].GetComponent<CardSlot>();

        Unit card = transform.parent.GetComponent<Unit>();

        targets[0].GetComponentInChildren<Unit>()?.transform.SetParent(cardSlot.transform, false);
        card.transform.SetParent(targetSlot.transform, false);

        base.OnActivate(targets);
    }
}
