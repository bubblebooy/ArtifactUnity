using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepReinforcements : Spell
{

    public override void OnPlay(CardPlayed_e cardPlayed_e) {
        base.OnPlay(cardPlayed_e);
        CardSlot cardSlot = gameObject.GetComponentInParent<CardSlot>();
        Unit target = transform.parent.GetComponent<Unit>();
        Transform side = target.transform.parent.parent;
        int slotNumber = target.transform.parent.GetSiblingIndex();
        int numberOfSlots = side.GetComponentsInChildren<CardSlot>().Length;
        int rnd = (Random.value > 0.5) ? -1 : 1;
        int destination;
        if ((slotNumber + rnd >= 0 || slotNumber + rnd < numberOfSlots) &&
            side.GetChild(slotNumber + rnd).GetComponentInChildren<Unit>() == null)
        {   destination = slotNumber + rnd; }
        else if ((slotNumber - rnd >= 0 || slotNumber - rnd < numberOfSlots) &&
            side.GetChild(slotNumber - rnd).GetComponentInChildren<Unit>() == null)
        {   destination = slotNumber - rnd; }
        else { DestroyCard(); return; }
        target.Move(side.GetChild(destination));
        // Dont think I need this line
        //target.transform.position = side.GetChild(destination).position;
        cardSlot.dontCollapse = true;
        if (hasAuthority)
        {
            PlayerManager.CmdSummon(cardSlot.GetLane().playerMeleeCreep,GetLineage(cardSlot.transform)); // not sure best way to generize this Cmd
        }

        DestroyCard();
    }

}
