using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerModifier : MonoBehaviour
{
    protected TowerManager tower;

    public bool temporary = true;
    public int duration = 0;

    public int decay = 0;
    public int regeneration = 0;

    public List<(System.Type, GameEventSystem.EventListener)> events = new List<(System.Type, GameEventSystem.EventListener)>();

    protected virtual void Awake()
    {
        tower = GetComponentInParent<TowerManager>();
        tower.TowerUpdateEvent += ModifyTower;
        if (temporary)
        {
            events.Add(GameEventSystem.Register<RoundStart_e>(RoundStart));
        }
    }

    protected virtual void ModifyTower()
    {
        tower.decay += decay;
        tower.regeneration += regeneration;
    }

    private void OnDestroy()
    {
        tower.TowerUpdateEvent -= ModifyTower;
        GameEventSystem.Unregister(events);
    }

    public virtual void RoundStart(RoundStart_e e)
    {
        duration--;
        if (duration <= 0)
        {
            Destroy(this);
        }
    }

}
