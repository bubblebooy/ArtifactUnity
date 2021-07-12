using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherTheFaithful : Ability
{
    // Summon Mega Creeps instead of Melee Creeps.
    // Your Melee Creep are summoned on the right. 
    // Deploy: Resummon your leftmost Melee Creep.

    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<Auras_e>(Auras)); // Use the Auras_e to make sure it is after GameUpdate_e 
    }

    void Auras(Auras_e e)
    {
        LaneManager lane = card.GetLane();
        if (card.hasAuthority)
        {
            lane.playerMeleeCreep = "Mega Creep";
            lane.playerCreepSummonDirection = false;
        }
        else
        {
            lane.enemyMeleeCreep = "Mega Creep";
            lane.enemyCreepSummonDirection = false;
        }  
    }
}
