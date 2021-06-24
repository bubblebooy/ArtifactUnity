using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject CardSlotPrefab;

    public GameManager GameManager;
    public GameObject PlayerArea;
    public GameObject EnemyArea;
    public GameObject PlayerDeck;
    public GameObject EnemyDeck;
    public GameObject PlayerOverDraw;
    public GameObject EnemyOverDraw;
    public GameObject PlayerFountain;
    public GameObject EnemyFountain;
    public GameObject PlayerGold;
    public GameObject EnemyGold;
    public GameObject Board;
    public Settings Settings;

    public List<GameObject> deck;
    public List<GameObject> heroes;
    public List<GameObject> items;

    public bool debug = true;

    System.Random rand = new System.Random();

    public bool IsMyTurn = true;

    private int cardSlotID = 0;

    //[SyncVar]
    //int expamle = 0;

    //private void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}

    //private void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}

    //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    Settings = FindObjectOfType<Settings>();
    //}

    public override void OnStartClient()
    {
        base.OnStartClient();
        DontDestroyOnLoad(gameObject);
        deck =  CardList.cardDict.Values.ToList();
        heroes = CardList.heroDict.Values.ToList();
        items = CardList.itemDict.Values.ToList();
    }

    private void Start()
    {
        Settings = FindObjectOfType<Settings>();
    }



    [ClientRpc]
    void RpcInitialize()
    {
        GameManager = GameObject.Find("GameManager")?.GetComponent<GameManager>();
        PlayerArea = GameObject.Find("PlayerArea");
        EnemyArea = GameObject.Find("EnemyArea");
        PlayerDeck = GameObject.Find("PlayerDeck");
        EnemyDeck = GameObject.Find("EnemyDeck");
        PlayerOverDraw = GameObject.Find("PlayerOverDraw");
        EnemyOverDraw = GameObject.Find("EnemyOverDraw");
        PlayerOverDraw.GetComponent<OverDrawManager>().handArea = PlayerArea;
        EnemyOverDraw.GetComponent<OverDrawManager>().handArea = EnemyArea;
        PlayerFountain = GameObject.Find("PlayerFountain");
        EnemyFountain = GameObject.Find("EnemyFountain");
        PlayerGold = GameObject.Find("PlayerGold");
        EnemyGold = GameObject.Find("EnemyGold");
        Board = GameObject.Find("Board");
        Settings?.Initialize();
    }
    [ClientRpc]
    void RpcInitializeLate()
    {
        foreach (LaneManager lane in Board.GetComponentsInChildren<LaneManager>())
        {
            lane.gameObject.GetComponent<LaneScroll>().Initialize();
        }
    }


    [ClientRpc]
    void RpcSetRandomSeed(int seed)
    {
        UnityEngine.Random.InitState(seed);
        GameManager.gameObject.GetComponent<GameHistory>().randomSeed = seed;
        print("RpcSetRandomSeed");
    }
    [ClientRpc]
    void RpcRandomSetTurn()
    {
        print("RpcRandomSetTurn");
        PlayerManager pm = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        pm.IsMyTurn = UnityEngine.Random.value > 0.5;
        if (isClientOnly)
        {
            pm.IsMyTurn = !pm.IsMyTurn;
        }
    }
   
    [Command]
    public void CmdStartGame()
    {
        RpcInitialize();
        if (isServer && hasAuthority) {
            RpcSetRandomSeed((int)System.DateTime.Now.Ticks);
            RpcRandomSetTurn();
        } // works but is called 2x without the hasAuthority

        GameObject card;
        List<GameObject> _deck = deck.OrderBy(a => rand.Next()).ToList();
        foreach(GameObject _card in _deck)
        {
            card = Instantiate(_card, new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(card, connectionToClient);
            RpcShowCard(card, "Deck");
        }  
        for (int i=0; i < Settings.values.startingHand; i++)
        {
            RpcDrawCard();
        }
        //*******For test alway draw the last card
        if (debug)
        {
            card = Instantiate(deck[deck.Count - 1], new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(card, connectionToClient);
            RpcShowCard(card, "Dealt");
            card = Instantiate(items[items.Count - 1], new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(card, connectionToClient);
            RpcShowCard(card, "Dealt");
            for(int i = 0; i < 3; i++)
            {
                card = Instantiate(items[i], new Vector2(0, 0), Quaternion.identity);
                NetworkServer.Spawn(card, connectionToClient);
                RpcShowCard(card, "Dealt");
            }
        }
        //*****************************************//
        GameObject cardSlot;
        foreach(LaneManager lane in GameObject.Find("Board").GetComponentsInChildren<LaneManager>())
        {
            for (int i = 0; i < Settings.values.numberOfSlots; i++)
            {
                cardSlot = Instantiate(CardSlotPrefab, new Vector2(0, 0), Quaternion.identity);
                NetworkServer.Spawn(cardSlot, connectionToClient);
                RpcNewCardSlot(lane.gameObject, cardSlot, i);
            }
        }
        GameObject hero;
        for (int i= 0; i < 5; i++)
        {
            hero = Instantiate(heroes[i], new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(hero, connectionToClient);
            RpcShowCard(hero, "Hero");
            RpcSetHeroRespawn(hero, Mathf.Max(0, i - 2));
        }
        RpcGameChangeState("Flop");
        RpcInitializeLate();
    }

    [ClientRpc]
    void RpcNewCardSlot(GameObject lane, GameObject cardSlot, int siblingIndex)
    {
        cardSlot.transform.SetParent(lane.transform.Find(hasAuthority ? "PlayerSide" : "EnemySide"), false);
        cardSlot.transform.SetSiblingIndex(siblingIndex);
        cardSlot.name = $"CardSlot ({cardSlotID})";
        cardSlotID += 1;
        //lane.GetComponent<LaneScroll>().SlotAddedRemoved();
    }


    [ClientRpc]
    void RpcSetHeroRespawn(GameObject hero, int respawn)
    {
        hero.GetComponent<Hero>().respawn = respawn;
    }

    [Command]
    public void CmdPass()
    {
        RpcPass();
        RpcNextTurn(quickcast:false);
        
    }

    [ClientRpc]
    void RpcPass()
    {
        GameManager.Passed();    
    }

    [ClientRpc]
    void RpcNextTurn(bool quickcast)
    {
        if (!quickcast)
        {
            PlayerManager pm = NetworkClient.connection.identity.GetComponent<PlayerManager>();
            pm.IsMyTurn = !pm.IsMyTurn;
        }
        GameManager.NextTurn();
    }

    [Command]
    public void CmdFinnishedShopping(int skipGold)
    {
        RpcGainGold(skipGold);
        RpcGameChangeState("Deploy");
    }

    [ClientRpc]
    void RpcGainGold(int gold)
    {
        GoldManager goldManager = (hasAuthority ? PlayerGold : EnemyGold).GetComponent<GoldManager>();
        goldManager.gold += gold;
    }
    [ClientRpc]
    void RpcSpendGold(int gold)
    {
        GoldManager goldManager = (hasAuthority ? PlayerGold : EnemyGold).GetComponent<GoldManager>();
        goldManager.gold -= gold;
        if(hasAuthority && GameManager.GameState == "Shop")
        {
            GameObject.Find("Shop").GetComponent<ShopManager>().UpdateShop();
        }
    }

    float drawRemainder = 0;
    [Command]
    public void CmdDeploy()
    {
        drawRemainder = Settings.values.cardDraw + drawRemainder;
        int draw = (int)drawRemainder;
        drawRemainder -= draw;
        for (int i = 0; i < draw; i++)
        {
            RpcDrawCard();
        }
        RpcGameChangeState("ResolveDeploy");
    }

    [Command]
    public void CmdDrawCards(int numberOfDraws)
    {
        for (int i = 0; i < numberOfDraws; i++)
        {
            RpcDrawCard();
        }
    }
    [ClientRpc]
    public void RpcDrawCard()
    {
        if (hasAuthority)
        {
            if(PlayerDeck.transform.childCount == 0) { return; }
            GameObject card = PlayerDeck.transform.GetChild(PlayerDeck.transform.childCount - 1).gameObject;
            card.transform.SetParent(PlayerOverDraw.transform, false);
            card.transform.rotation = Quaternion.identity;
        }
        else
        {
            if (EnemyDeck.transform.childCount == 0) { return; }
            GameObject card = EnemyDeck.transform.GetChild(EnemyDeck.transform.childCount - 1).gameObject;
            card.transform.SetParent(EnemyOverDraw.transform, false);
            card.transform.rotation = Quaternion.identity;
        }
    }

    [Command]
    public void CmdBuyItem(string itemString)
    {
        GameObject item = Instantiate(CardList.itemDict[itemString], new Vector2(0, 0), Quaternion.identity);
        NetworkServer.Spawn(item, connectionToClient);
        RpcShowCard(item, "Dealt");
        RpcSpendGold(item.GetComponent<IItem>().gold);
    }

    [Command]
    public void CmdDeployHero(GameObject card, List<string> targetLineage)
    {
        RpcPlaceCard(card, targetLineage);
    }

    [Command]
    public void CmdPlay()
    {
        RpcGameChangeState("Play");
    }

    [Command]
    public void CmdGameChangeState(string state)
    {
        RpcGameChangeState(state);
    }

    public void SpawnLaneCreeps(GameObject lane)
    {
        CmdSpawnLaneCreeps(lane);
    }

    [Command]
    void CmdSpawnLaneCreeps(GameObject lane)
    {
        //https://answers.unity.com/questions/1063917/command-cant-pass-gameobject-parameter-from-remote.html
        GameObject card = Instantiate(CardList.cardDict["Melee Creep"], new Vector2(0, 0), Quaternion.identity);
        NetworkServer.Spawn(card, connectionToClient);
        //RpcOnSpawn(card);
        RpcSpawnLaneCreeps(lane, card);
        CardPlayed_e cardPlayed_e = new CardPlayed_e();
        cardPlayed_e.card = card;
        RpcPlayCard(cardPlayed_e, false);
    }

    [ClientRpc]
    void RpcSpawnLaneCreeps(GameObject lane, GameObject creep)
    {
        foreach (Transform slot in lane.transform.Find(hasAuthority ? "PlayerSide" : "EnemySide"))
        {
            if (slot.childCount == 0)
            {
                creep.transform.SetParent(slot, false);
                creep.GetComponent<Card>().isDraggable = false;
                break;
            }
        }
    }

    [Command]
    public void CmdSummon(string unit, List<string> targetLineage) 
    {
        GameObject card = Instantiate(CardList.cardDict[unit], new Vector2(0, 0), Quaternion.identity);
        NetworkServer.Spawn(card, connectionToClient);
        //RpcOnSpawn(card);
        RpcPlaceCard(card, targetLineage);
        CardPlayed_e cardPlayed_e = new CardPlayed_e();
        cardPlayed_e.card = card;
        RpcPlayCard(cardPlayed_e, false);
    }

    [Command]
    public void CmdCloneToPlay(GameObject unit, List<string> targetLineage)
    {
        GameObject card;
        NetworkClient.GetPrefab(unit.GetComponent<NetworkIdentity>().assetId, out card);
        card = Instantiate(card, new Vector2(0, 0), Quaternion.identity);
        NetworkServer.Spawn(card, connectionToClient);
        RpcClone(original: unit, clone: card);
        Hero hero = unit.GetComponent<Hero>();
        if(hero != null)
        {
            foreach (Transform itemTransform in hero.items.transform)
            {
                GameObject item;
                NetworkClient.GetPrefab(itemTransform.gameObject.GetComponent<NetworkIdentity>().assetId, out item);
                item = Instantiate(item, new Vector2(0, 0), Quaternion.identity);
                NetworkServer.Spawn(item, connectionToClient);
                RpcItemCloneSetParent(item, card);
                RpcClone(original: itemTransform.gameObject, clone: item);
            }
        }
        RpcPlaceCard(card, targetLineage);
        CardPlayed_e cardPlayed_e = new CardPlayed_e();
        cardPlayed_e.card = card;
        RpcPlayCard(cardPlayed_e, false);
    }
    [ClientRpc]
    void RpcItemCloneSetParent(GameObject item, GameObject hero)
    {
        item.transform.SetParent(hero.GetComponent<Hero>().items.transform, false);
    }

    public void CloneToHand(GameObject unit, string color = null, bool ephemeral = false, GameObject validLane = null)
    {
        CmdCloneToHand(unit, color, ephemeral, validLane);
    }

    [Command]
    public void CmdCloneToHand(GameObject unit, string color, bool ephemeral, GameObject validLane)
    {
        GameObject card;
        NetworkClient.GetPrefab(unit.GetComponent<NetworkIdentity>().assetId, out card);
        card = Instantiate(card, new Vector2(0, 0), Quaternion.identity);
        NetworkServer.Spawn(card, connectionToClient);
        RpcClone(original: unit, clone: card);
        RpcShowCard(card, "Dealt");
        RpcModifyCard(card, color, ephemeral, validLane);
    }

    [ClientRpc]
    public void RpcClone(GameObject original, GameObject clone)
    {
        original.GetComponent<Unit>()?.CardUpdate(); // this gets rid of any Aura effects
        clone.GetComponent<Card>().Clone(original);
        GameManager.GameUpdate();
    }

    [ClientRpc]
    public void RpcModifyCard(GameObject card, string color, bool ephemeral, GameObject validLane)
    {
        Card _card = card.GetComponent<Card>();
        if (!string.IsNullOrEmpty(color))
        {
            _card.color = color;
            _card.OnValidate();
        }
        if (ephemeral)
        {
            card.gameObject.AddComponent<Ephemeral>();
        }
        if(validLane != null)
        {
            VaildLane v = card.gameObject.AddComponent<VaildLane>();
            v.lane = validLane;
        }
        
    }

    public void PlayCard(CardPlayed_e cardPlayed_e, List<string> targetLineage)
    {
        CmdPlayCard(cardPlayed_e, targetLineage);     
    }
    public void PlayCard(CardPlayed_e cardPlayed_e, List<string> targetLineage, List<List<string>> targetLineages)
    {
        CmdPlayTargetedCard(cardPlayed_e, targetLineage, targetLineages);
    }

    [Command]
    void CmdPlayCard(CardPlayed_e cardPlayed_e, List<string> targetLineage)
    {
        RpcPlaceCard(cardPlayed_e.card, targetLineage);
        RpcPayForCard(cardPlayed_e.card);
        RpcPlayCard(cardPlayed_e, true); 
        RpcNextTurn(cardPlayed_e.card.GetComponent<Card>().quickcast);
    }
    [Command]
    void CmdPlayTargetedCard(CardPlayed_e cardPlayed_e, List<string> targetLineage, List<List<string>> targetLineages)
    {
        RpcPlaceCard(cardPlayed_e.card, targetLineage);
        RpcPayForCard(cardPlayed_e.card);
        RpcPlayTargetedCard(cardPlayed_e, targetLineages);
        RpcNextTurn(cardPlayed_e.card.GetComponent<Card>().quickcast);
    }

    [ClientRpc]
    void RpcGameChangeState(string stateRequest)
    {
        GameManager.ChangeGameState(stateRequest);
    }

    [ClientRpc]
    void RpcPayForCard(GameObject card)
    {
        card.GetComponent<Card>().PayMana();
    }

    [ClientRpc]
    void RpcPlayCard(CardPlayed_e cardPlayed_e, bool gameManagerCardPlayed)
    {
        cardPlayed_e.card.GetComponent<Card>().OnPlay();
        if (gameManagerCardPlayed)
        {
            StartCoroutine(GameManager.CardPlayed(cardPlayed_e));
        }
    }

    [ClientRpc]
    void RpcPlayTargetedCard(CardPlayed_e cardPlayed_e, List<List<string>> targetLineages)
    {
        List<GameObject> targets = targetLineages.Select((lineage) => LineageToTransform(lineage).gameObject).ToList();
        cardPlayed_e.card.GetComponent<ITargets>().OnActivate(targets);
        StartCoroutine(GameManager.CardPlayed(cardPlayed_e));
    }

    public void ActivateAbility(GameObject card, int abilityIndex, bool quickcast = false)
    {
        CmdActivateAbility(card, abilityIndex, quickcast);
    }

    public void ActivateTargetAbility(GameObject card, int abilityIndex, List<List<string>> targetLineages, bool quickcast = false)
    {
        CmdActivateTargetAbility(card, abilityIndex, targetLineages, quickcast);
    }

    [Command]
    void CmdActivateAbility(GameObject card, int abilityIndex, bool quickcast)
    {
        RpcActivateAbility(card, abilityIndex);  // this prob needs to pass the card so on play effects can happen
        RpcNextTurn(quickcast); // This should be moved to so that the card call it (or not if quick)
    }
    [Command]
    void CmdActivateTargetAbility(GameObject card, int abilityIndex, List<List<string>> targetLineages, bool quickcast)
    {
        RpcActivateTargetAbility(card, abilityIndex, targetLineages);  
        RpcNextTurn(quickcast);
    }

    [ClientRpc]
    void RpcActivateAbility(GameObject card, int abilityIndex)
    {
        card.GetComponent<AbilitiesManager>().OnActivate(abilityIndex);
        StartCoroutine(GameManager.AbilityActivated(card, abilityIndex)); 
    }
    [ClientRpc]
    void RpcActivateTargetAbility(GameObject card, int abilityIndex, List<List<string>> targetLineages)
    {
        List<GameObject> targets = targetLineages.Select((lineage) => LineageToTransform(lineage).gameObject).ToList();
        card.GetComponent<AbilitiesManager>().OnActivate(abilityIndex, targets);
        StartCoroutine(GameManager.AbilityActivated(card, abilityIndex));
    }

    public void ActivateTowerEnchantment(GameObject Lane, string side, int enchantmentIndex, List<List<string>> targetLineages, bool quickcast = false)
    {
        CmdActivateTowerEnchantment(Lane, side, enchantmentIndex, targetLineages, quickcast);
    }

    [Command]
    void CmdActivateTowerEnchantment(GameObject Lane, string side, int enchantmentIndex, List<List<string>> targetLineages, bool quickcast)
    {
        RpcActivateTowerEnchantment(Lane, side, enchantmentIndex, targetLineages);  // this prob needs to pass the card so on play effects can happen
        RpcNextTurn(quickcast); // This should be moved to so that the card call it (or not if quick)
    }

    [ClientRpc]
    void RpcActivateTowerEnchantment(GameObject Lane, string side, int enchantmentIndex, List<List<string>> targetLineages)
    {
        List<GameObject> targets = targetLineages.Select((lineage) => LineageToTransform(lineage).gameObject).ToList();
        if (!hasAuthority)
        {
            side = side == "PlayerSide" ? "EnemySide" : "PlayerSide";
        }
        ITargets towerEnchantment = Lane.transform.Find(side + "/Enchantments").GetChild(enchantmentIndex).GetComponent<ITargets>();
        towerEnchantment.OnActivate(targets);
        StartCoroutine(GameManager.TowerEnchantmentActivated(Lane, enchantmentIndex, side));
    }


    [ClientRpc]
    void RpcShowCard(GameObject card, string type)
    {
        if (type == "Deck")
        {
            card.GetComponent<Card>().revealed = false;
            card.GetComponent<Card>().CardUIUpdate(new GameUpdateUI_e());
            if (hasAuthority)
            {
                card.transform.SetParent(PlayerDeck.transform, false);
            }
            else
            {
                card.transform.SetParent(EnemyDeck.transform, false);
            }
            
        }
        if (type == "Dealt")
        {
            if (hasAuthority)
            {
                card.transform.SetParent(PlayerOverDraw.transform, false);
            }
            else
            {
                card.transform.SetParent(EnemyOverDraw.transform, false);
                card.GetComponent<Card>().CardUIUpdate(new GameUpdateUI_e());
            }
        }
        else if (type == "Hero")
        {
            if (hasAuthority)
            {
                card.transform.SetParent(PlayerFountain.transform, false);
            }
            else
            {
                card.transform.SetParent(EnemyFountain.transform, false);
            }
        }
    }

    [ClientRpc]
    void RpcPlaceCard(GameObject card, List<string> targetLineage)
    {
        Transform target = LineageToTransform(targetLineage);
        card.transform.SetParent(target, false);
        ////////  CHECK TO SEE IF THIS WORKS IN THE PLAYER?
        Transform t = card.transform;
        string lineage = "";
        while (t.name != t.root.name)
        {
            lineage = t.name + "/" + lineage;
            t = t.parent;
        }
        Debug.Log(lineage);
        //////// CHECK TO SEE IF THIS WORKS IN THE PLAYER?
        card.transform.position = target.position;
        card.GetComponent<Card>().staged = true;
    }

    [Command]
    public void CmdEndCombat()
    {
        RpcGameChangeState("Shop");
    }
    [Command]
    public void CmdGameOver()
    {
        RpcGameChangeState("GameOver");
    }

    Transform LineageToTransform(List<string> lineage)
    {
        if(lineage is null) { return null; }
        Transform transform = GameObject.Find("Main Canvas").transform;
        foreach (string s in lineage)
        {
            if (hasAuthority)
            {
                transform = transform.Find(s);
            }
            else
            {
                string _s = s;
                _s = _s.Replace("Player", "ENEMY");
                _s = _s.Replace("Enemy", "Player");
                _s = _s.Replace("ENEMY", "Enemy");
                transform = transform.Find(_s);
            }

        }
        return transform;
    }
}
