using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkScroll : MultiTargetSpell, IItem
{
    [field: SerializeField]
    public int level { get; set; }
    [field: SerializeField]
    public int gold { get; set; }
    [field: SerializeField]
    public ItemType itemType { get; set; }

    public override bool IsVaildTarget(GameObject target)
    {
        return base.IsVaildTarget(target) &&
            Mathf.Abs(target.GetComponentInParent<LaneManager>().transform.GetSiblingIndex()
                 - transform.GetComponentInParent<LaneManager>().transform.GetSiblingIndex()) == 2;
    }

    public override void OnActivate(List<GameObject> targets)
    {
        CardSlot targetSlot = targets[0].GetComponent<CardSlot>();
        Unit card = transform.parent.GetComponent<Unit>();
        card.transform.SetParent(targetSlot.transform, false);
        base.OnActivate(targets);
    }
}
