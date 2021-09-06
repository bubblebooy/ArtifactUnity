using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoAccident : Spell
{
    //Deal 2 damage to a unit and give it Feeble permanently.
    public GameObject Ability;

    // copied from EnchantUnit. should this be an EnchantUnit script? should Enchantment cards be spells?
    public override void OnPlay(CardPlayed_e cardPlayed_e)
    {
        base.OnPlay(cardPlayed_e);
        Unit unit = transform.parent.GetComponentInChildren<Unit>();
        unit.Damage(cardPlayed_e.caster.GetComponent<Unit>(), 2);
        GameObject ability = Instantiate(Ability, unit.GetComponent<AbilitiesManager>().abilities.transform);
        ability.GetComponent<Ability>().opponentEffect = true; // hasAuthority != unit.hasAuthority;
        DestroyCard();
    }
}
