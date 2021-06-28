using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : ModifierAbility
{
    //public override void PlacedOnTopOf(Unit unit)
    //{
    //    base.PlacedOnTopOf(unit);
    //    unit?.Bounce();
    //}
    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<AfterCombat_e>(AfterCombat));
        foreach( Unit unit in card.GetCardSlot().GetComponentsInChildren<Unit>())
        {
            if(unit != card)
            {
                unit.Bounce();
            }
        }
    }
    //Blocker
    void AfterCombat(AfterCombat_e e)
    {
        card.DestroyCard(false);
    }

}
