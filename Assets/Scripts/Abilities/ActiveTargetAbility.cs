using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveTargetAbility : ActiveAbility, ITargets
{

    public TargetValidation[] vaildTargets;
    protected List<List<string>> selectedTargets = new List<List<string>>();

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
            (vaildTargets[i].crossLane    || targetSlot.GetLane() == card.GetLane()) &&
            (!vaildTargets[i].targetCaster || (targetUnit.caster && !targetUnit.stun && !targetUnit.silenced)) &&
            (!vaildTargets[i].targetHero   || targetUnit is Hero) &&
            (!vaildTargets[i].targetOnlyPlayerSide || targetSlot.GetSide() == "PlayerSide") &&
            (!vaildTargets[i].targetOnlyEnemySide  || targetSlot.GetSide() == "EnemySide") &&
            (targetUnit?.hasAuthority != false || targetUnit?.untargetable != true));
    }

    public void TargetSelected(GameObject target)
    {
        selectedTargets.Add(Card.GetLineage(target.transform));
        if(selectedTargets.Count < vaildTargets.Length)
        {
            TargetSelector targetSelector = gameObject.AddComponent<TargetSelector>();
        }
        else
        {
            PlayerManager.ActivateTargetAbility(
                card.gameObject,
                transform.GetSiblingIndex(),
                selectedTargets);
        }
    }

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
