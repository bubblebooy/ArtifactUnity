using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

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
    public ShopManager Shop;
    public LaneManager[] lanes;

    public string GameState = "Setup";
    //public int[] PlayerTowerHealth = new int[] { 40, 40, 40, 80};
    //public int[] EnemyTowerHealth = new int[] { 40, 40, 40, 80 };
    private int playerTowersDestroyed = 0;
    private int enemyTowersDestroyed = 0;

    public bool CombatDirection = true;

    private int ReadyClicks = 0; // should I make this a bool?
    public int flop = 0;

    public bool zoomEnabled = true;


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
        Shop = FindObjectOfType<ShopManager>(includeInactive: true);
        GameEventSystem.Register<TowerDestroyed_e>(TowerDestroyed);
        lanes = Board.GetComponentsInChildren<LaneManager>();
    }

    public void ChangeGameState(string stateRequest)
    {
         // just in case
        if (ReadyClicks == 1)
        {
            switch (stateRequest)
            {
                case "Flop":
                    GameUpdate(stateRequest);
                    if (flop == 0)
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
                        PlayerManager.CmdGameChangeState("PlayHeroes");
                    }
                    break;
                case "Deploy":
                    GameUpdate(stateRequest);
                    RoundStart();
                    UIManager.ZoomHand(false);
                    GameState = "Deploy";
                    UIManager.ButtonInteractable(true);
                    UIManager.UpdateButtonText("Confirm");
                    break;
                case "ResolveDeploy":
                    ResolveDeploy();
                    PlayerManager.CmdGameChangeState("PlayHeroes");
                    break;
                case "PlayHeroes":
                    GameUpdate("ResolveDeploy");
                    PlayHeros();
                    PlayerManager.CmdPlay();
                    break;
                case "Play":
                    GameUpdate(stateRequest);
                    UIManager.ZoomHand(true);
                    GameState = "Play";
                    StartCoroutine(PlayStart());
                    //GameEventSystem.Event(new TurnStart_e());
                    //UIManager.UpdateButtonText("Pass");
                    //UIManager.ButtonInteractable();
                    break;
                case "Combat":
                    GameUpdate(stateRequest);
                    GameState = "Combat";
                    UIManager.ButtonInteractable(false);
                    StartCoroutine(Combat());
                    break;
                case "Shop":
                    GameUpdate(stateRequest);
                    GameState = "Shop";
                    UIManager.ButtonInteractable(false);
                    Shop.gameObject.SetActive(true);
                    Shop.StartShopping();
                    UIManager.UpdateButtonText(""); // $"Skip: +{5} Gold"
                    break;
                case "GameOver":
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
        GameEventSystem.Event(new NextTurn_e());
        UIManager.ButtonInteractable();
        if(GameState != "Combat")
        {
            GameEventSystem.Event(new TurnStart_e());
        }

    }

    IEnumerator Combat()
    {
        Debug.Log("COMBAT START");
        // Dont know if I need this // GameEventSystem.Event(new StartCombatPhase_e());
        //LaneManager[] lanes = Board.GetComponentsInChildren<LaneManager>();
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
        yield return new WaitForSeconds(0.1f);
        SummonLaneCreeps();
        if(GameState == "GameOver") { yield break;  }
        PlayerManager.CmdEndCombat();
        yield return new WaitForSeconds(0.2f);
        UIManager.ButtonInteractable(true);
        GameUpdate();
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
    }

    void PlayHeros()
    {
        // foreach play hero. Needs to be done after both side have moved their heroes
        Hero[] heroes = Board.GetComponentsInChildren<Hero>();
        // need both clients to do this in the same order
        heroes = heroes.OrderBy(hero => hero.GetSide() == (isClientOnly ? "PlayerSide" : "EnemySide")).ToArray();

        foreach (Hero hero in heroes)
        {
            if (hero.staged)
            {
                //Do I bounce here?
                print(hero.cardName);
                GameHistory.HeroDeployed(hero.gameObject);
                hero.OnPlay(new CardPlayed_e());
            }
            
        }
    }

    public void SummonLaneCreeps()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        foreach (LaneManager lane in lanes)
        {
            lane.SummonCreeps();
        }
    }

    public static bool updateloop;
    public void GameUpdate(bool checkAlive = true)
    {
        GameUpdate(GameState, checkAlive);
    }
    public void GameUpdate(string state, bool checkAlive = true)
    {
        updateloop = false;
        GameEventSystem.Event(new GameUpdate_e(state, checkAlive));
        GameEventSystem.Event(new Auras_e());
        GameEventSystem.Event(new AuraModifiers_e());
        GameEventSystem.Event(new DeathEffects_e(), unregister: true);
        GameEventSystem.Event(new VariableSlotsUpdate_e());
        GameEventSystem.Event(new GameUpdateUI_e());

        if (updateloop) { GameUpdate(state, checkAlive); }
        SummonPlacehoders();
        IsGameOver();
    }

    void SummonPlacehoders()
    {
        UnitPlaceholder[] unitPlaceholders = Board.GetComponentsInChildren<UnitPlaceholder>();
        unitPlaceholders = unitPlaceholders.Where(placeholder => !string.IsNullOrEmpty(placeholder.placeholderCard)).ToArray();
        List<string> units = unitPlaceholders.Select(placeholder => placeholder.placeholderCard).ToList();
        List<List<string>> targetLineages = unitPlaceholders.Select(placeholder => Card.GetLineage(placeholder.transform.parent)).ToList();

        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        PlayerManager.CmdSummonPlacehoders(units, targetLineages);

        foreach(UnitPlaceholder placeholder in unitPlaceholders)
        {
            placeholder.placeholderCard = null;
        }
    }

    private IEnumerator DelayedGameUpdate()
    {
        yield return new WaitForSeconds(0.1f);//new WaitForSeconds(0.1f);
        GameUpdate();
    }

    //
    void RoundStart() // BEFORE HEROS ARE DEPLOYED
    {
        GameEventSystem.Event(new RoundStart_e());
        GameUpdate();
    }

    IEnumerator PlayStart() // AFTER HEROS ARE DEPLOYED
    {
        yield return new WaitForSeconds(0.1f);
        GameEventSystem.Event(new PlayStart_e());
        yield return new WaitForSeconds(0.1f);
        UIManager.UpdateButtonText("Pass");
        UIManager.ButtonInteractable();
        GameEventSystem.Event(new TurnStart_e());
        GameUpdate();
        
    }

    void TowerDestroyed(TowerDestroyed_e e)
    {
        if (e.tower.playerTower)
        {
            playerTowersDestroyed += 1;
        }
        else
        {
            enemyTowersDestroyed += 1;
        }  
}

    void IsGameOver()
    {
        string gameOverString = "";
        if ( playerTowersDestroyed < 2 && enemyTowersDestroyed < 2 ) { return; }
        else if(playerTowersDestroyed >= 2 & enemyTowersDestroyed >= 2)
        {
            playerTowersDestroyed += 100;
            enemyTowersDestroyed += 100;
            gameOverString = "GAME OVER: Tie game";
        }
        else if (playerTowersDestroyed >= 2)
        {
            playerTowersDestroyed += 100;
            enemyTowersDestroyed -= 100;
            gameOverString = "GAME OVER: You Lost  :-(";
        }
        else if (enemyTowersDestroyed >= 2)
        {
            playerTowersDestroyed -= 100;
            enemyTowersDestroyed += 100;
            gameOverString = "GAME OVER: You Won !!!";
        }
        GameState = "GameOver";
        UIManager.UpdatePhaseText(gameOverString);
        print("gameOverString");
        UIManager.ButtonInteractable(false);
        //PlayerManager.CmdGameOver();
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
