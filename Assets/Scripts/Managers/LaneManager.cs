using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LaneManager : NetworkBehaviour
{

    public bool combated = false;
    public GameManager GameManager;

    public override void OnStartClient()
    {
        base.OnStartClient();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
            //Units with trample combat 1st
            //System.Array.Sort(units, 
            //    (Unit a, Unit b) => 
            //    a.quickstrike ?
            //        ( b.quickstrike ? 0 : -1) :
            //        ( b.quickstrike ? 1 : 0)
            //    );

            // precombat(quick)
            foreach (Unit unit in units) { unit.PreCombat(quick: true); }
            foreach (Unit unit in units) { unit.Combat(quick: true); }
            foreach (Unit unit in units) { unit.quickstrikeDead(); }
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

    public PlayerManager GetPlayerManager()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        return networkIdentity.GetComponent<PlayerManager>();
    }
}
