using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlagueWard : ModifierAbility
{
    //Feeble.
    //Round Start: Strike a random adjacent enemy for 2.
    //If there are none, strike the enemy tower for 2  instead.

    public override void OnPlay()
    {
        inPlayEvents.Add(GameEventSystem.Register<PlayStart_e>(PlayStart));
    }

    void PlayStart(PlayStart_e e)
    {
        Unit[] adjacentEnemies = card.GetAdjacentEnemies()
            .Where(x => x != null).ToArray();
        if (adjacentEnemies.Length > 0)
        {
            int rnd = Random.Range(0, adjacentEnemies.Length);
            card.Strike(adjacentEnemies[rnd], 2);
        }
        else
        {
            bool player = card.GetSide() == "PlayerSide";
            TowerManager tower = card.GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
            card.Strike(tower, 2);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        //PhantomAssassin = (PhantomAssassin)card;
        card.StrikeUnitEvent += StrikeUnit;
        card.StrikeTowerEvent += StrikeTower;
    }

    public void StrikeUnit(Unit target, ref int damage, ref bool piercing)
    {
        if (card.GetLane().transform.Find(card.GetSide()).GetComponentInChildren<PoisonSting>() == null)
        {
            return;
        }           

        UnitModifier poisonSting = target.gameObject.AddComponent<UnitModifier>() as UnitModifier;
        poisonSting.decay = 1;
        if (card.GameManager.GameState == "Combat" || card.GameManager.GameState == "Shop")
        {
            poisonSting.duration = 2;
        }
    }

    public void StrikeTower(TowerManager target, ref int damage, ref bool piercing)
    {
        if (card.GetLane().transform.Find(card.GetSide()).GetComponentInChildren<PoisonSting>() == null)
        {
            return;
        }
        TowerModifier poisonSting = target.gameObject.AddComponent<TowerModifier>() as TowerModifier;
        poisonSting.decay = 1;
        if (card.GameManager.GameState == "Combat" || card.GameManager.GameState == "Shop")
        {
            poisonSting.duration = 2;
        }
    }
}
