using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcussiveShot : ActiveTargetAbility
{
    //Active 2, 1 Mana: Give an enemy and its neighbors -3.
    //Quickcast
    public override void OnActivate(List<GameObject> targets)
    {
        base.OnActivate(targets);
        ConcussiveEffect(targets[0]);
        foreach(Unit neighbor in targets[0].GetComponent<Unit>().GetNeighbors())
        {
            if(neighbor != null)
            {
                ConcussiveEffect(neighbor.gameObject);
            }
        }

    }

    void ConcussiveEffect(GameObject unit)
    {
        UnitModifier concussiveShot = unit.AddComponent<UnitModifier>() as UnitModifier;
        concussiveShot.maxArmor = -3;
    }
}
