using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Linq;

public class SpellSteal : Spell
{
    //Put a copy of the last non-item card your opponent played into your hand.
    //That card is blue and costs 1 less.
    //Quickcast
    GameHistory GameHistory;

    public override void OnStartClient()
    {
        base.OnStartClient();
        GameHistory = GameManager.GetComponent<GameHistory>();
    }

    public override void OnPlay(CardPlayed_e cardPlayed_e)
    {
        base.OnPlay(cardPlayed_e);
        if (hasAuthority)
        {
            //GameHistory.EnemyHistory.Where(x => x.action == "Card Played").ToList();
            for(int i = GameHistory.EnemyHistory.Count - 1 ; i >= 0; i--)
            {
                if( GameHistory.EnemyHistory[i].action != "Card Played" ||
                    GameHistory.EnemyHistory[i].gameObject.GetComponent<Item>() != null)
                {
                    continue;
                }
                PlayerManager.CloneToHand(GameHistory.EnemyHistory[i].gameObject, color: "blue", manaModifier: -1);
                break;
            }
        }
        DestroyCard();
    }
}
