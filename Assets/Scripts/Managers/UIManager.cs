using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UIManager : NetworkBehaviour
{
    public PlayerManager PlayerManager;
    public GameManager GameManager;
    public GameObject Board;
    public GameObject Button;
    public GameObject PhaseText;
    public GameObject PlayerHand;
    public GameObject PlayerFountain;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Board = GameObject.Find("Board");
        //Button = GameObject.Find("DrawCards");

    }

    public void UpdatePhaseText()
    {
        PhaseText.GetComponent<Text>().text = GameManager.GameState + " : " + UnityEngine.Random.value.ToString();
    }

    public void UpdateButtonText(string str)
    {
        Button.GetComponentInChildren<Text>().text = str;
    }

    public void ButtonInteractable(bool b)
    {
        Button.GetComponent<Button>().interactable = b;
    }

    public void ButtonInteractable()
    {
        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        Button.GetComponent<Button>().interactable = PlayerManager.IsMyTurn;
    }

    public void ZoomHand(bool Hand)
    {
        if (Hand)
        {
            PlayerHand.transform.localScale = new Vector3(1f, 1f, 1);
            PlayerFountain.transform.localScale = new Vector3(.3f, .3f, 1);;
        }
        else
        {
            PlayerHand.transform.localScale = new Vector3(0.3f, 0.3f, 1);
            PlayerFountain.transform.localScale = new Vector3(1f, 1f, 1);;
        }

    }

    public void Flop(int flop)
    {
        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        LaneManager[] lanes = Board.GetComponentsInChildren<LaneManager>();
        foreach (LaneManager lane in lanes)
        {
            if (flop == -1 ||
               (flop == 0 && lane.name == "MidLane") ||
               (flop == 1 && lane.name == (PlayerManager.IsMyTurn ? "LeftLane" : "RightLane")) ||
               (flop == 2 && lane.name == (PlayerManager.IsMyTurn ? "RightLane" : "LeftLane")))
            {
                lane.gameObject.transform.Find("Coverd").GetComponent<Image>().enabled = false;
                continue;
            }
            lane.gameObject.transform.Find("Coverd").GetComponent<Image>().enabled = true;
        }
    }

}
