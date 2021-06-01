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

    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<RoundStart_e>(RoundStart));
        inPlayEvents.Add(GameEventSystem.Register<Scheme_e>(Scheme));
    }

    public void Scheme(Scheme_e e)
    {
        if(card.hasAuthority == e.IsMyTrun)
        siege += 1;
    }
    public void RoundStart(RoundStart_e e)
    {
        siege = baseSiege;
    }
}
