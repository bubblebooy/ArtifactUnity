using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTargetSpell : Spell, ITargets
{
    public TargetValidation[] vaildTargets;
    // Might want to change selectedTargets to list of gameobjects
    // might need to reference previous selections during validation
    protected List<List<string>> selectedTargets = new List<List<string>>();

    CardPlayed_e cardPlayed_e;
    public override void Stage(CardPlayed_e e)
    {
        cardPlayed_e = e;
        TargetSelector targetSelector = gameObject.AddComponent<TargetSelector>();
    }

    public virtual bool IsVaildTarget(GameObject target)
    {
        int i = selectedTargets?.Count ?? 0;
        Unit targetUnit = target.GetComponent<Unit>();
        CardSlot targetSlot = target.GetComponentInParent<CardSlot>();
        return (target.tag == vaildTargets[i].targetTag &&
            (vaildTargets[i].targetCardInHand == (target.transform.parent.tag == "Hand")) &&
            (vaildTargets[i].crossLane || (targetSlot.GetLane() == transform.parent.gameObject.GetComponent<Unit>().GetLane())) &&
            (!vaildTargets[i].targetCaster || (targetUnit.caster && !targetUnit.stun && !targetUnit.silenced)) &&
            (!vaildTargets[i].targetHero || targetUnit is Hero) &&
            (!vaildTargets[i].targetOnlyPlayerSide || targetSlot.GetSide() == "PlayerSide") &&
            (!vaildTargets[i].targetOnlyEnemySide || targetSlot.GetSide() == "EnemySide") &&
            (!vaildTargets[i].cantTargetRooted || targetUnit?.rooted == false) &&
            (targetUnit?.hasAuthority != false || targetUnit?.untargetable != true));
    }

    public virtual void TargetSelected(GameObject target)
    {
        selectedTargets.Add(Card.GetLineage(target.transform));
        if (selectedTargets.Count < vaildTargets.Length)
        {
            TargetSelector targetSelector = gameObject.AddComponent<TargetSelector>();
        }
        else
        {
            PlayerManager.PlayCard(cardPlayed_e,
                GetLineage(),
                selectedTargets);
        }
    }

    public virtual void OnActivate(CardPlayed_e cardPlayed_e, List<GameObject> targets)
    {
        OnPlay(cardPlayed_e);
        OnActivate(targets);
    }
    public virtual void OnActivate(List<GameObject> targets)
    {
        DestroyCard();
    }

    public virtual void TargetCanceled()
    {
        UnStage();
        selectedTargets.Clear();
    }
}
