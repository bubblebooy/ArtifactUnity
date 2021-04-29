using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CunningPlan : Spell, ITargets
{
    public override void Stage()
    {
        TargetSelector targetSelector = gameObject.AddComponent<TargetSelector>();

        //PlayerManager.PlayCard(gameObject, GetLineage());
    }

    public bool IsVaildTarget(GameObject target)
    {
        if (transform.parent.gameObject == target) { return false; }
        // not sure if I should use isVaildPlay here. but in this case I think it should work
        return IsVaildPlay(target) &&
               transform.parent.gameObject != target &&
               target.GetComponent<Unit>().GetSide() == transform.parent.gameObject.GetComponent<Unit>().GetSide();
    }

    public void TargetSelected(GameObject target)
    {
        PlayerManager.PlayCard(gameObject, GetLineage(), GetLineage(target.transform));
    }

    public void OnPlay(GameObject target)
    {
        OnPlay();
        CardSlot cardSlot = gameObject.GetComponentInParent<CardSlot>();
        CardSlot targetSlot = target.GetComponentInParent<CardSlot>();

        Unit card = transform.parent.GetComponent<Unit>();

        card.transform.SetParent(targetSlot.transform,false);
        target.transform.SetParent(cardSlot.transform,false);

        DestroyCard();
    }
}
