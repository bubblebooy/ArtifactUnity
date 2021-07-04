using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitPlaceholder : Unit
{
    public override void OnValidate()
    {
        base.OnValidate();
        bounty = 0;
        disarmed = true;
        stun = true;
        damageImmunity = true;
        attack = 0; armor = 0; health = 0;
        maxArmor = 0; maxHealth = 0;
        baseMaxArmor = 0; baseMaxHealth = 0;
    }

    private void Start()
    {
        events.Add(GameEventSystem.Register<GameUpdate_e>(CardUpdate));
    }

    public override void OnStartClient(){}
    public override void CardUpdate()
    {
        if(!string.IsNullOrEmpty(cardName))
        {
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            PlayerManager = networkIdentity.GetComponent<PlayerManager>();
            PlayerManager.CmdSummon(cardName, GetLineage(GetCardSlot().transform));
            cardName = null;
        }
    }
    public override void CheckAlive(GameUpdate_e e) { }
    public override void CardUIUpdate(GameUpdateUI_e e) { }
    public override void OnPlay() { }

    public override void Bounce(bool ignoreRoot = false)
    {
        Destroy(gameObject);
    }
    public override void DestroyCard()
    {
        Destroy(gameObject);
    }
    public override void DestroyCard(bool killed)
    {
        GameEventSystem.Unregister(inPlayEvents);
        base.DestroyCard();
    }
    protected override void OnDestroy()
    {
        GameEventSystem.Unregister(inPlayEvents);
        GameEventSystem.Unregister(events);
    }
}
