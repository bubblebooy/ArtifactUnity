using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CardSlot : NetworkBehaviour
{
    //public PlayerManager PlayerManager;

    public GameManager GameManager;
    public List<(System.Type, GameEventSystem.EventListener)> events = new List<(System.Type, GameEventSystem.EventListener)>();
    public bool slotEnabled = true;

    Image image;
    new BoxCollider2D collider2D;

    public override void OnStartClient()
    {
        base.OnStartClient();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        events.Add(GameEventSystem.Register<GameUpdate_e>(SlotUpdate));
        events.Add(GameEventSystem.Register<GameUpdateUI_e>(SlotUpdateUI));
        image = gameObject.GetComponent<Image>();
        collider2D = gameObject.GetComponent<BoxCollider2D>();
    }

    // MIGHT NEED THIS IF ADD CARDS THEN ADD NEWS CARD SLOTS
    //public void Start()
    //{
    //    GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    //}

    public void SlotUpdate(GameUpdate_e e)
    {
        if (!isActiveAndEnabled) { return; }
        //NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        //PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        // Card[] cards = transform.GetComponentsInChildren<Unit>(); // Not sure about order if I do this

        //transform.GetChild(transform.childCount - 1).gameObject.GetComponent<Unit>().Devour();
        //does not matter for any card currently, does matter for Hunger Mode
        if(transform.childCount > 0)
        {
            slotEnabled = true;
        }

        for (int i = transform.childCount - 2; i >= 0 ; i--)
        {
            transform.GetChild(transform.childCount - 1).gameObject.GetComponent<Unit>().PlacedOnTopOf(transform.GetChild(i).gameObject.GetComponent<Unit>());
            if (e.gameState == "ResolveDeploy" && transform.GetChild(i).gameObject.GetComponent<Hero>() != null)
            {
                transform.GetChild(i).gameObject.GetComponent<Hero>().Bounce();
            }
            else
            {
                transform.GetChild(i).gameObject.GetComponent<Unit>().DestroyCard();
            }
            //Debug.Log(transform.childCount);
        }
    }
    public void SlotUpdateUI(GameUpdateUI_e e)
    {
        image.enabled = slotEnabled;
        collider2D.enabled = slotEnabled;
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

    private void OnDestroy()
    {
        GameEventSystem.Unregister(events);
    }


}
