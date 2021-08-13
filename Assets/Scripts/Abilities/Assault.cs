using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assault : CooldownAbility, IModifier
{
    // Death Effect: Give Rix, Oathbound Rapid Deployment. 2 round cooldown.
    public void ModifyCard()
    {
        if(cooldown <= 0)
        {
            (card as Hero).rapidDeploy = true;
        }
    }

    public void  Clone(IModifier originalIModifier)
    {
        print("I DONT THINK THIS SHOULD EVER BE CALLED");
        // dont think I need this on  Ability, IModifier. Ability should have its own clone
    }

    public override void OnKilled()
    {
        if(cooldown <= 0)
        {
            cooldown = baseCooldown;
        }
    }
}
