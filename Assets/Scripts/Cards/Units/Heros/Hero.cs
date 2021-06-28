using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Hero : Unit
{
    public int respawn = 0;
    public int[] signatureCards;
    private TextMeshProUGUI displayRespawn;
    public GameObject items;
    public int maxItemCount = 2;


    public override void OnValidate()
    {
        base.OnValidate();
        caster = true;
        gameObject.transform.Find("CardFront/ManaIcon").gameObject.SetActive(false);
        items = gameObject.transform.Find("Items").gameObject;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        displayRespawn = gameObject.transform.Find("Respawn").GetComponent<TextMeshProUGUI>();
        displayRespawn.text = respawn.ToString();
        displayRespawn.enabled = respawn != 0;
        events.Add(GameEventSystem.Register<RoundStart_e>(IncrementRespawn));
        items = gameObject.transform.Find("Items").gameObject;
        items.transform.SetSiblingIndex(1);
        items.SetActive(false);
    }

    private void IncrementRespawn(RoundStart_e e)
    {
        if(respawn > 0)
        {
            respawn -= 1;
        }
    }

    public override void CardUIUpdate(GameUpdateUI_e e)
    {
        base.CardUIUpdate(e);
        displayRespawn = gameObject.transform.Find("Respawn").GetComponent<TextMeshProUGUI>();
        displayRespawn.text = respawn.ToString();
        displayRespawn.enabled = respawn != 0;
    }

    public override void Stage(CardPlayed_e e)
    {
        staged = true;
    }


    public override void DestroyCard()
    {
        OnKilled();
        Bounce(ignoreRoot: true);
        respawn = 2;
    }

    public void ForceDestroyCard()
    {
        //for when you accually want to send a hero to the graveyard
        base.DestroyCard(killed: false);
    }

    public override void Bounce(bool ignoreRoot = false)
    {
        //When heroes enter the Fountain they are fully healed and temporary effects on them are purged.
        if (!ignoreRoot && rooted) { return; }
        respawn = 1;
        Purge(temporyEffectsOnly: true);
        gameObject.transform.SetParent(
            GameObject.Find(gameObject.GetComponent<NetworkIdentity>().hasAuthority ? "PlayerFountain" : "EnemyFountain").transform,
            false);
        isDraggable = true;
        armor = maxArmor;
        health = maxHealth;
        arrow = 0;
        inPlay = false;
        GameEventSystem.Unregister(inPlayEvents);
        GetComponent<AbilitiesManager>().Bounce();
    }

    public override bool IsVaildPlay(GameObject target)
    {
        if (respawn <= 0 && target.tag == targetTag && target.transform.parent.name == "PlayerSide")
        {
            if (GameManager.GameState == "Deploy")
            {
                return true;
            }
            else if (GameManager.GameState == "Flop")
            {
                string lane = target.transform.parent.parent.name;
                if  (GameManager.flop == 0 && lane == "MidLane"                                             ||
                    (GameManager.flop == 1 && lane == (PlayerManager.IsMyTurn ? "LeftLane" : "RightLane"))  ||
                    (GameManager.flop == 2 && lane == (PlayerManager.IsMyTurn ? "RightLane" : "LeftLane"))     )
                {
                    return true;
                }
            }


        }
        return false;
    }
}
