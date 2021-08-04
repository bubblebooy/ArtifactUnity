using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarbedMailItem : Item
{
    public override void OnPlay(CardPlayed_e cardPlayed_e)
    {
        base.OnPlay(cardPlayed_e);
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
