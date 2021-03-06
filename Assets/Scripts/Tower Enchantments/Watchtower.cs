using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watchtower : TowerEnchantment
{
    //Enchant Tower
    //Play Effect: Copy a random card from your deck.It is blue, ephemeral, and can only be played from this lane.
    //Whenever your opponent plays a spell or enchantment in this lane, repeat this effect.
    //Start is called before the first frame update

    public override void OnPlay()
    {
        WatchtowerEffect();
        inPlayEvents.Add(GameEventSystem.Register<CardPlayed_e>(CardPlayedEvent));
    }

    public void WatchtowerEffect()
    {
        //Have to do this for both play so random stays syncd. 
        //could not sure if I could do diff ranges and have it stay syncd.
        GameObject deck = GameObject.Find(GetSide() == "PlayerSide" ? "PlayerDeck" : "EnemyDeck");
        int rnd = Random.Range(0, deck.transform.childCount);
        if (GetSide() == "PlayerSide")
        {
            GameObject card = deck.transform.GetChild(rnd).gameObject;
            GetLane().GetPlayerManager().CloneToHand(card, color: "blue", ephemeral: true, validLane: GetLane().gameObject);
        }
    }

   void CardPlayedEvent(CardPlayed_e e)
    {
        
        if (e.lane == GetLane().gameObject &&
            e.caster.GetComponent<Unit>().GetSide() != GetSide())
        {
            Card c = e.card.GetComponent<Card>();
            if ((c is Spell || c is Enchantment ) &&
                !(c is IItem))
            {
                WatchtowerEffect();
            }
        }

    }
}
