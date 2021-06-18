using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierAbilitySecondary : ModifierAbility
{
    public override void OnValidate(){}
    protected override void Awake()
    {
        card = GetComponentInParent<Unit>();
    }
    public override void Expand(){}
    public override void Collapse(){}
}
