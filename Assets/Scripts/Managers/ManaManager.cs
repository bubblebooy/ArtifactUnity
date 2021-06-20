using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManaManager : MonoBehaviour
{
    public int mana = 30, maxMana = 30;
    public int manaIncrease = 1;
    private TextMeshProUGUI displayMana, displayMaxMana;

    public List<(System.Type, GameEventSystem.EventListener)> events = new List<(System.Type, GameEventSystem.EventListener)>();

    public void OnValidate()
    {
        displayMana = gameObject.transform.Find("Current").GetComponent<TextMeshProUGUI>();
        displayMaxMana = gameObject.transform.Find("Max").GetComponent<TextMeshProUGUI>();
        displayMana.text = "";// mana.ToString();
        displayMaxMana.text = "";// maxMana.ToString();
    }

    private void Start()
    {
        events.Add(GameEventSystem.Register<RoundStart_e>(RoundStart));
        events.Add(GameEventSystem.Register<GameUpdateUI_e>(ManaUpdate));
    }

    public void SetMana(int _mana)
    {
        mana = _mana;
        maxMana = _mana;
        ManaUpdate(new GameUpdateUI_e());
    }

    public void ManaUpdate(GameUpdateUI_e e)
    {
        displayMana = gameObject.transform.Find("Current").GetComponent<TextMeshProUGUI>();
        displayMaxMana = gameObject.transform.Find("Max").GetComponent<TextMeshProUGUI>();
        displayMana.text = mana.ToString();
        displayMaxMana.text = maxMana.ToString();
        displayMaxMana.enabled = mana != maxMana;
    }

    public void RoundStart(RoundStart_e e)
    {
        maxMana += manaIncrease;
        mana = maxMana;
    }

    public void Burn(int burn)
    {
        mana = Mathf.Max(0, mana - burn);
    }
}
