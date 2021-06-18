using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarbedMailItem : Item
{
    public override void OnPlay()
    {
        base.OnPlay();
        Unit target = transform.GetComponentInParent<Unit>();
        Unit[] AdjacentEnemies = target.GetAdjacentEnemies();
        for (int i = 0; i < 3; i++)
        {
            if (AdjacentEnemies[i] != null)
            {
                AdjacentEnemies[i].arrow = -1 * (i - 1);
            }
        }
    }
}
