using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PhantomRush : Ability, IAura
{
    //Aura: If Phantom Lancer is blocked, Phantom Lancer and allied Lancer Illusions have +1 and +1.
    public int attack = 1;
    public int armor = 1;

    private List<AuraModifier> auraMods = new List<AuraModifier>();

    public override void OnPlay()
    {
        inPlayEvents.Add(GameEventSystem.Register<Auras_e>(ContinuousEffect));
    }

    public void ContinuousEffect(Auras_e e)
    {
        
        ComponentGameObjectComparer comparer = new ComponentGameObjectComparer();
        Unit[] auraUnits = new Unit[0];
        if(card.GetCombatTarget() != null)
        {
            auraUnits = card.transform.parent.parent.gameObject.GetComponentsInChildren<Unit>();
            auraUnits = auraUnits.Where(unit => unit.cardName == "Phantom Lancer" || unit.cardName == "Lancer Illusion").ToArray();
        }
        foreach (Unit unit in auraUnits.Except(auraMods, comparer))
        {
            AuraModifier modifier = unit.gameObject.AddComponent<AuraModifier>() as AuraModifier;
            modifier.maxArmor += armor;
            modifier.attack += attack;
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
}
