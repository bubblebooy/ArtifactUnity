using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public abstract class Card : NetworkBehaviour
{
    [HideInInspector]
    public PlayerManager PlayerManager;
    [HideInInspector]
    public GameManager GameManager;
    [HideInInspector]
    public ManaManager ManaManager;

    public bool staged = false;
    [HideInInspector]
    public bool isDraggable = true;

    public Sprite cardArt;
    public string color;
    public string cardName;
    public int mana = 0;
    protected int baseMana;
    [TextArea]
    public string cardText;

    private TextMeshProUGUI displayMana;
    protected TextMeshProUGUI displayCardText;

    //time card takes to be destroyed

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

        if (!string.IsNullOrEmpty(cardText))
        {
            gameObject.transform.Find("Color/Card Text").gameObject.SetActive(true);
            displayCardText = gameObject.transform.Find("Color/Card Text").GetComponentInChildren<TextMeshProUGUI>(true);
            displayCardText.text = cardText;
        }
        else
        {
            gameObject.transform.Find("Color/Card Text").gameObject.SetActive(false);
        }


        if (color != null)
        {
            color = color.ToLower();
            if (colorDict.ContainsKey(color))
            {
                gameObject.transform.Find("Color").GetComponent<Image>().color = colorDict[color];
            }
        }

        //cardArt;
        transform.Find("Color/Image").GetComponent<Image>().sprite = cardArt;
        if(cardArt != null) { transform.Find("Color/Image").GetComponent<Image>().color = Color.white; }
    }


    // Start is called before the first frame update
    public virtual void OnSpawn()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        ManaManager = GameObject.Find(( hasAuthority ? "Player" : "Enemy") + "Mana").GetComponent<ManaManager>();
        displayMana = gameObject.transform.Find("Color/ManaIcon").GetComponentInChildren<TextMeshProUGUI>(true);
        displayCardText = gameObject.transform.Find("Color/Card Text").GetComponentInChildren<TextMeshProUGUI>(true);
        displayCardText.transform.parent.SetAsLastSibling();
        baseMana = mana;

        //InitializeCard();  // should prob just do Start({base.Start()})
    }

    //public virtual void InitializeCard(){}
    public virtual void DestroyCard()
    {
        // Move to graveyard based on player ownership? does artifact have a graveyard?
        GetComponent<CardZoom>().OnHoverExit();
        GetComponent<EventTrigger>().enabled = false;
        gameObject.transform.SetParent(GameObject.Find("Main Canvas").transform,true);
        gameObject.transform.Translate(new Vector3(10, 10, 0));
        StartCoroutine(Discard());

    }

    protected virtual IEnumerator Discard()
    {
        yield return new WaitForSeconds(0.0f);
        Transform discard = GameObject.Find((hasAuthority ? "Player" : "Enemy") + "Discard/Pile").transform;
        gameObject.transform.SetParent(discard);
    }

    public void PayMana()
    {
        ManaManager.mana -= mana;
    }

    public virtual void OnPlay()
    {
        staged = false;
        isDraggable = false;
    }

    public virtual void RoundStart(){}
    public virtual void Scheme(){}

    public virtual bool IsVaildPlay(GameObject target)
    {
        if (PlayerManager.IsMyTurn &&
            GameManager.GameState == "Play" &&
            ManaManager.mana >= mana &&
            target.GetComponent<Unit>()?.untargetable != true)
        {
            return true;
        }
        return false;
    }

    public virtual void Stage()
    {
        PlayerManager.PlayCard(gameObject, GetLineage());
    }

    public virtual void UnStage()
    {
        gameObject.GetComponent<DragDrop>().UnStage();
        staged = false;
    }

    public virtual void CardUpdate()
    {
        // would be more efficnt to remove CardUIUpdate
        // but then would have to call it manually when updating heros in the fountain
        // as is it is being called 2x for every card.
        CardUIUpdate(); 
    }

    public virtual void CardAuras()
    {
        foreach (IAura aura in gameObject.GetComponentsInChildren<IAura>())
        {
            aura.ContinuousEffect();
        }
    }

    public virtual void CardUIUpdate()
    {
        if (displayMana != null) { displayMana.text = mana.ToString(); }
        if (!string.IsNullOrEmpty(cardText)) { displayCardText.text = cardText; }
    }

    //    public abstract void OnPlay();

    public static List<string> GetLineage(Transform t)
    {
        if (t == null) { return null; }
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
