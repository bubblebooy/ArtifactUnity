using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Vacuum : ActiveTargetAbility
{
    // Start is called before the first frame update
    public override void OnActivate(List<GameObject> targets)
    {
        base.OnActivate(targets);
        
        CardSlot cardSlot = targets[0].GetComponent<CardSlot>();
        Transform side = cardSlot.transform.parent;

        foreach (Unit unit in side.GetComponentsInChildren<Unit>())
        {
            unit.Damage(1);
        }

        card.GameManager.GameUpdate();

        int slotNumber = cardSlot.transform.GetSiblingIndex();
        Unit[] units = side.GetComponentsInChildren<Unit>();
        units = units.Where(x => x.transform.parent.GetSiblingIndex() != slotNumber).ToArray();
        units = units.OrderBy(x => Mathf.Abs(x.transform.parent.GetSiblingIndex() - (slotNumber - 0.1f))).ToArray();
        foreach (Unit unit in units)
        {
            if (unit.rooted) { continue;  }
            int slot = unit.transform.parent.GetSiblingIndex();
            slot += slot > slotNumber ? -1 : 1;
            while(side.GetChild(slot).GetComponentInChildren<Unit>() == null)
            {
                unit.transform.SetParent(side.GetChild(slot), false);
                unit.transform.position = side.GetChild(slot).position;
                if(slot != slotNumber)
                {
                    slot += slot > slotNumber ? -1 : 1;
                }
            }
        }
    }
}

//            if (side.GetChild(slot).GetComponentInChildren<Unit>() == null)