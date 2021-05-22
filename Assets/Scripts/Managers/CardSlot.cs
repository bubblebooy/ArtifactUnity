using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CardSlot : NetworkBehaviour
{
    //public PlayerManager PlayerManager;

    public GameManager GameManager;

    public override void OnStartClient()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // MIGHT NEED THIS IF ADD CARDS THEN ADD NEWS CARD SLOTS
    //public void Start()
    //{
    //    GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    //}

    public void SlotUpdate()
    {
        //NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        //PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        // Card[] cards = transform.GetComponentsInChildren<Unit>(); // Not sure about order if I do this

        //transform.GetChild(transform.childCount - 1).gameObject.GetComponent<Unit>().Devour();
        //does not matter for any card currently, does matter for Hunger Mode


        for (int i = transform.childCount - 2; i >= 0 ; i--)
        {
            transform.GetChild(transform.childCount - 1).gameObject.GetComponent<Unit>().PlacedOnTopOf(transform.GetChild(i).gameObject.GetComponent<Unit>());
            if (GameManager.GameState == "ResolveDeploy" && transform.GetChild(i).gameObject.GetComponent<Hero>() != null)
            {
                transform.GetChild(i).gameObject.GetComponent<Hero>().Bounce();
            }
            transform.GetChild(i).gameObject.GetComponent<Unit>().DestroyCard();
            Debug.Log(transform.childCount);
        }
    }

    public void UnStage()
    {
        Card[] cards = transform.GetComponentsInChildren<Unit>();
        foreach(Card card in cards)
        {
            if (card.staged) { card.UnStage(); }
        }
    }

    public string GetSide()
    {
        return transform.parent.name;
    }

    public TowerManager GetTower()
    {
        return transform.parent.GetComponentInChildren<TowerManager>();
    }

    public LaneManager GetLane()
    {
        return transform.parent.parent.GetComponent<LaneManager>();
    }


}
