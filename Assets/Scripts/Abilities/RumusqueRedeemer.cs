using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumusqueRedeemer : Ability
{
    // Start is called before the first frame update
    public override void OnPlay()
    {
        base.OnPlay();
        foreach (Unit unit in card.GetNeighbors())
        {
            if (unit != null)
            {
                UnitModifier deathShield = unit.gameObject.AddComponent<UnitModifier>() as UnitModifier;
                deathShield.SetDeathShield();
                deathShield.duration = 999; //best wat to set temporary and no duration?
            }
        }

    }
}
