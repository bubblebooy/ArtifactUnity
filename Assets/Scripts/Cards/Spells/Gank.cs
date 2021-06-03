using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gank : MultiTargetSpell
{
    public override bool IsVaildTarget(GameObject target)
    {
        return base.IsVaildTarget(target) &&
            transform.parent.gameObject != target;
    }

    public override void OnActivate(List<GameObject> targets)
    {
        Unit card = transform.parent.GetComponent<Unit>();
        card.Battle(targets[0].GetComponent<Unit>());

        base.OnActivate(targets);
    }
}
