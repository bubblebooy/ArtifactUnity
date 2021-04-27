using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseModifier : MonoBehaviour
{

    public int mana = 0;
    private Card card;

    private void Start()
    {
        print("modifier added");
        card = GetComponentInParent<Card>();
    }

    public virtual void ModifyCard()
    {
        card.mana += mana;
    }

}
