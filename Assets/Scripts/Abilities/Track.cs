using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : Ability
{
    // Bounty Hunter deals +3 damage when striking tracked heroes.
    // Deploy: Track Bounty Hunter's target if it's a hero. Tracked units have +5 Bounty.

    public override void OnPlay()
    {
        base.OnPlay();
        card.StrikeUnitEvent += StrikeUnit;

        Unit unit = card.GetCombatTarget();
        if( unit is Hero && unit.gameObject.GetComponent<TrackModifier>() == null)
        {
            UnitModifier track = unit.gameObject.AddComponent<TrackModifier>() as TrackModifier;
            track.bounty = 5;
            track.duration = 999;
            track.opponentEffect = true;
        }

    }

    void StrikeUnit(Unit target, ref int damage, ref bool piercing)
    {
        if(target.gameObject.GetComponent<TrackModifier>() != null)
        {
            damage += 3;
        }
    }
}
