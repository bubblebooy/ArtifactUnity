using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargets
{
    bool IsVaildTarget(GameObject target);
    void TargetSelected(GameObject target);
    void OnActivate(List<GameObject> targets);
    void OnActivate(CardPlayed_e cardPlayed_e, List<GameObject> targets);
    void TargetCanceled();
}

