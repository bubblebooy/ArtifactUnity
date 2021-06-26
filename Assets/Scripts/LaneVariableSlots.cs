using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class LaneVariableSlots : MonoBehaviour
{
    int minSlots;
    int maxSlots;

    int offset; // account for tower and enchantments

    Transform pool;

    public PlayerManager PlayerManager;

    public static Queue<GameObject[]> cardSlotPool = new Queue<GameObject[]>();

    GameObject playerSide;
    GameObject enemySide;

    public List<(System.Type, GameEventSystem.EventListener)> events = new List<(System.Type, GameEventSystem.EventListener)>();

    public void Initialize(Settings settings)
    {
        pool = GameObject.Find("Pool").transform;
        minSlots = settings.values.numberOfSlots;
        playerSide = transform.Find("PlayerSide").gameObject;
        enemySide = transform.Find("EnemySide").gameObject;

        events.Add(GameEventSystem.Register<VariableSlotsUpdate_e>(VariableSlotsUpdate));
    }

    void VariableSlotsUpdate(VariableSlotsUpdate_e e)
    {
        UpdateSlots();
    }

    void UpdateSlots()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        if (cardSlotPool.Count < 10)
        {
            for(int i = 0; i < 10; i++)
            {
                PlayerManager.CmdNewPoolCardSlot();
            }
            if (cardSlotPool.Count <= 2) { return; }
        }
        CardSlot[] startCap = null;
        CardSlot[] endCap = null;
        bool playerFull = true;
        bool enemyFull = true;

        CardSlot[] playerSlots = playerSide.GetComponentsInChildren<CardSlot>(true);
        CardSlot[] enemySlots = enemySide.GetComponentsInChildren<CardSlot>(true);
        Unit[] playerUnits = playerSlots.Select(slot => slot.GetComponentInChildren<Unit>(true)).ToArray();
        Unit[] enemyUnit = enemySlots.Select(slot => slot.GetComponentInChildren<Unit>(true)).ToArray();

        int slotCount = playerSlots.Length;
        
        if(playerSlots.Length != enemySlots.Length) { return; }

        for (int i = 0; i < playerSlots.Length; i++)
        {
            playerSlots[i].gameObject.SetActive(true);
            enemySlots[i].gameObject.SetActive(true);
            playerSlots[i].slotEnabled = true;
            enemySlots[i].slotEnabled = true;
            if (slotCount > minSlots)
            {
                if (playerUnits[i] is null && enemyUnit[i] is null)
                {
                    if (i == 0) { startCap = new CardSlot[] { playerSlots[i], enemySlots[i] }; }
                    else if (i == playerSlots.Length - 1) { endCap = new CardSlot[] { playerSlots[i], enemySlots[i] }; }
                    else if (slotCount > minSlots + 2)
                    {
                        playerSlots[i].transform.SetParent(null, false);
                        enemySlots[i].transform.SetParent(null, false);
                        Destroy(playerSlots[i]);
                        Destroy(enemySlots[i]);
                        //PlayerManager.CmdEnqueueCardSlot(playerSlots[i].gameObject);
                        slotCount--;
                        //GameManager.updateloop = true;
                    }
                    else
                    {
                        playerFull = enemyFull = false;
                    }
                }
                else if (playerUnits[i] == null)
                {
                    playerFull = false;
                }
                else if (enemyUnit[i] == null)
                {
                    enemyFull = false;
                }
            }
            else
            {
                GameManager.updateloop = true;
            }
        }
        if (!(startCap?.Length > 0))
        {
            startCap = cardSlotPool.Dequeue().Select(slot => slot.GetComponent<CardSlot>()).ToArray();
            startCap[0].transform.SetParent(playerSide.transform,false);
            startCap[1].transform.SetParent(enemySide.transform, false);
            startCap[0].transform.SetAsFirstSibling();
            startCap[1].transform.SetAsFirstSibling();
            slotCount++;
            //GameManager.updateloop = true;
        }
        if (!(endCap?.Length > 0))
        {
            endCap = cardSlotPool.Dequeue().Select(slot => slot.GetComponent<CardSlot>()).ToArray();
            endCap[0].transform.SetParent(playerSide.transform, false);
            endCap[1].transform.SetParent(enemySide.transform, false);
            endCap[0].transform.SetSiblingIndex(slotCount);
            endCap[1].transform.SetSiblingIndex(slotCount);
            slotCount++;
            //GameManager.updateloop = true;
        }
        if(!playerFull && !enemyFull)
        {
            startCap[0].gameObject.SetActive(false);
            startCap[1].gameObject.SetActive(false);
            endCap[0].gameObject.SetActive(false);
            endCap[1].gameObject.SetActive(false);
        }
        else
        {
            startCap[0].gameObject.SetActive(true);
            startCap[1].gameObject.SetActive(true);
            endCap[0].gameObject.SetActive(true);
            endCap[1].gameObject.SetActive(true);
        }
        startCap[0].slotEnabled = playerFull;
        startCap[1].slotEnabled = enemyFull;
        endCap[0].slotEnabled = playerFull;
        endCap[1].slotEnabled = enemyFull;
    }
}
