using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantUnit : Enchantment
{
    [Header("Vaild Targets")]
    public bool enchantCaster = false;
    public bool enchantHero = false;

    public override bool IsVaildPlay(GameObject target)
    {
        Unit unit = target.GetComponent<Unit>();
        if (base.IsVaildPlay(target) &&
            unit != null &&
            (!enchantCaster || unit.caster) &&
            (!enchantCaster || unit.GetSide() == "PlayerSide") &&
            (!enchantHero || unit is Hero))
        {
            return true;
        }
        return false;
    }

    public override void OnPlay()
    {
        base.OnPlay();
        Unit unit = gameObject.transform.parent.GetComponentInChildren<Unit>();
        Instantiate(Ability, unit.GetComponent<AbilitiesManager>().abilities.transform);
        DestroyCard();
    }
}
