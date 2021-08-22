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

    public override void OnActivate(List<GameObject> targets)
    {
        CardSlot cardSlot = gameObject.GetComponentInParent<CardSlot>();
        CardSlot targetSlot = targets[0].GetComponent<CardSlot>();

        Unit card = transform.parent.GetComponent<Unit>();

        targets[0].GetComponentInChildren<Unit>()?.Move(cardSlot.transform, swap: true);
        card.Move(targetSlot.transform, swap: true);

        base.OnActivate(targets);
    }
}
