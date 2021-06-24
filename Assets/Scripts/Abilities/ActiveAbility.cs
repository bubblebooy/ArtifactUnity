using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ActiveAbility : CooldownAbility
{
    public int mana = 1;

    public bool quickcast = false;

    public PlayerManager PlayerManager;

    protected override void Awake()
    {
        base.Awake();

        EventTrigger m_EventTrigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { OnClick(); });
        m_EventTrigger.triggers.Add(entry);
    }

    public override void Clone(Ability originalAbility)
    {
        base.Clone(originalAbility);
        mana = (originalAbility as ActiveAbility).mana;
    }

    public virtual bool IsVaildPlay()
    {
        if (card.hasAuthority &&
            !(card.stun || card.silenced) &&
            PlayerManager.IsMyTurn &&
            card.GameManager.GameState == "Play" &&
            card.ManaManager.CurrentMana(gameObject.GetComponentInParent<LaneManager>()) >= mana && 
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
            ActivateAbility();
        }
    }

    public virtual void ActivateAbility()
    {
        PlayerManager.ActivateAbility(card.gameObject, transform.GetSiblingIndex(),quickcast: quickcast);
    }

    public virtual void OnActivate()
    {
        //card.ManaManager.mana -= mana;
        card.ManaManager.PayMana(mana, gameObject.GetComponentInParent<LaneManager>());
        cooldown = baseCooldown;
    }

}

