using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CooldownAbility : Ability
{

    public int baseCooldown = 2;
    public int cooldown = 0;

    public TextMeshProUGUI displayCooldown;

    public override void OnValidate()
    {
        base.OnValidate();
        displayCooldown = transform.Find("abilityIcon").GetComponentInChildren<TextMeshProUGUI>(true);
        displayCooldown.text = cooldown.ToString();
    }

    protected override void Awake()
    {
        base.Awake();
        events.Add(GameEventSystem.Register<RoundStart_e>(IncrementCooldown));
        displayCooldown = transform.Find("abilityIcon").GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public override void Clone(Ability originalAbility)
    {
        base.Clone(originalAbility);

        baseCooldown = (originalAbility as CooldownAbility).baseCooldown;
        cooldown = (originalAbility as CooldownAbility).cooldown;

        displayCooldown = transform.Find("abilityIcon").GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public override void CardUpdate()
    {
        base.CardUpdate();
        displayCooldown.text = cooldown.ToString();
        displayCooldown.enabled = cooldown > 0;
    }

    public void IncrementCooldown(RoundStart_e e)
    {
        if (cooldown > 0)
        {
            cooldown -= 1;
        }
    }
}
