using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Accessories,
    Consumables,
};

public interface IItem
{
    int level { get; set; }
    int gold { get; set; }

    ItemType itemType { get; set; }
}
