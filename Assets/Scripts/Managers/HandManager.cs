using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public UIManager UIManager;

    void Start()
    {
        UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    public void OnClick()
    {
        UIManager.ZoomHand(true);
    }
}
