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
        Unit targetUnit = target.GetComponentInChildren<Unit>();
        if (transform.parent.gameObject == target) { return false; }
        // not sure if I should use isVaildPlay here. but in this case I think it should work
        return PlayerManager.IsMyTurn &&
               GameManager.GameState == "Play" &&
               ManaManager.mana >= mana &&
               target.tag == "Card Slot" &&
               transform.parent.parent.gameObject != target &&
               target.GetComponent<CardSlot>().GetLane() == transform.parent.gameObject.GetComponent<Unit>().GetLane() &&
               target.GetComponent<CardSlot>().GetSide() == transform.parent.gameObject.GetComponent<Unit>().GetSide() &&
               (targetUnit?.hasAuthority != false || targetUnit?.untargetable != true);
    }

    public void TargetSelected(GameObject target)
    {
        PlayerManager.PlayCard(gameObject, GetLineage(), GetLineage(target.transform));
    }

    public void OnPlay(GameObject target, GameObject secondaryTarget = null)
    {
        OnPlay();
        CardSlot cardSlot = gameObject.GetComponentInParent<CardSlot>();
        CardSlot targetSlot = target.GetComponent<CardSlot>();

        Unit card = transform.parent.GetComponent<Unit>();

        target.GetComponentInChildren<Unit>()?.transform.SetParent(cardSlot.transform, false);
        card.transform.SetParent(targetSlot.transform,false);
        

        DestroyCard();
    }

    public void TargetCanceled()
    {
        UnStage();
    }
}
