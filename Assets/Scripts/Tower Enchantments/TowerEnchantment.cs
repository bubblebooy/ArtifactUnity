using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#pragma warning disable 0414 
public class TowerEnchantment : MonoBehaviour
{
    public Sprite abilityIcon;
    public string abilityName;
    [TextArea]
    public string abilityText;

    protected GameObject displayAbilityText;
    private GameObject Canvas;


    public bool broken = false;
    private bool expanded = false;
    

    public virtual void OnValidate()
    {
        transform.Find("abilityIcon/icon").GetComponent<Image>().sprite = abilityIcon;
        transform.Find("abilityText/text").GetComponent<TextMeshProUGUI>().text = abilityText;
    }

    protected virtual void Awake()
    {
        displayAbilityText = transform.Find("abilityText").gameObject;
        Canvas = GameObject.Find("Main Canvas");
    }

    public virtual void TowerUpdate() { } // for dealing with retaliate and armor
    public virtual void RoundStart() { }
    public virtual void Combat() { }

    public void Expand()
    {
        expanded = true;
        displayAbilityText.GetComponent<RectTransform>().sizeDelta = new Vector2(200,0);
        displayAbilityText.transform.SetParent(Canvas.transform,true);
    }

    public void Collapse()
    {
        expanded = false;
        displayAbilityText.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        displayAbilityText.transform.SetParent(transform, true);
    }

    public string GetSide()
    {
        return transform.parent.parent.name;
    }

    public LaneManager GetLane()
    {
        return transform.parent.parent.parent.GetComponent<LaneManager>();
    }

#pragma warning restore 0414
}
