using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticRemnantSummon : ModifierAbility
{
    //Feeble
    //After Combat: Slay Static Remnant.
    //Death Effect: Deal 2 piercing damage to Static Remnant's adjacent enemies.
    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<AfterCombat_e>(AfterCombat));
    }
    public void AfterCombat(AfterCombat_e e)
    {
        card.health = 0; // set health 0 to proc deathShield
    }

    public override void DestroyCard()
    {
        Unit[] AdjacentEnemies = card.GetAdjacentEnemies();
        foreach (Unit enemy in AdjacentEnemies)
        {
            enemy?.Damage(card, 2);
        } 
        base.DestroyCard();
    }
}
