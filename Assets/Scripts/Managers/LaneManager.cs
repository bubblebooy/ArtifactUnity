using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LaneManager : NetworkBehaviour
{

    public bool combated = false;
    GameManager GameManager;

    private void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public IEnumerator Combat()
    {
        if (!combated)
        {
            Unit[] units = gameObject.GetComponentsInChildren<Unit>();
            //List<Unit> units = new List<Unit>(gameObject.GetComponentsInChildren<Unit>());
            TowerManager[] towers = gameObject.GetComponentsInChildren<TowerManager>();

            foreach (Unit unit in units)
            {
                unit.Combat(true);
            }
            foreach (Unit unit in units)
            {
                unit.quickstrikeDead();
            }
            foreach (Unit unit in units)
            {
                unit.Combat();
            }
            foreach (TowerManager tower in towers) { tower.TowerUpdate(); } //Artifact Foundry checks for Game over here. Should it?
            GameManager.GameUpdate();
            // Game over check
            Debug.Log(gameObject.name + $" : {units.Length}");
        }
        yield return new WaitForSeconds(0.5f);
    }
}
