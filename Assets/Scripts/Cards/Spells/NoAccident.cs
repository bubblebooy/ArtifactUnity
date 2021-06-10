using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoAccident : Spell
{
    //Deal 2 damage to a unit and give it Feeble permanently.
    public GameObject Ability;

    // copied from EnchantUnit. should this be an EnchantUnit script? should Enchantment cards be spells?
    public override void OnPlay()
    {
        base.OnPlay();
        Unit unit = transform.parent.GetComponentInChildren<Unit>();
        unit.Damage(2);
        GameObject ability = Instantiate(Ability, unit.GetComponent<AbilitiesManager>().abilities.transform);
        ability.GetComponent<Ability>().opponentEffect = hasAuthority != unit.hasAuthority;
        DestroyCard();
    }
}
