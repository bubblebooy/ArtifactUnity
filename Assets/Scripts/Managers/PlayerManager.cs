using System.Collections;
using System.Collections.Generic;
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
    public GameObject laneCreep; // dict of all creeps

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

    }

    //[Server]
    //public override void OnStartServer()
    //{

    //}

    //[Command]
    //void CmdRandomSeed()
    //{
    //    RpcSetRandomSeed((int)System.DateTime.Now.Ticks);
    //}
    [ClientRpc]
    void RpcSetRandomSeed(int seed)
    {
        UnityEngine.Random.InitState(seed);
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
        if (isServer && hasAuthority) {
            RpcSetRandomSeed((int)System.DateTime.Now.Ticks);
            RpcRandomSetTurn();
        } // works but is called 2x without the hasAuthority
        

        for (int i=0; i < 5; i++)
        {
            GameObject card = Instantiate(cards[rand.Next(0, cards.Count)], new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(card, connectionToClient);
            RpcShowCard(card, "Dealt");
        }
        for (int i= 0; i < 5; i++)
        {
            GameObject hero = Instantiate(heroes[i], new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(hero, connectionToClient);
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
        RpcNextTurn();
        RpcPass();
    }

    [ClientRpc]
    void RpcPass()
    {
        GameManager.Passed();    
    }

    [ClientRpc]
    void RpcNextTurn()
    {
        PlayerManager pm = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        pm.IsMyTurn = !pm.IsMyTurn;
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
            RpcShowCard(card, "Dealt");
        }
        RpcGameChangeState("ResolveDeploy");
    }

    [Command]
    public void CmdDeployHero(GameObject card, List<string> targetLineage)
    {
        RpcMoveCard(card, targetLineage);
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
        GameObject card = Instantiate(laneCreep, new Vector2(0, 0), Quaternion.identity);
        NetworkServer.Spawn(card, connectionToClient);
        RpcSpawnLaneCreeps(lane, card);
    }

    [ClientRpc]
    void RpcSpawnLaneCreeps(GameObject lane, GameObject creep)
    {
        foreach (Transform slot in lane.transform.Find(hasAuthority ? "PlayerSide" : "EnemySide"))
        {
            if (slot.childCount == 0)
            {
                creep.transform.SetParent(slot, false);
                break;
            }
        }
    }

    public void PlayCard(GameObject card, List<string> targetLineage)
    {
        CmdPlayCard(card, targetLineage);     
    }

    [Command]
    void CmdPlayCard(GameObject card, List<string> targetLineage)
    {
        RpcMoveCard(card, targetLineage);
        RpcPlayCard(card);  // this prob needs to pass the card so on play effects can happen
        RpcNextTurn(); // This should be moved to so that the card call it (or not if quick)
    }

    [ClientRpc]
    void RpcGameChangeState(string stateRequest)
    {
        GameManager.ChangeGameState(stateRequest);
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
    void RpcPlayCard(GameObject card)
    {
        card.GetComponent<Card>().OnPlay();
        GameManager.CardPlayed();
    }

    [ClientRpc]
    void RpcMoveCard(GameObject card, List<string> targetLineage)
    {
        Transform target = GameObject.Find("Main Canvas").transform;
        foreach (string s in targetLineage)
        {
            if (hasAuthority)
            {
                target = target.Find(s);
            }
            else
            {
                string _s = s;
                _s = _s.Replace("Player", "ENEMY");
                _s = _s.Replace("Enemy", "Player");
                _s = _s.Replace("ENEMY", "Enemy");
                target = target.Find(_s);
            }

        }
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
    }

    [Command]
    public void CmdEndCombat()
    {
        RpcGameChangeState("Shop");
    }
}
