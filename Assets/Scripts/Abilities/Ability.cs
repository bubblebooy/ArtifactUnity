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

    private Unit card;
    private bool expanded = false;

    public void Awake()
    {
        card = GetComponentInParent<Unit>();
    }

    public virtual void OnValidate()
    {
        transform.Find("abilityIcon").GetComponent<Image>().sprite = abilityIcon;
        GetComponentInChildren<TextMeshProUGUI>(true).text = abilityText;
    }

    public void expand()
    {
        expanded = true;
        GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(true);
        transform.Find("background").GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        transform.Find("background").GetComponent<Image>().raycastTarget = true;
    }

    public void collapse()
    {
        expanded = false;
        GetComponentInChildren<TextMeshProUGUI>(true).gameObject.SetActive(false);
        transform.Find("background").GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.9f);
        transform.Find("background").GetComponent<Image>().raycastTarget = true;
    }
}
