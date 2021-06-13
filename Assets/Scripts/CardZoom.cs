using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardZoom : MonoBehaviour
{

    public GameObject Canvas;

    private GameObject placeholder;
    private GameObject cardFront;
    private int siblingIndex;

    private bool hover = false;

    public void Awake()
    {
        Canvas = GameObject.Find("Main Canvas");
        cardFront = gameObject.transform.Find("CardFront").gameObject;
        placeholder = gameObject.transform.Find("Placeholder").gameObject;
    }

    public void OnHoverEnter()
    {
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
        if (hover & !GetComponent<DragDrop>().isDragging) 
        {
            cardFront.transform.SetParent(Canvas.transform, true);

            cardFront.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 1);
        }
    }

    public void OnHoverExit()
    {
        hover = false;
        if (placeholder != null)
        {
            cardFront.transform.SetParent(placeholder.transform.parent, true);
            cardFront.transform.SetSiblingIndex(siblingIndex);
            cardFront.GetComponent<RectTransform>().localPosition = placeholder.GetComponent<RectTransform>().localPosition;
            cardFront.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }       
    }
}
