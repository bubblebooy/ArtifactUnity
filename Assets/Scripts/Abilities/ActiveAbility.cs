using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ActiveAbility : Ability
{
    public int mana = 1;
    public int baseCooldown = 2;
    public int cooldown = 0;

    public PlayerManager PlayerManager;
    public TextMeshProUGUI displayCooldown;

    public override void OnValidate()
    {
        base.OnValidate();
        displayCooldown = transform.Find("abilityIcon").GetComponentInChildren<TextMeshProUGUI>(true);
        displayCooldown.text = cooldown.ToString();
    }


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

    public override void CardUpdate()
    {
        base.CardUpdate();
        displayCooldown.text = cooldown.ToString();
        displayCooldown.enabled = cooldown > 0;
    }

    public virtual bool IsVaildPlay()
    {
        if (card.hasAuthority &&
            PlayerManager.IsMyTurn &&
            card.GameManager.GameState == "Play" &&
            card.ManaManager.mana >= mana && 
            cooldown <= 0)
        {
            return true;
        }
        return false;
    }

    private void OnClick()
    {
        PlayerManager = PlayerManager ?? card.PlayerManager;
        card.GetComponent<AbilitiesManager>().Collapse();
        if (IsVaildPlay())
        {
            //move to the ability so can handle selecting targets
            PlayerManager.ActivateAbility(card.gameObject, transform.GetSiblingIndex());
        }
    }

    public virtual void OnActivate()
    {
        card.ManaManager.mana -= mana;
        cooldown = baseCooldown;
        // use mana and stuff
    }

    public override void RoundStart()
    {
        base.RoundStart();
        if(cooldown > 0)
        {
            cooldown -= 1;
        }
    }
}

