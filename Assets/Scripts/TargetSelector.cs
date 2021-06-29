using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetSelector : MonoBehaviour
{
    private GameObject targetingLine;
    private Canvas Canvas;

    //// Update is called once per frame

    private void Start()
    {
        targetingLine = GameObject.Find("TargetingArrow");
        Canvas = GameObject.Find("Main Canvas").GetComponent<Canvas>();
    }
    void Update()
    {
        Vector3 start = transform.position;
        Vector3 end = Input.mousePosition;
        (targetingLine.transform as RectTransform).position = start;
        
        float length = Vector2.Distance(start, Input.mousePosition) / Canvas.scaleFactor;
        float angle = Vector2.SignedAngle(Vector2.up,end - start);
        (targetingLine.transform as RectTransform).sizeDelta = new Vector2(10,length);
        (targetingLine.transform as RectTransform).eulerAngles = new Vector3(0,0, angle);

        //hits graphics instead of colliders
        //https://docs.unity3d.com/2017.3/Documentation/ScriptReference/UI.GraphicRaycaster.Raycast.html
        if (Input.GetMouseButtonDown(0))
        {

            //RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.zero, 0f);
            //if (hit.transform != null) {
            //    Debug.Log("Input.mousePosition" + hit.transform.gameObject.name);
            //}

            RaycastHit2D[] hits = Physics2D.RaycastAll(Input.mousePosition, Vector2.zero, 0f);
            foreach (RaycastHit2D hit in hits)
            {
                Debug.Log("RaycastAll Hit " + hit.collider.gameObject.name);
                if (GetComponent<ITargets>().IsVaildTarget(hit.collider.gameObject))
                {
                    GetComponent<ITargets>().TargetSelected(hit.collider.gameObject);
                    Destroy(this);
                    return;
                }
            }
            gameObject.GetComponent<ITargets>().TargetCanceled();
            Destroy(this);
        }
    }
    private void OnDestroy()
    {
        (targetingLine.transform as RectTransform).position = new Vector2(-10,0);
        (targetingLine.transform as RectTransform).sizeDelta = new Vector2(10, 100);
        (targetingLine.transform as RectTransform).eulerAngles = new Vector3(0, 0, 0);
    }
}
