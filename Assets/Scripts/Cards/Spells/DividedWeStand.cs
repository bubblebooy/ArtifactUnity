using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DividedWeStand : Spell
{
    public override void OnPlay(CardPlayed_e cardPlayed_e)
    {
        base.OnPlay(cardPlayed_e);
        if (hasAuthority)
        {
            PlayerManager.CmdSummonHero("Meepo", GetLineage(transform.parent));
        }
        DestroyCard();
    }
}
