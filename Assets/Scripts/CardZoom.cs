using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardZoom : MonoBehaviour
{

    public GameObject Canvas;

    public GameObject placeholderPrefab;
    private GameObject placeholder;
    private int position;

    private bool hover = false;

    public void Awake()
    {
        Canvas = GameObject.Find("Main Canvas");
    }

    public void OnHoverEnter()
    {
        hover = true;
        StartCoroutine(DelayedHover(0.1f));

    }

    public IEnumerator DelayedHover(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (hover && placeholder == null)
        {
            position = transform.GetSiblingIndex();
            placeholder = Instantiate(placeholderPrefab, gameObject.transform.parent);
            placeholder.transform.SetSiblingIndex(position);

            gameObject.transform.SetParent(Canvas.transform, true);

            gameObject.transform.Find("Color").GetComponent<RectTransform>().localScale = new Vector3(2, 2, 1);
        }
    }

    public void OnHoverExit()
    {
        hover = false;
        if (placeholder != null)
        {
            gameObject.transform.Find("Color").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            gameObject.transform.SetParent(placeholder.transform.parent, true);
            gameObject.transform.SetSiblingIndex(position);
            Destroy(placeholder);
        }       
    }
}
