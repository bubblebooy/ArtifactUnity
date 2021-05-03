using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeRoute : ActiveTowerEnchantment
{
    public override bool IsVaildTarget(GameObject target)
    {
        if (primaryTarget == null)
        {
            return base.IsVaildTarget(target) &&
                   target.GetComponent<Hero>() != null &&
                   target.GetComponent<Hero>().GetSide() == "PlayerSide" &&
                   target.GetComponent<Hero>().GetLane() == LaneManager;
        }
        else
        {
            CardSlot targetSlot = target.GetComponent<CardSlot>();
            return base.IsVaildTarget(target) &&
                   targetSlot != null &&
                   Mathf.Abs(targetSlot.GetLane().transform.GetSiblingIndex() - LaneManager.transform.GetSiblingIndex()) == 1;
        }

    }

    public override void OnPlay(GameObject target, GameObject secondaryTarget = null)
    {
        base.OnPlay(target, secondaryTarget);

        target.transform.SetParent(secondaryTarget.transform, false);
    }
}
