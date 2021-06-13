using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#pragma warning disable 0414 
public class Ability : MonoBehaviour
{
    public Sprite abilityIcon;
    public string abilityName;
    [TextArea]
    public string abilityText;

    protected Unit card;
    public bool broken = false;

    public bool opponentEffect = false;
    public bool temporary = false;
    public bool baseAbility = true;
    public bool itemAbility = false;

    private bool expanded = false;
    private Color backgroundColor;

    public List<(System.Type, GameEventSystem.EventListener)> inPlayEvents = new List<(System.Type, GameEventSystem.EventListener)>();
    public List<(System.Type, GameEventSystem.EventListener)> events = new List<(System.Type, GameEventSystem.EventListener)>();


    public virtual void OnValidate()
    {
        transform.Find("abilityIcon").GetComponent<Image>().sprite = abilityIcon;
        transform.Find("abilityText").GetComponent<TextMeshProUGUI>().text = abilityText;
    }

    protected virtual void Awake()
    {
        card = GetComponentInParent<Unit>();
        backgroundColor = transform.Find("background").GetComponent<Image>().color;
    }

    public virtual void Clone(Ability originalAbility)
    {
        card = GetComponentInParent<Unit>();
        inPlayEvents = new List<(System.Type, GameEventSystem.EventListener)>();
        events = new List<(System.Type, GameEventSystem.EventListener)>();
        broken = originalAbility.broken;
        opponentEffect = originalAbility.opponentEffect;
        temporary = originalAbility.temporary;
        expanded = false;
    }

    public virtual void CardUpdate() { }
    //public virtual void RoundStart() { }
    public virtual void OnPlay() { }
    public virtual void OnKilled() { }
    public virtual void PlacedOnTopOf(Unit unit) { }

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
        transform.Find("background").GetComponent<Image>().color = backgroundColor;
        transform.Find("background").GetComponent<Image>().raycastTarget = true;
    }

    protected virtual void OnDestroy()
    {
        GameEventSystem.Unregister(inPlayEvents);
        GameEventSystem.Unregister(events);
    }

    public virtual void Bounce()
    {
        GameEventSystem.Unregister(inPlayEvents);
    }

    public virtual void DestroyCard()
    {
        OnDestroy();
    }

#pragma warning restore 0414
}
