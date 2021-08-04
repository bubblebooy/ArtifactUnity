using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmashTheirDefenses : Spell
{
    //Dispel a tower enchantment.
    // Draw a card.
    public override void OnPlay(CardPlayed_e cardPlayed_e)
    {
        base.OnPlay(cardPlayed_e);
        Destroy(transform.parent.GetComponent<TowerEnchantment>().gameObject);

        GameManager.GameUpdate();
        if (hasAuthority) { PlayerManager.CmdDrawCards(1); }
        DestroyCard();
    }
}
