using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveTargetAbility : ActiveAbility, ITargets
{
    public string targetTag = "Unit";
    public bool targetCrossLane = false;
    public bool targetOnlyEnemySide = false;
    public bool targetOnlyPlayerSide = false;

    public override void ActivateAbility()
    {
        //base.ActivateAbility();
        TargetSelector targetSelector = gameObject.AddComponent<TargetSelector>();
        transform.Find("background").GetComponent<Image>().color = Color.green;
    }

    public virtual bool IsVaildTarget(GameObject target)
    {
        CardSlot targetSlot = target.GetComponentInParent<CardSlot>();
        return (target.tag == targetTag &&
            (!targetCrossLane || targetSlot.GetLane() == card.GetLane()) &&
            (!targetOnlyPlayerSide || targetSlot.GetSide() == "PlayerSide") &&
            (!targetOnlyEnemySide || targetSlot.GetSide() == "EnemySide"));
    }

    public void TargetSelected(GameObject target)
    {
        PlayerManager.ActivateTargetAbility(
            card.gameObject,
            transform.GetSiblingIndex(),
            Card.GetLineage(target.transform));
    }

    public virtual void OnPlay(GameObject target, GameObject secondaryTarget = null)
    {
        cooldown = baseCooldown;
        transform.Find("background").GetComponent<Image>().color = Color.black;
        OnActivate();
    }

    public void TargetCanceled()
    {
        transform.Find("background").GetComponent<Image>().color = Color.black;
    }
}
