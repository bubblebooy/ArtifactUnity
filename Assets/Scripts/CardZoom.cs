using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardZoom : MonoBehaviour
{

    public GameObject Canvas;

    private GameObject placeholder;
    private GameObject cardFront;
    private int siblingIndex;
    Card card;

    private bool hover = false;

    public void Awake()
    {
        Canvas = GameObject.Find("Main Canvas");
        cardFront = gameObject.transform.Find("CardFront").gameObject;
        placeholder = gameObject.transform.Find("Placeholder").gameObject;
        card = GetComponent<Card>();
    }

    void Start()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Scroll;
        entry.callback.AddListener((data) => { OnScroll((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    public void OnHoverEnter()
    {
        if (!isActiveAndEnabled) { return; }
        if (cardFront.activeInHierarchy) 
        {
            hover = true;
            siblingIndex = cardFront.transform.GetSiblingIndex();
            StartCoroutine(DelayedHover(0.1f));
        }

    }

    public IEnumerator DelayedHover(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        if (isActiveAndEnabled && hover && !GetComponent<DragDrop>().isDragging) 
        {
            card.CardUIUpdate(new GameUpdateUI_e());
            cardFront.transform.SetParent(Canvas.transform, true);

            (cardFront.transform as RectTransform).localScale = new Vector3(2, 2, 1);
            float y = (cardFront.transform as RectTransform).anchoredPosition.y;
            float x = (cardFront.transform as RectTransform).anchoredPosition.x;
            y = Mathf.Clamp(y, -240, 240);
            x = Mathf.Clamp(x, -560, 560);
            (cardFront.transform as RectTransform).anchoredPosition = new Vector2(x, y);
        }
    }

    public void OnHoverExit()
    {
        if (!isActiveAndEnabled) { return; }
        hover = false;
        if (placeholder != null)
        {
            cardFront.transform.SetParent(placeholder.transform.parent, true);
            cardFront.transform.SetSiblingIndex(siblingIndex);
            cardFront.GetComponent<RectTransform>().localPosition = placeholder.GetComponent<RectTransform>().localPosition;
            cardFront.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            card.CardUIUpdate(new GameUpdateUI_e());
        }       
    }

    public void OnScroll(PointerEventData eventData)
    {
        GetComponentInParent<LaneScroll>()?.OnScroll(eventData);
    }
}
