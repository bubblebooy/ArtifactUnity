using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public abstract class Card : NetworkBehaviour
{
    public PlayerManager PlayerManager;
    public GameManager GameManager;
    public ManaManager ManaManager;

    public bool staged { get; protected set; } = false;
    public bool isDraggable = true;

    public string color;
    public string cardName;
    public int mana = 0;
    private TextMeshProUGUI displayMana;

    private Dictionary<string, Color> colorDict = new Dictionary<string, Color>()
    {
        { "red", new Color(0.4f, 0.0f, 0.0f, 1.0f) },
        { "green", new Color(0.0f, 0.4f, 0.0f, 1.0f)},
        { "blue", new Color(0.0f, 0.0f, 0.4f, 1.0f)},
        { "black", new Color(0.1f, 0.1f, 0.1f, 1.0f)},
        { "colorless", new Color(0.4f, 0.4f, 0.4f, 1.0f)}
    };

    public virtual void OnValidate()
    {
        gameObject.transform.Find("Color/Card Name").GetComponent<TextMeshProUGUI>().text = cardName;
        displayMana = gameObject.transform.Find("Color/ManaIcon").GetComponentInChildren<TextMeshProUGUI>(true);
        displayMana.text = mana.ToString();
        
        if (color != null)
        {
            color = color.ToLower();
            if (colorDict.ContainsKey(color))
            {
                gameObject.transform.Find("Color").GetComponent<Image>().color = colorDict[color];
            }
        }
    }


    // Start is called before the first frame update
    protected virtual void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        ManaManager = GameObject.Find(( hasAuthority ? "Player" : "Enemy") + "Mana").GetComponent<ManaManager>();
        displayMana = gameObject.transform.Find("Color/ManaIcon").GetComponentInChildren<TextMeshProUGUI>(true);

        //InitializeCard();  // should prob just do Start({base.Start()})
    }

    //public virtual void InitializeCard(){}

    public virtual void DestroyCard()
    {
        // Move to graveyard based on player ownership? does artifact have a graveyard?
        Destroy(gameObject);
    }

    public virtual void OnPlay()
    {
        staged = false;
        isDraggable = false;
        ManaManager.mana -= mana;
    }

    public virtual void RoundStart(){}

    public virtual bool IsVaildPlay(GameObject target)
    {
        if (PlayerManager.IsMyTurn &&
            GameManager.GameState == "Play" &&
            ManaManager.mana >= mana)
        {
            return true;
        }
        return false;
    }

    public virtual void Stage()
    {
        PlayerManager.PlayCard(gameObject, GetLineage());
    }

    public virtual void unStage()
    {
        gameObject.GetComponent<DragDrop>().unStage();
        staged = false;
    }

    public virtual void CardUpdate()
    {
        if (displayMana != null) { displayMana.text = mana.ToString(); }
    }

    //    public abstract void OnPlay();

    public List<string> GetLineage(Transform t)
    {
        List<string> lineage = new List<string>();
        while (t.name != t.root.name)
        {
            lineage.Add(t.name);
            t = t.parent;
        }
        lineage.Reverse();
        return lineage;
    }
    public List<string> GetLineage()
    {
        return GetLineage(gameObject.transform.parent);
    }

}