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

    public TargetValidation[] vaildTargets;
    //Might want to change this to list of gameobjects
    // might need to reference previous selections during validation
    protected List<List<string>> selectedTargets = new List<List<string>>();

    private TextMeshProUGUI displayCooldown;

    protected override void Awake()
    {
        base.Awake();
        EventTrigger m_EventTrigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { OnClick(); });
        m_EventTrigger.triggers.Add(entry);

        ManaManager = GameObject.Find((GetSide() == "PlayerSide" ? "Player" : "Enemy") + "Mana").GetComponent<ManaManager>();


        inPlayEvents.Add(GameEventSystem.Register<RoundStart_e>(RoundStart));
        inPlayEvents.Add(GameEventSystem.Register<GameUpdateUI_e>(EnchantmentUpdate));

        displayCooldown = transform.Find("abilityIcon").GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public void EnchantmentUpdate(GameUpdateUI_e e)
    {
        displayCooldown.text = cooldown > 0 ? cooldown.ToString() : "";
        // for dealing with retaliate and armor
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
        int i = selectedTargets?.Count ?? 0;
        Unit targetUnit = target.GetComponent<Unit>();
        CardSlot targetSlot = target.GetComponentInParent<CardSlot>();
        return (target.tag == vaildTargets[i].targetTag &&
            (vaildTargets[i].crossLane || (targetSlot.GetLane() == LaneManager)) &&
            (!vaildTargets[i].targetCaster || targetUnit.caster) &&
            (!vaildTargets[i].targetHero || targetUnit is Hero) &&
            (!vaildTargets[i].targetOnlyPlayerSide || targetSlot.GetSide() == "PlayerSide") &&
            (!vaildTargets[i].targetOnlyEnemySide || targetSlot.GetSide() == "EnemySide") &&
            (targetUnit?.hasAuthority != false || targetUnit?.untargetable != true));
    }

    public void OnClick()
    {
        LaneManager = LaneManager ?? GetLane();
        PlayerManager = PlayerManager ?? GetLane().GetPlayerManager();
        ManaManager = ManaManager ?? GameObject.Find((GetSide() == "PlayerSide" ? "Player" : "Enemy") + "Mana").GetComponent<ManaManager>();
        Collapse();
        if (IsVaildPlay())
        {
            transform.Find("frame").GetComponent<Image>().color = Color.green;
            ActivateAbility();
            
        }
    }

    public virtual void ActivateAbility()
    {
        if (selectedTargets.Count < vaildTargets.Length)
        {
            TargetSelector targetSelector = gameObject.AddComponent<TargetSelector>();
        }
        else
        {
            PlayerManager.ActivateTowerEnchantment(
                GetLane().gameObject,
                GetSide(),
                transform.GetSiblingIndex(),
                selectedTargets);
        }

    }

    public void TargetSelected(GameObject target)
    {
        selectedTargets.Add(Card.GetLineage(target.transform));
        ActivateAbility();
    }

    public virtual void OnActivate(List<GameObject> targets)
    {
        cooldown = baseCooldown;
        ManaManager.mana -= mana;
        transform.Find("frame").GetComponent<Image>().color = Color.white;
        selectedTargets.Clear();
    }

    public void TargetCanceled()
    {
        transform.Find("frame").GetComponent<Image>().color = Color.white;
        selectedTargets.Clear();
    }

    public void RoundStart(RoundStart_e e)
    {
        if (cooldown > 0)
        {
            cooldown -= 1;
        }
    }
}