using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

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

    public List<(System.Type, GameEventSystem.EventListener)> inPlayEvents = new List<(System.Type, GameEventSystem.EventListener)>();

    public virtual void OnValidate()
    {
        transform.Find("abilityIcon/icon").GetComponent<Image>().sprite = abilityIcon;
        transform.Find("abilityText/text").GetComponent<TextMeshProUGUI>().text = abilityText;
    }

    protected virtual void Awake()
    {
        EventTrigger m_EventTrigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventData) => { Expand(); });
        m_EventTrigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((eventData) => { Collapse(); });
        m_EventTrigger.triggers.Add(entry);

        displayAbilityText = transform.Find("abilityText").gameObject;
        Canvas = GameObject.Find("Main Canvas");
    }

    public virtual void Combat() { }

    public void Expand()
    {
        expanded = true;
        displayAbilityText.GetComponent<RectTransform>().sizeDelta = new Vector2(300,0);
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

    private void OnDestroy()
    {
        GameEventSystem.Unregister(inPlayEvents);
    }

#pragma warning restore 0414
}
