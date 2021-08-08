using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerModifier : MonoBehaviour
{
    protected TowerManager tower;

    public bool temporary = true;
    public int duration = 0;

    public int maxArmor;
    public int decay = 0;
    public int regeneration = 0;

    protected bool firstMod = true;

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
        tower.maxArmor += maxArmor;

        tower.decay += decay;
        tower.regeneration += regeneration;

        if (firstMod)
        {
            tower.armor += maxArmor;
            firstMod = false;
        }
    }

    private void OnDestroy()
    {
        tower.TowerUpdateEvent -= ModifyTower;
        GameEventSystem.Unregister(events);

        if (maxArmor > 0)
        {
            tower.maxArmor -= maxArmor;
            if (tower.armor > tower.maxArmor)
            {
                tower.armor = Mathf.Max(tower.maxArmor, tower.armor - maxArmor);
            }
        }
        else if (maxArmor < 0 && tower.armor != 0)
        {
            tower.armor -= maxArmor;
        }
    }

    public virtual void RoundStart(RoundStart_e e)
    {
        duration--;
        if (temporary && duration <= 0)
        {
            Destroy(this);
        }
    }

}
