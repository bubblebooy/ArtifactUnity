using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eclipse : Spell
{
    //Repeat one time for each charge: Deal 2 piercing damage to a random enemy.
    //Charges: 1

    public int charges = 1;

    public override void OnPlay(CardPlayed_e cardPlayed_e)
    {
        base.OnPlay(cardPlayed_e);
        
        for (int i = 0; i < charges; i++)
        {
            Transform side = transform.parent
                .Find(hasAuthority ? "EnemySide" : "PlayerSide");
            Unit[] enemies = side.GetComponentsInChildren<Unit>();
            if (enemies.Length > 0)
            {
                int rnd = Random.Range(0, enemies.Length);
                enemies[rnd].Damage(cardPlayed_e.caster.GetComponent<Unit>(), 2, piercing: true);
                GameManager.GameUpdate();
            }
            else { break; }
        }
        DestroyCard();
    }

    public override void CardUIUpdate(GameUpdateUI_e e)
    {
        base.CardUIUpdate(e);
        displayCardText.text = cardText.Remove(cardText.Length - 1, 1) + charges;
    }
}
