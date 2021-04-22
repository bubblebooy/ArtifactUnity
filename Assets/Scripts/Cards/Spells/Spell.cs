using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Spell : Card
{
    public string targetTag = "Lane";

    public override bool IsVaildPlay(GameObject target)
    {
        base.IsVaildPlay(target);
        if (target.tag == targetTag)
        {
            return true;
        }
        return false;
    }

    public override void CardUpdate()
    {

    }
}
