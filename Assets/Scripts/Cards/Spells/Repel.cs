using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repel : Spell
{
    public override void OnPlay()
    {
        base.OnPlay();
        Unit target = transform.parent.GetComponent<Unit>();
        //quickcast set on prefab

        //Purge oppents effects
        target.Purge(oppenentEffectsOnly: true, triggerAuthority: hasAuthority);

        //Untargetable
        UnitModifier untargetable = target.gameObject.AddComponent<UnitModifier>() as UnitModifier;
        untargetable.untargetable = true;
        untargetable.opponentEffect = hasAuthority != target.hasAuthority;

        DestroyCard();
    }
}
