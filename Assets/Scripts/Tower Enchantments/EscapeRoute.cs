using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeRoute : ActiveTowerEnchantment
{

    public override bool IsVaildTarget(GameObject target)
    {
        int i = selectedTargets?.Count ?? 0;
        print(i);
        return base.IsVaildTarget(target) &&
            (!(i == 1) || Mathf.Abs(
                target.GetComponentInParent<CardSlot>().GetLane().transform.GetSiblingIndex() 
                - LaneManager.transform.GetSiblingIndex()) == 1);
    }

    public override void OnActivate(List<GameObject> targets)
    {
        base.OnActivate(targets);

        targets[0].transform.SetParent(targets[1].transform, false);
    }
}
