using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ActiveTargetAbility : ActiveAbility, ITargets
{

    public TargetValidation[] vaildTargets;
    //protected List<List<string>> selectedTargets = new List<List<string>>();
    protected List<GameObject> selectedTargets = new List<GameObject>();


    public override void ActivateAbility()
    {
        //base.ActivateAbility();
        TargetSelector targetSelector = gameObject.AddComponent<TargetSelector>();
        transform.Find("background").GetComponent<Image>().color = Color.green;
    }

    public virtual bool IsVaildTarget(GameObject target)
    {
        int i = selectedTargets?.Count ?? 0;
        Unit targetUnit = target.GetComponent<Unit>();
        CardSlot targetSlot = target.GetComponentInParent<CardSlot>();
        return (target.tag == vaildTargets[i].targetTag &&
            (vaildTargets[i].targetCardInHand == (target.transform.parent.tag == "Hand")) &&
            (vaildTargets[i].crossLane     || vaildTargets[i].targetCardInHand || targetSlot?.GetLane() == card.GetLane()) &&
            (!vaildTargets[i].targetCaster || (targetUnit.caster && !targetUnit.stun && !targetUnit.silenced)) &&
            (!vaildTargets[i].targetHero   || targetUnit is Hero) &&
            (!vaildTargets[i].targetOnlyPlayerSide || targetSlot.GetSide() == "PlayerSide") &&
            (!vaildTargets[i].targetOnlyEnemySide  || targetSlot.GetSide() == "EnemySide") &&
            (!vaildTargets[i].cantTargetRooted || targetUnit?.rooted == false) &&
            (targetUnit?.hasAuthority != false || targetUnit?.untargetable != true));
    }

    public void TargetSelected(GameObject target)
    {
        //selectedTargets.Add(Card.GetLineage(target.transform));
        selectedTargets.Add(target);
        if (selectedTargets.Count < vaildTargets.Length)
        {
            TargetSelector targetSelector = gameObject.AddComponent<TargetSelector>();
        }
        else
        {
            PlayerManager.ActivateTargetAbility(
                card.gameObject,
                transform.GetSiblingIndex(),
                selectedTargets.Select(x => Card.GetLineage(x.transform)).ToList(),
                quickcast: quickcast);
        }
    }

    public void OnActivate(CardPlayed_e cardPlayed_e, List<GameObject> targets) { }
    public virtual void OnActivate(List<GameObject> targets)
    {
        OnActivate();
        transform.Find("background").GetComponent<Image>().color = backgroundColor;
        selectedTargets.Clear();
    }

    public void TargetCanceled()
    {
        transform.Find("background").GetComponent<Image>().color = backgroundColor;
        selectedTargets.Clear();
    }
}
