using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManaManager : MonoBehaviour
{
    public int mana = 30, maxMana = 30;
    private TextMeshProUGUI displayMana, displayMaxMana;

    public void OnValidate()
    {
        displayMana = gameObject.transform.Find("Current").GetComponent<TextMeshProUGUI>();
        displayMaxMana = gameObject.transform.Find("Max").GetComponent<TextMeshProUGUI>();
        displayMana.text = mana.ToString();
        displayMaxMana.text = maxMana.ToString();
    }

    public void ManaUpdate()
    {
        displayMana = gameObject.transform.Find("Current").GetComponent<TextMeshProUGUI>();
        displayMaxMana = gameObject.transform.Find("Max").GetComponent<TextMeshProUGUI>();
        displayMana.text = mana.ToString();
        displayMaxMana.text = maxMana.ToString();
        displayMaxMana.enabled = mana != maxMana;
    }

    public void RoundStart()
    {
        maxMana += 1;
        mana = maxMana;
    }
}
