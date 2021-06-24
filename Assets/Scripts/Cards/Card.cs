using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public abstract class Card : NetworkBehaviour
{
    public string cardName;
    public int ID;

    [HideInInspector]
    public PlayerManager PlayerManager;
    [HideInInspector]
    public GameManager GameManager;
    [HideInInspector]
    public ManaManager ManaManager;

    [SerializeField]
    private GameObject CardFront;
    [SerializeField]
    private GameObject CardBack;

    public bool staged = false;
    [HideInInspector]
    public bool isDraggable = true;

    public Sprite cardArt;
    public string color;

    public int mana = 0;
    public bool revealed = true;
    public int locked = 0;
    public bool quickcast = false;
    protected int baseMana;
    [TextArea]
    public string cardText;

    protected TextMeshProUGUI displayMana;
    protected TextMeshProUGUI displayCardText;

    public static Dictionary<string, Color> colorDict = new Dictionary<string, Color>()
    {
        { "red", new Color(0.4f, 0.0f, 0.0f, 1.0f) },
        { "green", new Color(0.0f, 0.4f, 0.0f, 1.0f)},
        { "blue", new Color(0.0f, 0.0f, 0.4f, 1.0f)},
        { "black", new Color(0.1f, 0.1f, 0.1f, 1.0f)},
        { "colorless", new Color(0.4f, 0.4f, 0.4f, 1.0f)},
        { "item", new Color(0.7176471f, 0.4784314f, 0.1333333f, 1.0f)}
    };

    public List<(System.Type, GameEventSystem.EventListener)> events = new List<(System.Type, GameEventSystem.EventListener)>();

    public virtual void OnValidate()
    {
        //if(!string.IsNullOrEmpty(cardName) && ID == 0)
        //{
        //    CardIDs IDs = FindObjectOfType<CardIDs>();
        //    if(IDs != null)
        //    {
        //        CardIDs.cardID cid = IDs.cardList.Find(c => c.Name == cardName);
        //        ID = cid.ID; 
        //    }
        //}
        if(CardFront == null)
        {
            CardFront = gameObject.transform.Find("CardFront").gameObject;
            CardBack = gameObject.transform.Find("CardBack").gameObject;
        }

        gameObject.transform.Find("CardFront/Card Name").GetComponent<TextMeshProUGUI>().text = cardName;
        displayMana = gameObject.transform.Find("CardFront/ManaIcon").GetComponentInChildren<TextMeshProUGUI>(true);
        displayMana.text = mana.ToString();

        if (!string.IsNullOrEmpty(cardText))
        {
            gameObject.transform.Find("CardFront/Card Text").gameObject.SetActive(true);
            displayCardText = gameObject.transform.Find("CardFront/Card Text").GetComponentInChildren<TextMeshProUGUI>(true);
            displayCardText.text = cardText;
        }
        else
        {
            gameObject.transform.Find("CardFront/Card Text").gameObject.SetActive(false);
        }


        if (color != null)
        {
            color = color.ToLower();
            if (colorDict.ContainsKey(color))
            {
                gameObject.transform.Find("CardFront/Color").GetComponent<Image>().color = colorDict[color];
            }
        }

        //cardArt;
        transform.Find("CardFront/Image").GetComponent<Image>().sprite = cardArt;
        if(cardArt != null) { transform.Find("CardFront/Image").GetComponent<Image>().color = Color.white; }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        ManaManager = GameObject.Find(( hasAuthority ? "Player" : "Enemy") + "Mana").GetComponent<ManaManager>();
        displayMana = gameObject.transform.Find("CardFront/ManaIcon").GetComponentInChildren<TextMeshProUGUI>(true);
        displayCardText = gameObject.transform.Find("CardFront/Card Text").GetComponentInChildren<TextMeshProUGUI>(true);
        displayCardText.transform.parent.SetAsLastSibling();
        baseMana = mana;
        events.Add(GameEventSystem.Register<GameUpdateUI_e>(CardUIUpdate));
        events.Add(GameEventSystem.Register<RoundStart_e>(DecrementLock));
    }

    public virtual void Clone(GameObject originalGameObject)
    {
        Card originalCard = originalGameObject.GetComponent<Card>();
        mana = originalCard.mana;
        color = originalCard.color;
    }

    //public virtual void InitializeCard(){}
    public virtual void DestroyCard()
    {
        GameEventSystem.Unregister(events);
        GetComponent<CardZoom>().OnHoverExit();
        GetComponent<EventTrigger>().enabled = false;
        gameObject.transform.SetParent(GameObject.Find("Main Canvas").transform,true);
        gameObject.transform.Translate(new Vector3(10, 10, 0));
        StartCoroutine(Discard());
    }

    protected virtual void OnDestroy()
    {
        GameEventSystem.Unregister(events);
    }

    protected virtual IEnumerator Discard()
    {
        yield return new WaitForSeconds(0.0f);
        Transform discard = GameObject.Find((hasAuthority ? "Player" : "Enemy") + "Discard/Pile").transform;
        gameObject.transform.SetParent(discard);
    }

    public void PayMana()
    {
        // gameObject.GetComponentInParent<LaneManager>();
        ManaManager.PayMana(mana, gameObject.GetComponentInParent<LaneManager>());
            //mana -= mana;
    }

    public virtual void OnPlay()
    {
        staged = false;
        isDraggable = false;
        revealed = true;
        CardFront.SetActive(revealed);
        CardBack.SetActive(!revealed);
    }

    public delegate void IsVaildDelegate(ref bool valid, GameObject card);
    public event IsVaildDelegate IsVaildEvent;
    public virtual bool IsVaildPlay(GameObject target)
    {
        //DragDrop.EndDrag also does some validation when determining caster
        Unit targetUnit = target.GetComponent<Unit>();
        if (PlayerManager.IsMyTurn &&
            GameManager.GameState == "Play" &&
            locked <= 0 &&
            ManaManager.CurrentMana(target.GetComponentInParent<LaneManager>()) >= mana &&
            (targetUnit?.hasAuthority != !hasAuthority || targetUnit?.untargetable != true))
        {
            bool valid = true;
            IsVaildEvent?.Invoke(ref valid, target);
            return valid;
        }
        return false;
    }

    public virtual void Stage(CardPlayed_e e)
    {
        PlayerManager.PlayCard(e, GetLineage());
    }


    public virtual void UnStage()
    {
        gameObject.GetComponent<DragDrop>().UnStage();
        staged = false;
    }

    public virtual void CardUIUpdate(GameUpdateUI_e e)
    {
        if (revealed)
        {
            if (displayMana != null) { displayMana.text = mana.ToString(); }
            if (!string.IsNullOrEmpty(cardText)) { displayCardText.text = cardText; }
        }
        CardFront.SetActive(revealed);
        CardBack.SetActive(!revealed);
    }

    public void DecrementLock(RoundStart_e e)
    {
        if(locked > 0)
        {
            locked -= 1;
        }
    }

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
