using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticRemnant : Ability
{
    // Whenever Storm Spirit moves from a slot, summon a Static Remnant in that slot.
    Transform previousSlot = null;

    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<GameUpdate_e>(GameUpdate));
    }

    protected override void Awake()
    {
        base.Awake();
        card.MoveEvent += MoveEvent;
    }

    public void MoveEvent(Transform origin, Transform destination)
    {
        previousSlot = origin;
    }

    public void GameUpdate(GameUpdate_e e)
    {
        if(previousSlot != null)
        {
            if (card.hasAuthority && previousSlot.childCount == 0)
            {
                card.PlayerManager.CmdSummon("Static Remnant", Card.GetLineage(previousSlot));
            }
            previousSlot = null;
        }
    }

}
