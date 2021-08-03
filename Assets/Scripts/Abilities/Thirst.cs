using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Thirst : ModifierAbility
{
    // Bloodseeker has + equal to the highest missing from an enemy hero in any lane
    // Does not decrease if the hero dies. Cross Lane

    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<RoundStart_e>(RoundStart));
    }

    public override void CardUpdate()
    {
        base.CardUpdate();
        if (!card.inPlay) { return; }
        Hero[] heros = card.GetLane().transform.parent.GetComponentsInChildren<Hero>();
        heros = heros.Where(hero => card.hasAuthority != hero.hasAuthority).ToArray();
        foreach (Hero hero in heros)
        {
            if (hero.health > 0)
            {
                attack = Mathf.Max(attack, hero.maxHealth - hero.health);
            }
        }
    }

    public virtual void RoundStart(RoundStart_e e)
    {
        attack = 0;
    }
}
