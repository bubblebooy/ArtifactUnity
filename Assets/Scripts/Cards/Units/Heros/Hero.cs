using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Hero : Unit
{
    public int respawn = 0;
    private TextMeshProUGUI displayRespawn;

    public override void OnValidate()
    {
        base.OnValidate();
        caster = true;
        gameObject.transform.Find("Color/ManaIcon").gameObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
        displayRespawn = gameObject.transform.Find("Respawn").GetComponent<TextMeshProUGUI>();
        displayRespawn.text = respawn.ToString();
        displayRespawn.enabled = respawn != 0;
    }

    public override void CardUpdate()
    {
        base.CardUpdate();
        displayRespawn = gameObject.transform.Find("Respawn").GetComponent<TextMeshProUGUI>();
        displayRespawn.text = respawn.ToString();
        displayRespawn.enabled = respawn != 0;
    }

    public override void Stage()
    {
        staged = true;
    }

    public override void DestroyCard()
    {
        respawn = 2;
        gameObject.transform.SetParent(
            GameObject.Find(gameObject.GetComponent<NetworkIdentity>().hasAuthority ? "PlayerFountain" : "EnemyFountain").transform,
            false);
        isDraggable = true;
        armor = maxArmor;
        health = maxHealth;
    }

    public override void Bounce()
    {
        respawn = 1;
        gameObject.transform.SetParent(
            GameObject.Find(gameObject.GetComponent<NetworkIdentity>().hasAuthority ? "PlayerFountain" : "EnemyFountain").transform,
            false);
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