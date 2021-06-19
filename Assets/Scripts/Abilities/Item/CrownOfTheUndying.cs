using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CrownOfTheUndying : Ability
{
    protected override void Awake()
    {
        base.Awake();
        inPlayEvents.Add(GameEventSystem.Register<EndCombatPhase_e>(EndCombatPhase));
    }

    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<EndCombatPhase_e>(EndCombatPhase));
    }

    public void EndCombatPhase(EndCombatPhase_e e)
    {
        CardSlot cardSlot = card.GetCardSlot();
        Transform side = card.transform.parent.parent;
        if(side == null) { return; }
        int slotNumber = cardSlot.transform.GetSiblingIndex();
        int numberOfSlots = side.GetComponentsInChildren<CardSlot>().Length;

        CardSlot[] slots = side.GetComponentsInChildren<CardSlot>();
        slots = slots.Where(x => x.GetComponentInChildren<Unit>() == null).ToArray();
        slots = slots.OrderBy(x => Mathf.Abs(x.transform.GetSiblingIndex() - (slotNumber - 0.1f))).ToArray();

        if(slots.Length > 0) {
            if (card.hasAuthority)
            {
                card.PlayerManager.CmdSummon("Zombie", Card.GetLineage(slots[0].transform));
            }
        }
    }
}


        //if ((slotNumber - 1 < numberOfSlots) &&
        //    side.GetChild(slotNumber - 1).GetComponentInChildren<Unit>() == null)
        //{
        //    destination = slotNumber - 1;
        //}
        //else if ((slotNumber + 1 < numberOfSlots) &&
        //    side.GetChild(slotNumber + 1).GetComponentInChildren<Unit>() == null)
        //{
        //    destination = slotNumber + 1;
        //}
        //else { return; }