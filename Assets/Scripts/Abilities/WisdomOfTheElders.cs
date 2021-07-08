using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WisdomOfTheElders : ActiveAbility
{
    // Active 2, 1 Mana: Draw a card.
    public override void OnActivate()
    {
        base.OnActivate();
        if(card.hasAuthority){
            card.PlayerManager.CmdDrawCards(1);
        }
    }
}
