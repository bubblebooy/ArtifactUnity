using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetherWard : Ability
{
    public override void OnPlay()
    {
        inPlayEvents.Add(GameEventSystem.Register<CardPlayed_e>(CardPlayed));
    }

    private void CardPlayed(CardPlayed_e cardPlayed_e)
    {
        if(cardPlayed_e.card.GetComponent<Card>().hasAuthority != card.hasAuthority &&
            cardPlayed_e.lane == card.GetLane().gameObject)
        {
            card.GetLane().transform
                .Find(card.GetSide() == "PlayerSide" ? "EnemySide" : "PlayerSide")
                .Find("Tower")
                .GetComponent<TowerManager>()
                .Damage(1, piercing: true);
            GameObject.Find(card.GetSide() == "PlayerSide" ? "EnemyMana" : "PlayerMana")
                .GetComponent<ManaManager>()
                .Burn(1);
        }
    }
}
