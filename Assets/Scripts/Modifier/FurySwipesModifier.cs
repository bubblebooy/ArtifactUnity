using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurySwipesModifier : UnitModifier
{
    private int previousMaxArmor;
    public override void Clone(IModifier originalIModifier)
    {
        previousMaxArmor = maxArmor;
    }

    protected override void Awake()
    {
        base.Awake();
        firstMod = false;
    }

    public override void ModifyCard()
    {
        base.ModifyCard();
        unit.armor += maxArmor - previousMaxArmor;
        previousMaxArmor = maxArmor;
    }
}
