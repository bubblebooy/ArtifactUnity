using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    public LobbyPlayerManager lobbyPlayerManager;
    public PlayerManager playerManager;

    [SerializeField]
    private GameObject SettingsDisplay;

    [SerializeField]
    private TMP_InputField inputTowerHealth;
    [SerializeField]
    private TMP_InputField inputAncientHealth;
    [SerializeField]
    private TMP_InputField inputStartingMana;
    [SerializeField]
    private TMP_InputField inputManaGrowth;
    [SerializeField]
    private TMP_InputField inputTowerMana;
    [SerializeField]
    private TMP_InputField inputTowerManaGrowth;
    [SerializeField]
    private TMP_InputField inputStartingGold;
    [SerializeField]
    private TMP_InputField inputStartingHand;
    [SerializeField]
    private TMP_InputField inputCardDraw;
    [SerializeField]
    private TMP_InputField inputMaxHandSize;
    [SerializeField]
    private TMP_InputField inputNumberOfSlots;
    [SerializeField]
    private Button OpenSettings;
    [SerializeField]
    private Button CloseSettings;


    [System.Serializable]
    public struct Values
    {
        public int towerHealth;
        public int ancientHealth;

        public float startingMana;
        public float manaGrowth;
        public float towerMana;
        public float towerManaGrowth;

        public int startingGold;
        public int startingHand;
        public float cardDraw;
        public int maxHandSize;
        public int numberOfSlots;
    }
    public Values values;
    public bool debug = false;

    private static Settings settingsInstance;
    private void OnValidate()
    {
        UpdateUI();
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (settingsInstance is null)
        {
            settingsInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        LoadPlayerPrefs();
    }

    public void LoadPlayerPrefs()
    {
        if (debug){return;}
        //                                          Key             Default Value
        values.towerHealth = PlayerPrefs.GetInt("TowerHealth", values.towerHealth);
        values.ancientHealth = PlayerPrefs.GetInt("AncientHealth", values.ancientHealth);

        values.startingMana = PlayerPrefs.GetFloat("StartingMana", values.startingMana);
        values.manaGrowth = PlayerPrefs.GetFloat("ManaGrowth", values.manaGrowth);
        values.towerMana = PlayerPrefs.GetFloat("TowerMana", values.towerMana);
        values.towerManaGrowth = PlayerPrefs.GetFloat("TowerManaGrowth", values.towerManaGrowth);

        values.startingGold = PlayerPrefs.GetInt("StartingGold", values.startingGold);
        values.startingHand = PlayerPrefs.GetInt("StartingHand", values.startingHand);
        values.cardDraw = PlayerPrefs.GetFloat("CardDraw", values.cardDraw);
        values.maxHandSize = PlayerPrefs.GetInt("MaxHandSize", values.maxHandSize);
        values.numberOfSlots = PlayerPrefs.GetInt("NumberOfSlots", values.numberOfSlots);

        UpdateUI();
    }

    public void SettingsChanged()
    {
        lobbyPlayerManager?.CmdSettingsChanged(values);
    }

    public void UpdateUI()
    {
        inputTowerHealth.text = values.towerHealth.ToString();
        inputAncientHealth.text = values.ancientHealth.ToString();

        inputStartingMana.text = values.startingMana.ToString();
        inputManaGrowth.text = values.manaGrowth.ToString();
        inputTowerMana.text = values.towerMana.ToString();
        inputTowerManaGrowth.text = values.towerManaGrowth.ToString();

        inputStartingGold.text = values.startingGold.ToString();
        inputStartingHand.text = values.startingHand.ToString();
        inputCardDraw.text = values.cardDraw.ToString();
        inputMaxHandSize.text = values.maxHandSize.ToString();
        inputNumberOfSlots.text = values.numberOfSlots.ToString();
    }
    public void UIInteractable(bool interactable = true)
    {
        inputTowerHealth.interactable = interactable;
        inputAncientHealth.interactable = interactable;

        inputStartingMana.interactable = interactable;
        inputManaGrowth.interactable = interactable;
        inputTowerMana.interactable = interactable;
        inputTowerManaGrowth.interactable = interactable;

        inputStartingGold.interactable = interactable;
        inputStartingHand.interactable = interactable;
        inputCardDraw.interactable = interactable;
        inputMaxHandSize.interactable = interactable;
        inputNumberOfSlots.interactable = interactable;
    }

    public void ChangeTowerHealth()
    {
        values.towerHealth = int.Parse(inputTowerHealth.text);
        PlayerPrefs.SetInt("TowerHealth", values.towerHealth);
        PlayerPrefs.Save();
        SettingsChanged();
    }
    public void ChangeAncientHealth()
    {
        values.ancientHealth = int.Parse(inputAncientHealth.text);
        PlayerPrefs.SetInt("AncientHealth", values.ancientHealth);
        PlayerPrefs.Save();
        SettingsChanged();
    }
    public void ChangeStartingMana()
    {
        values.startingMana = float.Parse(inputStartingMana.text);
        PlayerPrefs.SetFloat("StartingMana", values.startingMana);
        PlayerPrefs.Save();
        SettingsChanged();
    }
    public void ChangeManaGrowth()
    {
        values.manaGrowth = float.Parse(inputManaGrowth.text);
        PlayerPrefs.SetFloat("ManaGrowth", values.manaGrowth);
        PlayerPrefs.Save();
        SettingsChanged();
    }
    public void ChangeTowerMana()
    {
        values.towerMana = float.Parse(inputTowerMana.text);
        PlayerPrefs.SetFloat("TowerMana", values.towerMana);
        PlayerPrefs.Save();
        SettingsChanged();
    }
    public void ChangeTowerManaGrowth()
    {
        values.towerManaGrowth = float.Parse(inputTowerManaGrowth.text);
        PlayerPrefs.SetFloat("TowerManaGrowth", values.towerManaGrowth);
        PlayerPrefs.Save();
        SettingsChanged();
    }

    public void ChangeStartingGold()
    {
        values.startingGold = int.Parse(inputStartingGold.text);
        PlayerPrefs.SetInt("StartingGold", values.startingGold);
        PlayerPrefs.Save();
        SettingsChanged();
    }
    public void ChangeStartingHand()
    {
        values.startingHand = int.Parse(inputStartingHand.text);
        PlayerPrefs.SetInt("StartingHand", values.startingHand);
        PlayerPrefs.Save();
        SettingsChanged();
    }
    public void ChangeCardDraw()
    {
        values.cardDraw = float.Parse(inputCardDraw.text);
        PlayerPrefs.SetFloat("CardDraw", values.cardDraw);
        PlayerPrefs.Save();
        SettingsChanged();
    }
    public void ChangeMaxHandSize()
    {
        values.maxHandSize = int.Parse(inputMaxHandSize.text);
        PlayerPrefs.SetInt("MaxHandSize", values.maxHandSize);
        PlayerPrefs.Save();
        SettingsChanged();
    }
    public void ChangeNumberOfSlots()
    {
        values.numberOfSlots = int.Parse(inputNumberOfSlots.text);
        PlayerPrefs.SetInt("NumberOfSlots", values.numberOfSlots);
        PlayerPrefs.Save();
        SettingsChanged();
    }

    public void Initialize()
    {
        foreach( ManaManager manaManager in FindObjectsOfType<ManaManager>())
        {
            manaManager.Initialize(this);
        }
        foreach (TowerManager towerManager in FindObjectsOfType<TowerManager>())
        {
            towerManager.Initialize(this);
        }
        foreach (GoldManager goldManager in FindObjectsOfType<GoldManager>())
        {
            goldManager.SetGold(values.startingGold);
        }
        foreach (OverDrawManager overDrawManager in FindObjectsOfType<OverDrawManager>())
        {
            overDrawManager.maxHandSize = values.maxHandSize;
        }
    }

    public void Collapse()
    {
        SettingsDisplay.SetActive(false);
        OpenSettings.gameObject.SetActive(true);
    }
    public void Expand()
    {
        SettingsDisplay.SetActive(true);
        OpenSettings.gameObject.SetActive(false);
    }
}
