using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PrecisionAura : Ability, IAura
{
    int damage = 1;
    string side;

    private GameObject board;

    private List<AuraModifier> auraMods = new List<AuraModifier>();

    public override void OnPlay()
    {
        inPlayEvents.Add(GameEventSystem.Register<Auras_e>(ContinuousEffect));
        board = GameObject.Find("Board");
        side = card.GetSide();
    }

    public void ContinuousEffect(Auras_e e)
    {
        ComponentGameObjectComparer comparer = new ComponentGameObjectComparer();
        Unit[] auraUnits = board.GetComponentsInChildren<Unit>();
        auraUnits = auraUnits.Where(unit => unit != card && unit.GetSide() == side).ToArray();
        foreach (Unit unit in auraUnits.Except(auraMods, comparer))
        {
            AuraModifier modifier = unit.gameObject.AddComponent<AuraModifier>() as AuraModifier;
            modifier.attack += damage;
            auraMods.Add(modifier);
            GameManager.updateloop = true;
        }
        foreach (AuraModifier modifier in auraMods.ToArray().Except(auraUnits, comparer))
        {
            Destroy(modifier);
            auraMods.Remove(modifier);
            GameManager.updateloop = true;
        }

    }

    public override void Bounce()
    {
        base.Bounce();
        if (card.GameManager.GameState == "Combat")
        {
            inPlayEvents.Add(GameEventSystem.Register<AfterCombat_e>(AfterCombat));
        } else
        {
            LeavePlay();
        }
        
    }

   void AfterCombat(AfterCombat_e e)
    {
        LeavePlay();
        GameEventSystem.Unregister(inPlayEvents);
    }

    private void LeavePlay()
    {
        foreach (AuraModifier modifier in auraMods.ToList())
        {
            Destroy(modifier);
            auraMods.Remove(modifier);
            GameManager.updateloop = true;
        }
    }

    protected override void OnDestroy()
    {
        LeavePlay();
        base.OnDestroy();
    }
}
