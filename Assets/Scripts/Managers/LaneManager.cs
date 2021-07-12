using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class LaneManager : NetworkBehaviour
{

    public bool combated = false;
    public GameManager GameManager;
    public string playerMeleeCreep = "Melee Creep";
    public string enemyMeleeCreep = "Melee Creep";
    public bool playerCreepSummonForward = true;
    public bool enemyCreepSummonForward = true;

    public GameObject unitPlaceholder;

    public List<(System.Type, GameEventSystem.EventListener)> events = new List<(System.Type, GameEventSystem.EventListener)>();

    PlayerManager PlayerManager;

    public override void OnStartClient()
    {
        base.OnStartClient();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        events.Add(GameEventSystem.Register<EndCombatPhase_e>(EndCombatPhase));
        events.Add(GameEventSystem.Register<GameUpdate_e>(GameUpdate));
    }

    public IEnumerator Combat()
    {
        if (!combated)
        {
            Unit[] units = gameObject.GetComponentsInChildren<Unit>();
            //List<Unit> units = new List<Unit>(gameObject.GetComponentsInChildren<Unit>());
            TowerManager[] towers = gameObject.GetComponentsInChildren<TowerManager>();
            TowerEnchantment[] towerEnchantments = gameObject.GetComponentsInChildren<TowerEnchantment>();

            foreach (TowerEnchantment towerEnchantment in towerEnchantments)
            {
                towerEnchantment.Combat();
            }
            foreach (Unit unit in units) { unit.PreCombat(quick: true); }
            foreach (Unit unit in units) { unit.Combat(quick: true); }
            foreach (Unit unit in units) { unit.quickstrikeDead(); }
            //GameManager.GameUpdate(checkAlive: false);
            foreach (Unit unit in units) { unit.PreCombat(); }
            foreach (Unit unit in units) { unit.Combat(); }

            //Artifact Foundry checks for Game over here. Should it?
            //foreach (TowerManager tower in towers) { tower.TowerUpdate(); }
            GameManager.GameUpdate();
            // Game over check
            Debug.Log(gameObject.name + $" : {units.Length}");
        }
        yield return new WaitForSeconds(0.5f);
    }

    private void EndCombatPhase(EndCombatPhase_e e)
    {
        combated = false;
    }

    public void SummonCreeps()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();

        SummonCreeps(true);
        SummonCreeps(false);
    }

    public void SummonCreeps(bool playerSide)
    {
        IEnumerable<Transform> slots = transform.Find(playerSide ? "PlayerSide" : "EnemySide")
            .Cast<Transform>()
            .Where(slot => slot.GetComponent<CardSlot>() != null);

        if (PlayerManager.Settings.values.variableSlots)
        {
            int middleSlot = (slots.Count() - 1) / 2;
            //bool summonDirection;
            float orderSlots(Transform slot, bool summonDirection)
            {
                float value = Mathf.Abs(slot.GetSiblingIndex() - middleSlot + .1f);
                if (!summonDirection) { value *= -1; }
                if (!slot.gameObject.activeInHierarchy) { value += 1000; }
                return value;
            }
            slots = slots.OrderBy(slot => orderSlots(slot, playerSide ? playerCreepSummonForward : enemyCreepSummonForward));
        }
        else
        {
            if (!( playerSide ? playerCreepSummonForward : enemyCreepSummonForward)) { slots = slots.Reverse(); }
        }

        foreach (Transform slot in slots)
        {
            if (slot.childCount == 0)
            {
                UnitPlaceholder placeholder = Instantiate(unitPlaceholder, slot).GetComponent<UnitPlaceholder>();
                placeholder.placeholderCard = playerSide ? playerMeleeCreep : null;
                break;
            }
        }
    }


    public PlayerManager GetPlayerManager()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        return networkIdentity.GetComponent<PlayerManager>();
    }

    void GameUpdate(GameUpdate_e e)
    {
        playerMeleeCreep = "Melee Creep";
        enemyMeleeCreep  = "Melee Creep";
        playerCreepSummonForward = true;
        enemyCreepSummonForward = true;
    }
}
