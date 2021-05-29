using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetherWard : Ability
{
    public override void OnPlay()
    {
        inPlayEvents.Add(GameEventSystem.Register<CardPlayed_e>(CardPlayed));
    }

    private void CardPlayed(CardPlayed_e CardPlayed)
    {

    }
}
