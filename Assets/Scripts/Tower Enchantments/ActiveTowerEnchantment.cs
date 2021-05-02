using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ActiveTowerEnchantment : TowerEnchantment, ITargets
{
    protected PlayerManager PlayerManager;
    protected ManaManager ManaManager;
    protected LaneManager LaneManager;

    public int mana = 1;
    public int baseCooldown = 1;
    public int cooldown = 0;
    [SerializeField]
    private bool hasSecondTarget = false;
    protected GameObject primaryTarget;

    private TextMeshProUGUI displayCooldown;

    protected override void Awake()
    {
        base.Awake();
        EventTrigger m_EventTrigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { OnClick(); });
        m_EventTrigger.triggers.Add(entry);

        displayCooldown = transform.Find("abilityIcon").GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public override void EnchantmentUpdate()
    {
        base.EnchantmentUpdate();
        displayCooldown.text = cooldown > 0 ? cooldown.ToString() : "";
        // for dealing with retaliate and armor
    }

    public void OnClick()
    {
        LaneManager = LaneManager ?? GetLane();
        PlayerManager = PlayerManager ?? GetLane().GetPlayerManager();
        ManaManager = GameObject.Find((GetSide() == "PlayerSide" ? "Player" : "Enemy") + "Mana").GetComponent<ManaManager>();
        Collapse();
        if (IsVaildPlay())
        {
            TargetSelector targetSelector = gameObject.AddComponent<TargetSelector>();
            transform.Find("frame").GetComponent<Image>().color = Color.green;
        }
    }

    public virtual bool IsVaildPlay()
    {
        if (GetSide() == "PlayerSide" &&
            PlayerManager.IsMyTurn &&
            LaneManager.GameManager.GameState == "Play" &&
            ManaManager.mana >= mana &&
            cooldown <= 0)
        {
            return true;
        }
        return false;
    }

    public virtual bool IsVaildTarget(GameObject target)
    {
        return true;
        //if(primaryTarget != null)
        //{
        //    return true;
        //}
    }

    public void TargetSelected(GameObject target)
    {
        if (hasSecondTarget && primaryTarget != null)
        {
            primaryTarget = target;
            TargetSelector targetSelector = gameObject.AddComponent<TargetSelector>();
        }
        else
        {
            PlayerManager.ActivateTowerEnchantment(GetLane().gameObject,
                                                   GetSide(),
                                                   transform.GetSiblingIndex(),
                                                   Card.GetLineage(target.transform));
        }

    }
    public virtual void OnPlay(GameObject target, GameObject secondaryTarget = null)
    {
        cooldown = baseCooldown;
        transform.Find("frame").GetComponent<Image>().color = Color.white;
        primaryTarget = null;
    }

    public void TargetCanceled()
    {
        transform.Find("frame").GetComponent<Image>().color = Color.white;
        primaryTarget = null;
    }

    public override void RoundStart()
    {
        base.RoundStart();
        if (cooldown > 0)
        {
            cooldown -= 1;
        }
    }
}