using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GatherTheFaithful : Ability
{
    // Summon Mega Creeps instead of Melee Creeps.
    // Your Melee Creep are summoned on the right. 
    // Deploy: Resummon your leftmost Melee Creep.

    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<Auras_e>(Auras)); // Use the Auras_e to make sure it is after GameUpdate_e 
        Auras(new Auras_e());
        IEnumerable<Transform> slots = card.GetLane().transform
            .Find(card.GetSide())
            .Cast<Transform>()
            .Where(slot => slot.GetComponent<CardSlot>() != null);
        if (card.PlayerManager.Settings.values.variableSlots)
        {
            int middleSlot = (slots.Count() - 1) / 2;
            slots = slots.OrderBy(slot => Mathf.Abs(slot.GetSiblingIndex() - middleSlot + .1f));
        }
        slots = slots.Where(slot => slot.GetComponentInChildren<MeleeCreep>() != null);
        if(slots.Count() > 0)
        {
            slots.ToArray()[0].GetComponentInChildren<MeleeCreep>().DestroyCard(killed: false);
            card.GetLane().SummonCreeps(card.hasAuthority);
        }
    }

    void Auras(Auras_e e)
    {
        LaneManager lane = card.GetLane();
        if (card.hasAuthority)
        {
            lane.playerMeleeCreep = "Mega Creep";
            lane.playerCreepSummonForward = false;
        }
        else
        {
            lane.enemyMeleeCreep = "Mega Creep";
            lane.enemyCreepSummonForward = false;
        }  
    }

}
