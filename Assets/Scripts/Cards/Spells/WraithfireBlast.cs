using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WraithfireBlast : Spell
{
    //Stun a unit. Give it +2 Decay for two rounds.
    public override void OnPlay(CardPlayed_e cardPlayed_e)
    {
        base.OnPlay(cardPlayed_e);
        Unit target = transform.parent.GetComponent<Unit>();

        UnitModifier stunDecay = target.gameObject.AddComponent<UnitModifier>() as UnitModifier;
        stunDecay.stun = true;
        stunDecay.decay = 2;
        stunDecay.duration = 2;

        DestroyCard();
    }
}
