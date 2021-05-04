using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Enchantment : Card
{
    public GameObject Ability;

    protected override IEnumerator Discard()
    {
        yield return new WaitForSeconds(1.0f);
        yield return base.Discard();
    }
}

