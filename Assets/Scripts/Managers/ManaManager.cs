using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ManaManager : MonoBehaviour
{
    public bool playerMana;
    private int mana = 30, maxMana = 30;
    public float manaGrowth = 1, growthRemainder;
    private TextMeshProUGUI displayMana, displayMaxMana;

    private Dictionary<LaneManager, TowerManager> laneTowerDict;

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

    public void Initialize(Settings settings)
    {
        mana = (int) settings.values.startingMana;
        growthRemainder = settings.values.startingMana % 1;
        manaGrowth = settings.values.manaGrowth;
        maxMana = (int) settings.values.startingMana;
        ManaUpdate(new GameUpdateUI_e());

        laneTowerDict = FindObjectsOfType<TowerManager>()
            .Where(tower => tower.playerTower == playerMana)
            .ToDictionary(tower => tower.GetComponentInParent<LaneManager>(), tower => tower);
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
        growthRemainder += manaGrowth;
        int _manaGrowth = (int)growthRemainder;
        growthRemainder -= _manaGrowth;
        maxMana += _manaGrowth;
        mana = maxMana;
    }

    public void Burn(int burn)
    {
        mana = Mathf.Max(0, mana - burn);
    }

    public int CurrentMana()
    {
        return mana;
    }
    public int CurrentMana(LaneManager lane)
    {
        if(lane == null)
        {
            return CurrentMana();
        }
        return CurrentMana() + laneTowerDict[lane].CurrentMana();
    }

    public void PayMana(int cost)
    {
        mana -= cost;
    }
    public void PayMana(int cost, LaneManager lane)
    {
        PayMana(laneTowerDict[lane].PayMana(cost));
    }

    public int RestoreMana(int restore)
    {
        mana = mana + restore;
        if(mana <= maxMana)
        {
            return 0;
        }
        else
        {
            restore = mana - maxMana;
            mana = maxMana;
            return restore;
        }
    }
    public void RestoreMana(int restore, LaneManager lane)
    {

        laneTowerDict[lane].RestoreMana(RestoreMana(restore));
    }
}
