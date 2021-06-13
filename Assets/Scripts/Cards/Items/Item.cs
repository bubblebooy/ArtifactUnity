using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Item : Card
{
    public int level;
    public int gold;

    private TextMeshProUGUI displayGold;

    public override void OnValidate()
    {
        base.OnValidate();
        displayGold = gameObject.transform.Find("CardFront/GoldIcon/Gold").GetComponent<TextMeshProUGUI>();
        displayGold.text = gold.ToString();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        displayGold = gameObject.transform.Find("CardFront/GoldIcon/Gold").GetComponent<TextMeshProUGUI>();
        displayGold.text = gold.ToString();
    }
}
