using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetSelector : MonoBehaviour
{
    //// Update is called once per frame
    void Update()
    {
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
}
