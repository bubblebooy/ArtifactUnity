using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivineIntervention : Spell
{

    public override void OnPlay(CardPlayed_e cardPlayed_e)
    {
        base.OnPlay(cardPlayed_e);
        Unit target = transform.parent.GetComponent<Unit>();
        Transform side = transform.parent.Find(hasAuthority ? "PlayerSide" : "EnemySide");
        foreach (Unit unit in side.GetComponentsInChildren<Unit>())
        {

            UnitModifier divineIntervention = unit.gameObject.AddComponent<UnitModifier>() as UnitModifier;
            divineIntervention.damageImmunity = true;
        }
        DestroyCard();
    }
    
}