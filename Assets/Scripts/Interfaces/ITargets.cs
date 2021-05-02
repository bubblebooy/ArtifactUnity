using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargets
{
    bool IsVaildTarget(GameObject target);
    void TargetSelected(GameObject target);
    void OnPlay(GameObject target, GameObject secondaryTarget = null);
    void TargetCanceled();
}
