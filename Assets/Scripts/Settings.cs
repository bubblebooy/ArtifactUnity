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
    private TMP_InputField inputStartingMana;
    [SerializeField]
    private TMP_InputField inputStartingGold;
    [SerializeField]
    private TMP_InputField inputStartingHand;
    [SerializeField]
    private TMP_InputField inputCardDraw;
    [SerializeField]
    private Button OpenSettings;
    [SerializeField]
    private Button CloseSettings;


    [System.Serializable]
    public struct Values
    {
        public int startingMana;
        public int startingGold;
        public int startingHand;
        public float cardDraw;
    }
    public Values values;

    private void OnValidate()
    {
        UpdateUI();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadPlayerPrefs();
    }

    public void LoadPlayerPrefs()
    {
        values.startingMana = PlayerPrefs.GetInt("StartingMana", values.startingMana);
        values.startingGold = PlayerPrefs.GetInt("StartingGold", values.startingGold);
        values.startingHand = PlayerPrefs.GetInt("StartingHand", values.startingHand);
        values.cardDraw = PlayerPrefs.GetFloat("CardDraw", values.cardDraw);
        UpdateUI();
    }

    public void SettingsChanged()
    {
        lobbyPlayerManager?.CmdSettingsChanged(values);
    }

    public void UpdateUI()
    {
        inputStartingMana.text = values.startingMana.ToString();
        inputStartingGold.text = values.startingGold.ToString();
        inputStartingHand.text = values.startingHand.ToString();
        inputCardDraw.text = values.cardDraw.ToString();
    }
    public void UIInteractable(bool interactable = true)
    {
        inputStartingMana.interactable = interactable;
        inputStartingGold.interactable = interactable;
        inputStartingHand.interactable = interactable;
        inputCardDraw.interactable = interactable;
    }

    public void ChangeStartingMana()
    {
        values.startingMana = int.Parse(inputStartingMana.text);
        PlayerPrefs.SetInt("StartingMana", values.startingMana);
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

    public void Initialize()
    {
        foreach( ManaManager manaManager in FindObjectsOfType<ManaManager>())
        {
            manaManager.SetMana(values.startingMana);
        }
        foreach (GoldManager goldManager in FindObjectsOfType<GoldManager>())
        {
            goldManager.SetGold(values.startingGold);
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
