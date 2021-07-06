using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlingAxes : Ability
{
    //Round Start: Troll Warlord strikes its combat target for 1.
    public override void OnPlay()
    {
        inPlayEvents.Add(GameEventSystem.Register<PlayStart_e>(PlayStart));
    }

    void PlayStart(PlayStart_e e)
    {
        Unit combatTarget = card.GetCombatTarget();
        if(combatTarget != null)
        {
            card.Strike(card.GetCombatTarget(), 1);
        }
        else
        {
            bool player = card.GetSide() == "PlayerSide";
            TowerManager tower = card.GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
            card.Strike(tower, 1);
        }
    }
}
