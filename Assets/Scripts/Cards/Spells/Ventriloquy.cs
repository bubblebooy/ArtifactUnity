using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ventriloquy : Spell
{

    public int damage = 1;

    public override void OnPlay(CardPlayed_e cardPlayed_e)
    {
        base.OnPlay(cardPlayed_e);
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
        //Could prob do this with one loop but card reads this way so using 2 just incase
        for (int i = 0; i < 3; i++)
        {
            if (AdjacentEnemies[i] != null)
            {
                AdjacentEnemies[i].Strike(AdjacentEnemies[i].GetCombatTarget(), damage);
            }
        }

        DestroyCard(); // this might take too long
    }
}
