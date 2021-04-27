using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PoisedToStrike : Spell
{
    int damage = 1;
    public override void OnPlay()
    {
        Transform side = transform.parent.Find(hasAuthority ? "EnemySide" : "PlayerSide");
        for(int i = 0; i < 3; i++)
        {
            List<Unit> units = side.GetComponentsInChildren<Unit>().ToList();
            if(units.Count == 0) { continue; }
            int leastHP = units.Select(x => x.health).Min();
            units = units.Where(x => x.health == leastHP).ToList();
            Unit rndTarget = units[Random.Range(0, units.Count)];
            rndTarget.Damage(damage, true);

            GameManager.GameUpdate();
        }

        DestroyCard();
    }

}
