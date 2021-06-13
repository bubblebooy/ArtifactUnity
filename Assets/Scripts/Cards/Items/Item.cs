using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Item : Card, ITargets
{
    public TargetValidation vaildPlay;

    public int level;
    public int gold;
    [SerializeField]
    private GameObject abilityPrefab;
    public GameObject ability;

    public Hero hero;

    public enum ItemType
    {
        Weapon,
        Armor,
        Accessories,
        Consumables,
    };
    public ItemType itemType;

    private TextMeshProUGUI displayGold;

    protected List<List<string>> selectedTargets = new List<List<string>>();

    public override void OnValidate()
    {
        base.OnValidate();
        displayGold = gameObject.transform.Find("CardFront/GoldIcon/Gold").GetComponent<TextMeshProUGUI>();
        displayGold.text = gold.ToString();
    }

    public override void Clone(GameObject originalGameObject)
    {
        base.Clone(originalGameObject);
        hero = gameObject.GetComponentInParent<Hero>();
        ability = Instantiate(abilityPrefab, hero.GetComponent<AbilitiesManager>().abilities.transform);
        Ability originalAbility = originalGameObject.GetComponent<Item>().ability.GetComponent<Ability>();
        ability.GetComponent<Ability>().Clone(originalAbility);
    }

    public override bool IsVaildPlay(GameObject target)
    {
        CardSlot targetSlot = target.GetComponentInParent<CardSlot>();
        Unit targetUnit = target.GetComponent<Unit>();
        if (itemType == ItemType.Consumables)
        {

            if (base.IsVaildPlay(target) &&
                target.tag == vaildPlay.targetTag &&
                (!vaildPlay.targetCaster || (targetUnit.caster && !targetUnit.stun && !targetUnit.silenced)) &&
                (!vaildPlay.targetHero || targetUnit is Hero) &&
                (!vaildPlay.targetOnlyPlayerSide || targetSlot.GetSide() == "PlayerSide") &&
                (!vaildPlay.targetOnlyEnemySide || targetSlot.GetSide() == "EnemySide"))
            {
                return true;
            }
        }
        else
        {
            if (base.IsVaildPlay(target) &&
                target.tag == "Unit" &&
                targetUnit is Hero &&
                targetSlot.GetSide() == "PlayerSide")
            {
                foreach(Item item in (targetUnit as Hero).items.GetComponentsInChildren<Item>())
                {
                    if(item.name == name)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        return false;

    }

    public virtual bool IsVaildTarget(GameObject target)
    {
        Item item = target.GetComponent<Item>();
        return item != null && 
            item.hero == gameObject.GetComponentInParent<Hero>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        displayGold = gameObject.transform.Find("CardFront/GoldIcon/Gold").GetComponent<TextMeshProUGUI>();
        displayGold.text = gold.ToString();
    }

    CardPlayed_e cardPlayed_e;
    public override void Stage(CardPlayed_e e)
    {
        Hero hero = gameObject.transform.parent.GetComponentInChildren<Hero>();
        if (itemType != ItemType.Consumables && hero.items.transform.childCount > hero.maxItemCount)
        {
            cardPlayed_e = e;
            TargetSelector targetSelector = gameObject.AddComponent<TargetSelector>();
            //Expand items
        }
        else
        {
            base.Stage(e);
        }
    }

    public override void OnPlay()
    {
        base.OnPlay();
        if(itemType != ItemType.Consumables)
        {
            hero = gameObject.GetComponentInParent<Hero>();
            gameObject.transform.SetParent(hero.items.transform);
            ability = Instantiate(abilityPrefab, hero.GetComponent<AbilitiesManager>().abilities.transform);
        }
    }

    public virtual void TargetSelected(GameObject target)
    {
        selectedTargets.Add(GetLineage(target.transform));
        PlayerManager.PlayCard(cardPlayed_e,
            GetLineage(),
            selectedTargets);
    }

    public virtual void OnActivate(List<GameObject> targets)
    {
        //Bounce
        OnPlay();
    }

    public virtual void TargetCanceled()
    {
        //colapse items
        UnStage();
        selectedTargets.Clear();
        
    }
}
