using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vendetta : ActiveTargetAbility
{
    // Active 3, 1 Mana: Choose an enemy in any lane and move Nyx Assassin across from it.
    // Nyx Assassin strikes the new unit blocking it with +3. Cross Lane
    public override bool IsVaildTarget(GameObject target)
    {
        if (card.rooted) { return false; }
        Unit unit = target.GetComponent<Unit>();
        return base.IsVaildTarget(target) &&
            unit.GetAdjacentEnemies(0)[0] == null;
    }

    public override void OnActivate(List<GameObject> targets)
    {
        base.OnActivate(targets);
        card.Move(targets[0].GetComponent<Unit>().GetAcrossCardSlot().transform);
        card.Strike(targets[0].GetComponent<Unit>(), card.attack + 3);

    }
}
