using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Juxtapose : Ability
{
    //After Combat: Summon a Lancer Illusion into the closest slot.\
    public GameObject unitPlaceholder;

    public override void OnPlay()
    {
        inPlayEvents.Add(GameEventSystem.Register<EndCombatPhase_e>(EndCombatPhase));
    }
    public void EndCombatPhase(EndCombatPhase_e e)
    {
        card.GetLane().GetComponent<LaneVariableSlots>()?.UpdateSlots();
        CardSlot cardSlot = card.GetCardSlot();
        Transform side = card.transform.parent.parent;
        if (side == null) { return; }
        int slotNumber = cardSlot.transform.GetSiblingIndex();
        int numberOfSlots = side.GetComponentsInChildren<CardSlot>().Length;

        CardSlot[] slots = side.GetComponentsInChildren<CardSlot>();
        slots = slots.Where(x => x.GetComponentInChildren<Unit>() == null).ToArray();
        slots = slots.OrderBy(x => Mathf.Abs(x.transform.GetSiblingIndex() - (slotNumber - 0.1f))).ToArray();

        if (slots.Length > 0)
        {
            UnitPlaceholder placeholder = Instantiate(unitPlaceholder, slots[0].transform).GetComponent<UnitPlaceholder>();
            if (card.hasAuthority)
            {
                placeholder.cardName = "Lancer Illusion";
            }
        }
    }
}
