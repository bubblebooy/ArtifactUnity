using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DividedWeStand : Spell
{
    public override void OnPlay()
    {
        if (hasAuthority)
        {
            PlayerManager.CmdSummonHero("Meepo", GetLineage(transform.parent));
        }
        DestroyCard();
    }
}
