using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public UIManager UIManager;
    public PlayerManager PlayerManager;
    public GameHistory GameHistory;
    public GameObject Board;
    public GameObject PlayerFountain;
    public GameObject EnemyFountain;
    public GameObject PlayerMana;
    public GameObject EnemyMana;


    public string GameState = "Setup";
    //public int[] PlayerTowerHealth = new int[] { 40, 40, 40, 80};
    //public int[] EnemyTowerHealth = new int[] { 40, 40, 40, 80 };

    public bool CombatDirection = true;

    private int ReadyClicks = 0; // should I make this a bool?
    public int flop = 0;


    // Start is called before the first frame update
    public override void OnStartClient()
    {
        base.OnStartClient();
        UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        UIManager.UpdatePhaseText();
        Board = GameObject.Find("Board");
        PlayerFountain = GameObject.Find("PlayerFountain");
        EnemyFountain = GameObject.Find("EnemyFountain");
        PlayerMana = GameObject.Find("PlayerMana");
        EnemyMana = GameObject.Find("EnemyMana");
        GameHistory = GetComponent<GameHistory>();

    }

    public void ChangeGameState(string stateRequest)
    {
        GameUpdate(); // just in case
        if (ReadyClicks == 1)
        {
            switch (stateRequest)
            {
                case "Flop":
                    if(flop == 0)
                    {
                        SummonLaneCreeps();
                        UIManager.ZoomHand(false);
                    }
                    else { PlayHeros(); }
                    GameUpdate();
                    GameState = "Flop";
                    UIManager.ButtonInteractable(true);
                    UIManager.UpdateButtonText("Confirm");
                    UIManager.Flop(flop);
                    break;
                case "ResolveFlop":
                    ResolveDeploy();
                    if(flop < 2)
                    {
                        flop++;
                        PlayerManager.CmdGameChangeState("Flop");
                    }
                    else
                    {
                        UIManager.Flop(-1);
                        PlayerManager.CmdPlay();
                    }
                    break;
                case "Deploy":
                    RoundStart();
                    UIManager.ZoomHand(false);
                    GameState = "Deploy";
                    UIManager.ButtonInteractable(true);
                    UIManager.UpdateButtonText("Confirm");
                    break;
                case "ResolveDeploy":
                    ResolveDeploy();
                    PlayerManager.CmdPlay();
                    break;
                case "Play":
                    PlayHeros();
                    UIManager.ZoomHand(true);
                    GameState = "Play";
                    UIManager.UpdateButtonText("Pass");
                    UIManager.ButtonInteractable();
                    break;
                case "Combat":
                    GameState = "Combat";
                    UIManager.ButtonInteractable(false);
                    StartCoroutine(Combat());
                    break;
                case "Shop":
                    GameState = "Shop";
                    UIManager.ButtonInteractable(true);
                    UIManager.UpdateButtonText($"Skip: +{5} Gold");

                    break;
                default:
                    Debug.Log( GameState + " is not a vaild state request");
                    break;
            }
            ReadyClicks = 0;
        }
        else
        {
            ReadyClicks++;
        }

        UIManager.UpdatePhaseText();
        
        //Debug.Log("ReadyClicks: " + ReadyClicks);
        //Debug.Log($"State: {GameState}, Request: {stateRequest}");
    }


    public IEnumerator ActionTaken()
    {
        ReadyClicks = 0;
        yield return StartCoroutine(DelayedGameUpdate()); // was causing problems with purge
    }
    public IEnumerator CardPlayed(CardPlayed_e cardPlayed_e)
    {
        GameHistory.CardPlayed(cardPlayed_e.card.gameObject);
        yield return StartCoroutine(ActionTaken());
        GameEventSystem.Event(cardPlayed_e);
        yield return StartCoroutine(DelayedGameUpdate());
    }
    public IEnumerator AbilityActivated(GameObject card, int index)
    {
        GameHistory.AbilityActivated(card, index);
        yield return StartCoroutine(ActionTaken());
        //push AbilityActivated event
    }
    public IEnumerator TowerEnchantmentActivated(GameObject card, int index, string side)
    {
        GameHistory.TowerEnchantmentActivated(card, index, side);
        yield return StartCoroutine(ActionTaken());
        //push TowerEnchantmentActivated event
    }

    public void Passed()
    {
        GameHistory.Passed(PlayerManager.IsMyTurn); // how do I know who passed?
        if (ReadyClicks == 1)
        {
            ChangeGameState("Combat");
            ReadyClicks = 0;
        }
        else
        {
            GameEventSystem.Event(new Scheme_e(PlayerManager.IsMyTurn));
            ReadyClicks = 1;
        }
        GameUpdate();
    }

    public void NextTurn()
    {
        UIManager.ButtonInteractable();
    }

    IEnumerator Combat()
    {
        Debug.Log("COMBAT START");
        // Dont know if I need this // GameEventSystem.Event(new StartCombatPhase_e());
        LaneManager[] lanes = Board.GetComponentsInChildren<LaneManager>();
        if (!CombatDirection) { Array.Reverse(lanes); }
        foreach (LaneManager lane in lanes)
        {
            // call in the coroutine // GameEventSystem.Event(new Combat_e());
            yield return StartCoroutine(lane.Combat());
            // call in the coroutine // GameEventSystem.Event(new AfterCombat_e());
        }

        //any reason this should be one lane at a time?
        GameEventSystem.Event(new AfterCombat_e());
        
        CombatDirection = !CombatDirection;
        GameEventSystem.Event(new EndCombatPhase_e());
        SummonLaneCreeps();
        PlayerManager.CmdEndCombat();
        UIManager.ButtonInteractable(true);
    }


    void ResolveDeploy()
    {
        Hero[] heroes = Board.GetComponentsInChildren<Hero>();
        foreach (Hero hero in heroes)
        {
            // should I check hero.hasAuthority instead? of side?
            if (hero.staged && hero.transform.parent.parent.name == "PlayerSide")
            {
                PlayerManager.CmdDeployHero(hero.gameObject, hero.GetLineage());
            }
        }
        GameUpdate(); // have this do the bouncing
    }

    void PlayHeros()
    {
        // foreach play hero. Needs to be done after both side have moved their heroes
        Hero[] heroes = Board.GetComponentsInChildren<Hero>();
        foreach (Hero hero in heroes)
        {
            if (hero.staged)
            {
                //Do I bounce here?
                print(hero.cardName);
                GameHistory.HeroDeployed(hero.gameObject);
                hero.OnPlay();
            }
            
        }
        GameUpdate();
    }

    public void SummonLaneCreeps()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        LaneManager[] lanes = Board.GetComponentsInChildren<LaneManager>();
        foreach (LaneManager lane in lanes)
        {
            Transform side = lane.gameObject.transform.Find("PlayerSide");
            if (side.GetComponentsInChildren<Unit>().Length < side.GetComponentsInChildren<CardSlot>().Length)
            {
                PlayerManager.SpawnLaneCreeps(lane.gameObject);
            }
        }
    }

    public void GameUpdate(bool checkAlive = true)
    {
        //Set death counter to 0
        GameEventSystem.Event(new GameUpdate_e(checkAlive));
        GameEventSystem.Event(new Auras_e());
        GameEventSystem.Event(new GameUpdateUI_e());
        if (false) { GameUpdate(); }
    }

    private IEnumerator DelayedGameUpdate()
    {
        yield return null;//new WaitForSeconds(0.1f);
        GameUpdate();
    }

    //
    void RoundStart()
    {
        GameEventSystem.Event(new RoundStart_e());
        GameUpdate();
    }

    // Is there anypoint of having both a round end and a round start. yes round start after deploy and round end is before deploy
    //void RoundEnd()
    //{
    //    Card[] cards = Board.GetComponentsInChildren<Card>();
    //    foreach (Card card in cards) { card.RoundEnd(); }
    //    TowerManager[] towers = Board.GetComponentsInChildren<TowerManager>();
    //    foreach (TowerManager tower in towers) { tower.RoundEnd(); }
    //    GameUpdate();
    //}
}
