using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sven : Hero
{
    public override void Combat(bool quick = false)
    {
        if (quick == quickstrike && !disarmed)
        {
            int slotNumber = transform.parent.GetSiblingIndex();
            int numberOfSlots = transform.parent.parent.GetComponentsInChildren<CardSlot>().Length;
            bool player = transform.parent.parent.name == "PlayerSide";
            Transform targetSlot = transform.parent.parent.parent.Find(player ? "EnemySide" : "PlayerSide").GetChild(slotNumber + arrow);
            Unit target = targetSlot.GetComponentInChildren<Unit>();

            if (target != null)
            {
                Strike(target, attack + cleave);
            }
            else
            {
                TowerManager tower = transform.parent.parent.parent.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
                tower.Damage(attack);
            }
            for (int i = -1; i <= 1; i++)
            {
                if (i == arrow) { continue; }
                if (slotNumber + i < 0 || slotNumber + i >= numberOfSlots) { continue; }
                targetSlot = transform.parent.parent.parent.Find(player ? "EnemySide" : "PlayerSide").GetChild(slotNumber + i);
                target = targetSlot.GetComponentInChildren<Unit>();
                if (target != null)
                {
                    Strike(target, attack + cleave);
                }
            }


        }
    }
}
