using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    public GameManager GameManager;
    public GameObject PlayerArea;
    public GameObject EnemyArea;
    public GameObject PlayerFountain;
    public GameObject EnemyFountain;
    public GameObject Board;
    

    public List<GameObject> cards = new List<GameObject>();
    public List<GameObject> heroes = new List<GameObject>();
    private Dictionary<string, GameObject> cardDict; // = cards.ToDictionary(x => x.name, x => x);
    //public GameObject laneCreep; // dict of all creeps

    System.Random rand = new System.Random();

    public bool IsMyTurn = true;

    //[SyncVar]
    //int expamle = 0;


    // Start is called before the first frame update
    public override void OnStartClient()
    {
        base.OnStartClient();

        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        PlayerArea = GameObject.Find("PlayerArea");
        EnemyArea = GameObject.Find("EnemyArea");
        PlayerFountain = GameObject.Find("PlayerFountain");
        EnemyFountain = GameObject.Find("EnemyFountain");
        Board = GameObject.Find("Board");

        cardDict = cards.ToDictionary(x => x.name, x => x);

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

    //[ClientRpc]
    //void RpcOnSpawn(GameObject card)
    //{
    //    card.GetComponent<Card>().OnSpawn();
    //}
    

    [Command]
    public void CmdStartGame()
    {
        if (isServer && hasAuthority) {
            RpcSetRandomSeed((int)System.DateTime.Now.Ticks);
            RpcRandomSetTurn();
        } // works but is called 2x without the hasAuthority

        GameObject card;
        for (int i=0; i < 5; i++)
        {
            card = Instantiate(cards[rand.Next(0, cards.Count)], new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(card, connectionToClient);
            //RpcOnSpawn(card);
            RpcShowCard(card, "Dealt");
        }
        //*******For test alway draw the last card
        card = Instantiate(cards[cards.Count-1], new Vector2(0, 0), Quaternion.identity);
        NetworkServer.Spawn(card, connectionToClient);
        //RpcOnSpawn(card);
        RpcShowCard(card, "Dealt");
        //*****************************************//
        for (int i= 0; i < 5; i++)
        {
            GameObject hero = Instantiate(heroes[i], new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(hero, connectionToClient);
            //RpcOnSpawn(hero);
            //can I set deploy order here? Mathf.Max(0, i - 2);
            RpcShowCard(hero, "Hero");
            RpcSetHeroRespawn(hero, Mathf.Max(0, i - 2));
        }
        //CmdRandomSeed();
        RpcGameChangeState("Flop");
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
        //Debug.Log(pm.IsMyTurn);
    }

    [Command]
    public void CmdFinnishedShopping()
    {
        RpcGameChangeState("Deploy");
    }

    [Command]
    public void CmdDeploy()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject card = Instantiate(cards[rand.Next(0, cards.Count)], new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(card, connectionToClient);
            //RpcOnSpawn(card);
            RpcShowCard(card, "Dealt");
        }
        RpcGameChangeState("ResolveDeploy");
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
        GameObject card = Instantiate(cardDict["Melee Creep"], new Vector2(0, 0), Quaternion.identity);
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
        GameObject card = Instantiate(cardDict[unit], new Vector2(0, 0), Quaternion.identity); 
        NetworkServer.Spawn(card, connectionToClient);
        //RpcOnSpawn(card);
        RpcPlaceCard(card, targetLineage);
        CardPlayed_e cardPlayed_e = new CardPlayed_e();
        cardPlayed_e.card = card;
        RpcPlayCard(cardPlayed_e, false);
    }


    public void PlayCard(CardPlayed_e cardPlayed_e, List<string> targetLineage)
    {
        CmdPlayCard(cardPlayed_e, targetLineage);     
    }
    public void PlayCard(CardPlayed_e cardPlayed_e, List<string> targetLineage, List<string> secondaryTargetLineage)
    {
        CmdPlayTargetedCard(cardPlayed_e, targetLineage, secondaryTargetLineage);
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
    void CmdPlayTargetedCard(CardPlayed_e cardPlayed_e, List<string> targetLineage, List<string> secondaryTargetLineage)
    {
        RpcPlaceCard(cardPlayed_e.card, targetLineage);
        RpcPayForCard(cardPlayed_e.card);
        RpcPlayTargetedCard(cardPlayed_e, secondaryTargetLineage);
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
    void RpcPlayTargetedCard(CardPlayed_e cardPlayed_e, List<string> secondaryTargetLineage)
    {
        Transform target = LineageToTransform(secondaryTargetLineage);
        cardPlayed_e.card.GetComponent<ITargets>().OnPlay(target.gameObject);
        StartCoroutine(GameManager.CardPlayed(cardPlayed_e));
    }

    public void ActivateAbility(GameObject card, int abilityIndex, bool quickcast = false)
    {
        CmdActivateAbility(card, abilityIndex, quickcast);
    }

    [Command]
    void CmdActivateAbility(GameObject card, int abilityIndex, bool quickcast)
    {
        RpcActivateAbility(card, abilityIndex);  // this prob needs to pass the card so on play effects can happen
        RpcNextTurn(quickcast); // This should be moved to so that the card call it (or not if quick)
    }
    [ClientRpc]
    void RpcActivateAbility(GameObject card, int abilityIndex)
    {
        card.GetComponent<AbilitiesManager>().OnActivate(abilityIndex);
        StartCoroutine(GameManager.AbilityActivated(card, abilityIndex)); 
    }

    public void ActivateTowerEnchantment(GameObject Lane, string side, int enchantmentIndex, List<string> targetLineage, List<string> secondaryTargetLineage = null, bool quickcast = false)
    {
        CmdActivateTowerEnchantment(Lane, side, enchantmentIndex, targetLineage, secondaryTargetLineage, quickcast);
    }

    [Command]
    void CmdActivateTowerEnchantment(GameObject Lane, string side, int enchantmentIndex, List<string> targetLineage, List<string> secondaryTargetLineage, bool quickcast)
    {
        RpcActivateTowerEnchantment(Lane, side, enchantmentIndex, targetLineage, secondaryTargetLineage);  // this prob needs to pass the card so on play effects can happen
        RpcNextTurn(quickcast); // This should be moved to so that the card call it (or not if quick)
    }

    [ClientRpc]
    void RpcActivateTowerEnchantment(GameObject Lane, string side, int enchantmentIndex, List<string> targetLineage, List<string> secondaryTargetLineage)
    {
        Transform target = LineageToTransform(targetLineage);
        Transform secondaryTarget = LineageToTransform(secondaryTargetLineage);
        if (!hasAuthority)
        {
            side = side == "PlayerSide" ? "EnemySide" : "PlayerSide";
        }
        ITargets towerEnchantment = Lane.transform.Find(side + "/Enchantments").GetChild(enchantmentIndex).GetComponent<ITargets>();
        towerEnchantment.OnPlay(target.gameObject, secondaryTarget?.gameObject);
        StartCoroutine(GameManager.TowerEnchantmentActivated(Lane, enchantmentIndex, side));
    }


    [ClientRpc]
    void RpcShowCard(GameObject card, string type)
    {
        if (type == "Dealt")
        {
            if (hasAuthority)
            {
                card.transform.SetParent(PlayerArea.transform, false);
            }
            else
            {
                card.transform.SetParent(EnemyArea.transform, false);
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
