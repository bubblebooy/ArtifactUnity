using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class Spell : Card
{
    public TargetValidation vaildPlay;

    public override void OnValidate()
    {
        base.OnValidate();
        if (vaildPlay.targetCaster) { vaildPlay.targetOnlyPlayerSide = true; }
    }

    public override bool IsVaildPlay(GameObject target)
    {
        CardSlot targetSlot = target.GetComponentInParent<CardSlot>();
        Unit targetUnit = target.GetComponent<Unit>();
        if (base.IsVaildPlay(target) && 
            target.tag == vaildPlay.targetTag &&
            (!vaildPlay.targetCaster || (targetUnit.caster && !targetUnit.stun && !targetUnit.silenced)) &&
            (!vaildPlay.targetHero || targetUnit is Hero) &&
            (!vaildPlay.targetOnlyPlayerSide || targetSlot.GetSide() == "PlayerSide") &&
            (!vaildPlay.targetOnlyEnemySide || targetSlot.GetSide() == "EnemySide") &&
            (!vaildPlay.cantTargetRooted || targetUnit?.rooted == false)
            )
        {
            return true;
        }
        return false;
    }

    protected override IEnumerator Discard()
    {
        yield return new WaitForSeconds(1.0f);
        yield return base.Discard();
    }

}
