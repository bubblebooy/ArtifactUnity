using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LucentBeam : Ability
{
    // Round Start: Deal 1 piercing damage to a random enemy and add a charge to your Eclipse cards.
    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<PlayStart_e>(PlayStart));
    }

    void PlayStart(PlayStart_e e)
    {
        Transform side = card.GetLane().transform
            .Find(card.GetSide() == "PlayerSide" ? "EnemySide" : "PlayerSide");
        Unit[] enemies = side.GetComponentsInChildren<Unit>();
        //print(enemies.Length);
        if (enemies.Length > 0)
        {
            int rnd = Random.Range(0, enemies.Length);
            enemies[rnd].Damage(card, 1, piercing: true);
        }
        if (card.hasAuthority)
        {
            ChargeEclipse(card.PlayerManager.PlayerDeck);
            ChargeEclipse(card.PlayerManager.PlayerArea);
            ChargeEclipse(card.PlayerManager.PlayerOverDraw);
        }
        else
        {
            ChargeEclipse(card.PlayerManager.EnemyDeck);
            ChargeEclipse(card.PlayerManager.EnemyArea);
            ChargeEclipse(card.PlayerManager.EnemyOverDraw);
        }

    }
    
    void ChargeEclipse(GameObject location)
    {
        Eclipse[] eclipses = location.GetComponentsInChildren<Eclipse>();
        foreach(Eclipse eclipse in eclipses)
        {
            eclipse.charges += 1;
        }
    }
}
