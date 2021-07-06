using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChargesAbility : Ability
{
    public int charges = 0;

    public TextMeshProUGUI displayCharges;

    public override void OnValidate()
    {
        base.OnValidate();
        displayCharges = transform.Find("abilityIcon").GetComponentInChildren<TextMeshProUGUI>(true);
        displayCharges.text = charges.ToString();
    }

    protected override void Awake()
    {
        base.Awake();
        displayCharges = transform.Find("abilityIcon").GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public override void Clone(Ability originalAbility)
    {
        base.Clone(originalAbility);
        charges = (originalAbility as CooldownAbility).cooldown;

        displayCharges = transform.Find("abilityIcon").GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public override void CardUpdate()
    {
        base.CardUpdate();
        displayCharges.text = charges.ToString();
    }
}
