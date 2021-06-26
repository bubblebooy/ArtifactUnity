using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LaneScroll : MonoBehaviour
{
    public BunchedEndLayoutGroup[] content;
    public int scrollSensitivity = 10;


    void Start()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Scroll;
        entry.callback.AddListener((data) => { OnScroll((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    public void Initialize()
    {
        //if (false)
        //{
        //    Destroy(GetComponent<EventTrigger>());
        //}
    }

    public void OnScroll(PointerEventData eventData)
    {
        content[0].scrollOffest += scrollSensitivity * eventData.scrollDelta.y;
        content[1].scrollOffest = content[0].scrollOffest;
        LayoutRebuilder.MarkLayoutForRebuild(content[0].transform as RectTransform);
        LayoutRebuilder.MarkLayoutForRebuild(content[1].transform as RectTransform);
    }

}
