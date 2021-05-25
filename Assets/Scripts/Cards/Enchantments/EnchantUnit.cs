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
        Unit unit = target.GetComponent<Unit>();
        if (base.IsVaildPlay(target) &&
            unit != null &&
            (!enchantCaster || unit.caster) &&
            (!enchantCaster || unit.GetSide() == "PlayerSide") &&
            (!enchantHero || unit is Hero) &&
            (!targetOnlyPlayerSide || unit.GetSide() == "PlayerSide") &&
            (!targetOnlyEnemySide  || unit.GetSide() == "EnemySide"))
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
