using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class Spell : Card
{
    [Header("Target information")]
    public string targetTag = "Lane";
    public bool targetOnlyEnemySide = false;
    public bool targetOnlyPlayerSide = false;
    public bool targetCaster = false;
    public bool targetHero = false;

    public override void OnValidate()
    {
        base.OnValidate();
        if (targetCaster) { targetOnlyPlayerSide = true; }
    }

    public override bool IsVaildPlay(GameObject target)
    {
        CardSlot targetSlot = target.GetComponentInParent<CardSlot>();
        Unit targetUnit = target.GetComponent<Unit>();
        if (base.IsVaildPlay(target) && 
            target.tag == targetTag &&
            (!targetCaster || targetUnit.caster) &&
            (!targetHero   || targetUnit is Hero) &&
            (!targetOnlyPlayerSide || targetSlot.GetSide() == "PlayerSide") &&
            (!targetOnlyEnemySide || targetSlot.GetSide() == "EnemySide")
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
