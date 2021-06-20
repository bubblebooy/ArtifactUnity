using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldManager : MonoBehaviour
{
    public int gold = 45;
    private TextMeshProUGUI displayGold;

    public List<(System.Type, GameEventSystem.EventListener)> events = new List<(System.Type, GameEventSystem.EventListener)>();

    public void OnValidate()
    {
        displayGold = gameObject.transform.Find("Gold").GetComponent<TextMeshProUGUI>();
        displayGold.text = "";//gold.ToString();
    }

    private void Start()
    {
        events.Add(GameEventSystem.Register<GameUpdateUI_e>(GoldUpdate));

    }

    public void SetGold(int _gold)
    {
        gold = _gold;
        GoldUpdate(new GameUpdateUI_e());
    }

    public void GoldUpdate(GameUpdateUI_e e)
    {
        displayGold = gameObject.transform.Find("Gold").GetComponent<TextMeshProUGUI>();
        displayGold.text = gold.ToString();
    }
}
