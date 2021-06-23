using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraModifier : StatModifier
{
    protected override void Awake()
    {
        base.Awake();
        events.Add(GameEventSystem.Register<AuraModifiers_e>(AuraModifiers));
    }

    void AuraModifiers(AuraModifiers_e e)
    {
        ModifyCard();
    }
}

public class ComponentGameObjectComparer : IEqualityComparer<Component>
{
    public bool Equals(Component x, Component y)
    {
        return x.gameObject == y.gameObject;
    }

    public int GetHashCode(Component obj)
    {
        return obj.gameObject.GetHashCode();
    }
}
