using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManaManager : MonoBehaviour
{
    public int mana = 30, maxMana = 30;
    private TextMeshProUGUI displayMana, displayMaxMana;

    public List<(System.Type, GameEventSystem.EventListener)> events = new List<(System.Type, GameEventSystem.EventListener)>();

    public void OnValidate()
    {
        displayMana = gameObject.transform.Find("Current").GetComponent<TextMeshProUGUI>();
        displayMaxMana = gameObject.transform.Find("Max").GetComponent<TextMeshProUGUI>();
        displayMana.text = mana.ToString();
        displayMaxMana.text = maxMana.ToString();
    }

    private void Start()
    {
        events.Add(GameEventSystem.Register<RoundStart_e>(RoundStart));

    }

    public void ManaUpdate()
    {
        displayMana = gameObject.transform.Find("Current").GetComponent<TextMeshProUGUI>();
        displayMaxMana = gameObject.transform.Find("Max").GetComponent<TextMeshProUGUI>();
        displayMana.text = mana.ToString();
        displayMaxMana.text = maxMana.ToString();
        displayMaxMana.enabled = mana != maxMana;
    }

    public void RoundStart(RoundStart_e e)
    {
        maxMana += 1;
        mana = maxMana;
    }
}
