using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiabolicConclusion : Spell
{
    //public override bool IsVaildPlay(GameObject target)
    //{

    //    if (base.IsVaildPlay(target) &&
    //        target.GetComponent<Unit>().caster == true &&
    //        target.GetComponent<Unit>().GetSide() == "PlayerSide")
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    public override void OnPlay()
    {
        Unit target = transform.parent.GetComponent<Unit>();
        Transform side = target.transform.parent.parent;
        int attack = 0;
        foreach (Unit unit in side.GetComponentsInChildren<Unit>())
        {
            if(unit is Hero) { continue; }  // should I add "|| target == unit"       ???
            if(unit.disarmed == false)
            {
                attack += unit.attack;
                UnitModifier disarmed = unit.gameObject.AddComponent<UnitModifier>() as UnitModifier;
                disarmed.disarmed = true;
            }
            
        }
        UnitModifier attackBuff = target.gameObject.AddComponent<UnitModifier>() as UnitModifier;
        attackBuff.attack = attack;
        attackBuff.trample = true;
        DestroyCard();
    }
    
}
