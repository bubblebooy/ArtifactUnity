using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeleeCreep : Unit
{
    public Sprite direCreepArt;
    public Sprite radiantCreepArt;

    public override void OnStartClient()
    {
        base.OnStartClient();
        transform.Find("CardFront/Image").GetComponent<Image>().sprite = PlayerManager.radiantDire == hasAuthority ? direCreepArt : radiantCreepArt;
    }
}
