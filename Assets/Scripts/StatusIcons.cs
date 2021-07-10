using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusIcons : MonoBehaviour
{
    [System.Serializable]
    public struct StatusIconsGameObjects
    {
        public GameObject bounty;
        public GameObject cleave;
        public GameObject damageImmunity;
        public GameObject deathShield;
        public GameObject decay;
        public GameObject disarmed;
        public GameObject feeble;
        public GameObject leathalToCreeps;
        public GameObject leathalToHeroes;
        public GameObject piercing;
        public GameObject quickstrike;
        public GameObject rapidDeploy;
        public GameObject regeneration;
        public GameObject retaliate;
        public GameObject rooted;
        public GameObject siege;
        public GameObject silenced;
        public GameObject stun;
        public GameObject tracked;
        public GameObject trample;
        public GameObject untargetable;
    }
    public StatusIconsGameObjects statusIcons;

    private List<(System.Type, GameEventSystem.EventListener)> events = new List<(System.Type, GameEventSystem.EventListener)>();

    private Unit card;

    private void Start()
    {
        events.Add(GameEventSystem.Register<GameUpdateUI_e>(GameUpdateUI));
        card = GetComponentInParent<Unit>();
    }

    void GameUpdateUI(GameUpdateUI_e e)
    {
        if (card?.faceup == true && statusIcons.bounty != null)
        {
            statusIcons.bounty.SetActive(card.bounty > (card is Hero ? 5 : 1));
            statusIcons.cleave.SetActive(card.cleave > 0);
            statusIcons.damageImmunity.SetActive(card.damageImmunity);
            statusIcons.deathShield.SetActive(card.deathShield);
            statusIcons.decay.SetActive(card.decay > 0);
            statusIcons.disarmed.SetActive(card.disarmed);
            statusIcons.feeble.SetActive(card.feeble);
            //statusIcons.leathalToCreeps.SetActive();
            //statusIcons.leathalToHeroes.SetActive();
            statusIcons.piercing.SetActive(card.piercing);
            statusIcons.quickstrike.SetActive(card.quickstrike);
            //statusIcons.rapidDeploy.SetActive();
            statusIcons.regeneration.SetActive(card.regeneration > 0);
            statusIcons.retaliate.SetActive(card.retaliate > 0);
            statusIcons.rooted.SetActive(card.rooted);
            statusIcons.siege.SetActive(card.siege > 0);
            statusIcons.silenced.SetActive(card.silenced);
            statusIcons.stun.SetActive(card.stun);
            //statusIcons.tracked.SetActive(card.tracked)
            statusIcons.trample.SetActive(card.trample);
            statusIcons.untargetable.SetActive(card.untargetable);
        }
    }
}
