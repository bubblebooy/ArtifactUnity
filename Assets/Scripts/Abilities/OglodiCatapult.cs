using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OglodiCatapult : ModifierAbility
{
    private int baseSiege;

    protected override void Awake()
    {
        base.Awake();
        baseSiege = siege;
    }

    public override void Scheme()
    {
        base.Scheme();
        siege += 1;
    }
    public override void RoundStart()
    {
        base.RoundStart();
        siege = baseSiege;
    }
}
