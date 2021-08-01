using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitModifier : StatModifier, IModifier
{
    // Moved all to StatModifier so I can share code with an AuraModifier
    public bool opponentEffect = false;
    public bool temporary = true;
    public int duration = 0;

    protected override void Awake()
    {
        base.Awake();
        events.Add(GameEventSystem.Register<RoundStart_e>(RoundStart));
    }

    public override void Clone(IModifier originalIModifier)
    {
        opponentEffect = (originalIModifier as UnitModifier).opponentEffect;
        temporary = (originalIModifier as UnitModifier).temporary;
        duration = (originalIModifier as UnitModifier).duration;

        base.Clone(originalIModifier);
    }

    public virtual void RoundStart(RoundStart_e e)
    {
        duration--;
        if (temporary && duration <= 0)
        {
            Destroy(this);
        }
    }

    public void SetDestoryOnNextTurn()
    {
        events.Add(GameEventSystem.Register<NextTurn_e>(DestoryOnNextTurn));
    }
    public virtual void DestoryOnNextTurn(NextTurn_e e)
    {
            Destroy(this);
    }

    //NextTurn_e
}
