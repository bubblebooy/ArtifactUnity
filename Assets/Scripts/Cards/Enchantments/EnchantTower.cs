using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantTower : Enchantment
{

    public override bool IsVaildPlay(GameObject target)
    {
        if (base.IsVaildPlay(target) &&
            target.GetComponent<LaneManager>() != null)
        {
            return true;
        }
        return false;
    }

    public override void OnPlay()
    {
        base.OnPlay();
        Transform enchantments = transform.parent.Find(hasAuthority ? "PlayerSide/Enchantments" : "EnemySide/Enchantments");
        Instantiate(Ability, enchantments);
        DestroyCard();
    }
}
