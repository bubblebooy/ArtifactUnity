using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class DrawCards : NetworkBehaviour
{

    public PlayerManager PlayerManager;
    public GameManager GameManager;
    public GameObject Fountain;
    public GameObject Board;
    public Button button;


    public override void OnStartClient()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Fountain = GameObject.Find("PlayerFountain");
        Board = GameObject.Find("Board");
        button = gameObject.GetComponent<Button>();
    }

    public void OnClick()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        switch (GameManager.GameState)
        {
            case "Setup":
                SetupClick();
                break;
            case "Play":
                PassClick();
                break;
            case "Shop":
                break;
            case "Deploy":
                Deploy();
                break;
            case "Flop":
                Flop();
                break;
            default:
                break;
        }
    }

    void SetupClick()
    {
        PlayerManager.CmdStartGame();
        //GameManager.SummonLaneCreeps();
        button.interactable = false;
    }
    void PassClick()
    {
        PlayerManager.CmdPass();
    }
    void Deploy()
    {
        bool allDeployed = true;
        foreach (Hero hero  in Fountain.GetComponentsInChildren<Hero>())
        {
            if (hero.respawn <= 0) { allDeployed = false; }
        }
        if (allDeployed)
        {
            PlayerManager.CmdDeploy();
            button.interactable = false;
        }
    }
    void Flop()
    {
        int stagedCount = 0;
        foreach (Hero hero in Board.GetComponentsInChildren<Hero>())
        {
            if (hero.staged) { stagedCount++; }
        }
        if(stagedCount == 1)
        {
            PlayerManager.CmdGameChangeState("ResolveFlop");
            button.interactable = false;
        }
    }
}
