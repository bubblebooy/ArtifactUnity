using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrecisionAura : Ability, IAura
{
    int damage = 1;
    string side;

    public override void OnPlay()
    {
        inPlayEvents.Add(GameEventSystem.Register<Auras_e>(ContinuousEffect));
        side = card.GetSide();
    }

    public void ContinuousEffect(Auras_e e)
    {
        foreach (Unit unit in GameObject.Find("Board").GetComponentsInChildren<Unit>())
        {
            if (unit != card && unit.GetSide() == side)
            {
                unit.attack += damage;
            }
        }
        //print(card.transform.parent.name);
    }

    public override void Bounce()
    {
        if(card.GameManager.GameState == "Combat")
        {
            inPlayEvents.Add(GameEventSystem.Register<AfterCombat_e>(AfterCombat));
        } else
        {
            base.Bounce();
        }
    }

   void AfterCombat(AfterCombat_e e)
    {
        base.Bounce();
    }
}
