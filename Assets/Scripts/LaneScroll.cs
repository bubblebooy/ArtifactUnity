using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LaneScroll : MonoBehaviour
{
    public RectTransform[] content;
    public int scrollSensitivity = 10;

    float viewportWidth;
    float contentWidth;

    void Start()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Scroll;
        entry.callback.AddListener((data) => { OnScroll((PointerEventData)data); });
        trigger.triggers.Add(entry);
        viewportWidth = (transform as RectTransform).sizeDelta.x;
    }

    public void Initialize()
    {
        contentWidth = content[0].sizeDelta.x;
        if (viewportWidth < contentWidth)
        {
            Destroy(GetComponent<EventTrigger>());
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        //print(content[0].anchoredPosition.x * eventData.scrollDelta.y );
        if ((content[0].anchoredPosition.x * eventData.scrollDelta.y) < content[0].sizeDelta.x / 2)
        {
            foreach (RectTransform rectTransform in content)
            {
                rectTransform.anchoredPosition += new Vector2(scrollSensitivity * eventData.scrollDelta.y, 0);
            }
        }

    }

}
