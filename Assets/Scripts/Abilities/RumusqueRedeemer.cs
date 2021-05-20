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
                unit.deathShield = true;
            }
        }

    }
}
