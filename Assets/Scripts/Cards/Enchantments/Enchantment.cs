using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Enchantment : Card
{

    public override bool IsVaildPlay(GameObject target)
    {
        if (target.GetComponent<Unit>() != null) // && GetComponentInParent<Board>
        {
            return true;
        }
        return false;
    }

    public override void CardUpdate()
    {

    }
}
