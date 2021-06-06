using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyToggle : MonoBehaviour
{
    [SerializeField]
    private bool connected;
    [SerializeField]
    private bool ready;

    [SerializeField]
    private Image background;
    [SerializeField]
    private Color connectedColor;
    [SerializeField]
    private Color disconnectedColor;

    [SerializeField]
    private Image checkmark;
    [SerializeField]
    private Color ReadyColor;
    [SerializeField]
    private Color NotReadyColor;

    private void OnValidate()
    {
        Ready(ready);
        Connected(connected);
    }

    public void Ready(bool isReady)
    {
        ready = isReady;
        checkmark.color = isReady ? ReadyColor : NotReadyColor;
    }
    public void Connected(bool isConnected)
    {
        connected = isConnected;
        background.color = isConnected ? connectedColor : disconnectedColor;
        checkmark.gameObject.SetActive(isConnected);
    }

}
