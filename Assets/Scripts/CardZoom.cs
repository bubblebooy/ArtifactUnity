using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardZoom : MonoBehaviour
{

    public GameObject Canvas;

    private GameObject placeholder;
    private GameObject color;

    private bool hover = false;

    //public void OnValidate()
    //{
    //    color = gameObject.transform.Find("Color").gameObject;
    //    foreach (Image image in color.GetComponentsInChildren<Image>())
    //    {
    //        image.raycastTarget = false;
    //    }
    //    placeholder = gameObject.transform.Find("Placeholder").gameObject;
    //    placeholder.GetComponent<Image>().raycastTarget = true;
    //}

    public void Awake()
    {
        Canvas = GameObject.Find("Main Canvas");
        color = gameObject.transform.Find("Color").gameObject;
        placeholder = gameObject.transform.Find("Placeholder").gameObject;
    }

    public void OnHoverEnter()
    {
        hover = true;
        StartCoroutine(DelayedHover(0.1f));

    }

    public IEnumerator DelayedHover(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (hover & !GetComponent<DragDrop>().isDragging) // && placeholder == null
        {
            color.transform.SetParent(Canvas.transform, true);

            color.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 1);


        }
    }

    public void OnHoverExit()
    {
        hover = false;
        if (placeholder != null)
        {
            color.transform.SetParent(placeholder.transform.parent, true);
            color.transform.SetSiblingIndex(1);
            color.GetComponent<RectTransform>().localPosition = placeholder.GetComponent<RectTransform>().localPosition;
            color.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }       
    }
}
