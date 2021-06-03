using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantUnit : Enchantment
{
    [Header("Vaild Targets")]
    public bool enchantCaster = false;
    public bool enchantHero = false;
    public bool targetOnlyEnemySide = false;
    public bool targetOnlyPlayerSide = false;

    public override bool IsVaildPlay(GameObject target)
    {
        Unit targetUnit = target.GetComponent<Unit>();
        if (base.IsVaildPlay(target) &&
            targetUnit != null &&
            (!enchantCaster || (targetUnit.caster && !targetUnit.stun && !targetUnit.silenced)) &&
            (!enchantCaster || targetUnit.GetSide() == "PlayerSide") &&
            (!enchantHero || targetUnit is Hero) &&
            (!targetOnlyPlayerSide || targetUnit.GetSide() == "PlayerSide") &&
            (!targetOnlyEnemySide  || targetUnit.GetSide() == "EnemySide"))
        {
            return true;
        }
        return false;
    }

    public override void OnPlay()
    {
        base.OnPlay();
        Unit unit = transform.parent.GetComponentInChildren<Unit>();
        GameObject ability = Instantiate(Ability, unit.GetComponent<AbilitiesManager>().abilities.transform);
        ability.GetComponent<Ability>().opponentEffect = hasAuthority != unit.hasAuthority;
        DestroyCard();
    }
}
