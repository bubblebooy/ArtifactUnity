using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrapper : ModifierAbility
{
    //After Combat: If Debbi the Cunning dealt damage to a tower, give the tower -1 and give Debbi +1 permanently.
    TowerManager struckTower = null;

    protected override void Awake()
    {
        base.Awake();
        card.StrikeTowerEvent += StrikeTower;
    }

    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<AfterCombat_e>(AfterCombat));
    }
    public override void Bounce()
    {
        base.Bounce();
        struckTower = null;
    }
    public void AfterCombat(AfterCombat_e e)
    {
        if(struckTower != null)
        {
            maxArmor += 1;
            TowerModifier scrapper = struckTower.gameObject.GetComponent<TowerModifier>() ?? struckTower.gameObject.AddComponent<TowerModifier>() as TowerModifier;
            scrapper.maxArmor -= 1;
            scrapper.temporary = false;
        }
        struckTower = null;
    }

    public void StrikeTower(TowerManager target, ref int damage, ref bool piercing)
    {
        struckTower = target;
    }
}
