using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmashTheirDefenses : Spell
{
    //Dispel a tower enchantment.
    // Draw a card.
    public override void OnPlay()
    {
        base.OnPlay();
        Destroy(transform.parent.GetComponent<TowerEnchantment>().gameObject);

        GameManager.GameUpdate();
        //Draw a card.
        DestroyCard();
    }
}
