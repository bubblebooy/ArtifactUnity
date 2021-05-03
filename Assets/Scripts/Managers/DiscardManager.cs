using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardManager : MonoBehaviour
{
    public int expanedSize = 600;

    private bool expanded = false;
    RectTransform RectTransform;

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }

    public void OnHover()
    {
        if (!expanded)
        {
            Vector2 position = RectTransform.anchoredPosition;
            position.x = 150;
            RectTransform.anchoredPosition = position;
        }

    }
    public void StopHover()
    {
        if (!expanded)
        {
            Vector2 position = RectTransform.anchoredPosition;
            position.x = 20;
            RectTransform.anchoredPosition = position;
        }

    }
    public void OnClick()
    {
        if (expanded)
        {
            expanded = false;
            Vector2 position = RectTransform.anchoredPosition;
            position.x = 20;
            RectTransform.anchoredPosition = position;

            Vector2 sizeDelta = RectTransform.sizeDelta;
            sizeDelta.x = 150;
            RectTransform.sizeDelta = sizeDelta;
        }
        else
        {
            expanded = true;
            Vector2 position = RectTransform.anchoredPosition;
            position.x = expanedSize;
            RectTransform.anchoredPosition = position;

            Vector2 sizeDelta = RectTransform.sizeDelta;
            sizeDelta.x = position.x;
            RectTransform.sizeDelta = sizeDelta;
        }
    }
}
