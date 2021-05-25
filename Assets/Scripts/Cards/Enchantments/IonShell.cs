using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IonShell : EnchantUnit
{
    public override void OnPlay()
    {
        Unit target = transform.parent.GetComponent<Unit>();
        Unit[] AdjacentEnemies = target.GetAdjacentEnemies();
        for (int i = 0; i < 3; i++)
        {
            print(i);
            if (AdjacentEnemies[i] != null)
            {
                AdjacentEnemies[i].arrow = -1 * (i - 1);
            }
        }
        base.OnPlay();
    }
}
