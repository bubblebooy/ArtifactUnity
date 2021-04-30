using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Ability : MonoBehaviour
{
    public Sprite abilityIcon;
    public string abilityName;
    [TextArea]
    public string abilityText;

    protected Unit card;
    public bool broken = false;
    private bool expanded = false;

    protected virtual void Awake()
    {
        card = GetComponentInParent<Unit>();
    }

    public virtual void CardUpdate() { }
    public virtual void RoundStart() { }


    public virtual void OnValidate()
    {
        transform.Find("abilityIcon").GetComponent<Image>().sprite = abilityIcon;
        transform.Find("abilityText").GetComponent<TextMeshProUGUI>().text = abilityText;
    }

    public void Expand()
    {
        expanded = true;
        //GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(true);
        transform.Find("background").GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        transform.Find("background").GetComponent<Image>().raycastTarget = true;
    }

    public void Collapse()
    {
        expanded = false;
        //GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(false);
        transform.Find("background").GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.9f);
        transform.Find("background").GetComponent<Image>().raycastTarget = true;
    }
}
