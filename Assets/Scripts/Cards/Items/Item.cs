using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Item : Card, ITargets, IItem
{
    [field: SerializeField]
    public int level { get; set; }
    [field: SerializeField]
    public int gold { get; set; }
    [SerializeField]
    private GameObject abilityPrefab;
    [HideInInspector]
    public GameObject ability;

    public Hero hero;

    private int sibIndex;

    [field: SerializeField]
    public ItemType itemType { get; set; }

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
        foreach(Ability originalAbility in originalGameObject.GetComponent<Item>().ability.GetComponents<Ability>())
        {
            (ability.GetComponent(originalAbility.GetType()) as Ability).Clone(originalAbility);
        }
        //Ability originalAbility = originalGameObject.GetComponent<Item>().ability.GetComponent<Ability>();
        //ability.GetComponent<Ability>().Clone(originalAbility);
    }

    public override bool IsVaildPlay(GameObject target)
    {
        CardSlot targetSlot = target.GetComponentInParent<CardSlot>();
        Unit targetUnit = target.GetComponent<Unit>();
        if (itemType == ItemType.Consumables)
        {
            print("Consumables should be a spell or unit with the IItems interface");
            return false;
        }
        else
        {
            if (base.IsVaildPlay(target) &&
                target.tag == "Unit" &&
                targetUnit is Hero &&
                targetSlot.GetSide() == "PlayerSide")
            {
                foreach (Item item in (targetUnit as Hero).items.GetComponentsInChildren<Item>())
                {
                    if (item.name == name)
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
        hero.GetComponent<CardZoom>().OnHoverExit();
        gameObject.GetComponent<CardZoom>().enabled = false;
        hero.GetComponent<CardZoom>().enabled = false;
        if (itemType != ItemType.Consumables && hero.items.transform.childCount >= hero.maxItemCount)
        {
            cardPlayed_e = e;
            TargetSelector targetSelector = gameObject.AddComponent<TargetSelector>();
            Expand();
        }
        else
        {
            base.Stage(e);
        }
    }

    public override void OnPlay(CardPlayed_e cardPlayed_e)
    {
        if (itemType != ItemType.Consumables)
        {
            hero = gameObject.GetComponentInParent<Hero>();
            gameObject.transform.SetParent(hero.items.transform);
            ability = Instantiate(abilityPrefab, hero.GetComponent<AbilitiesManager>().abilities.transform);
            hero.GetComponent<CardZoom>().enabled = true;
        }
        base.OnPlay(cardPlayed_e);
    }

    public virtual void TargetSelected(GameObject target)
    {
        Collapse();
        selectedTargets.Add(GetLineage(target.transform));
        PlayerManager.PlayCard(cardPlayed_e,
            GetLineage(),
            selectedTargets);
    }

    public virtual void OnActivate(CardPlayed_e cardPlayed_e, List<GameObject> targets)
    {
        OnActivate(targets);
        OnPlay(cardPlayed_e);
    }
    public virtual void OnActivate(List<GameObject> targets)
    {
        targets[0].GetComponent<Item>().locked = 1;
        targets[0].GetComponent<Item>().Bounce();
    }

    public virtual void TargetCanceled()
    {
        Collapse();
        UnStage();
        selectedTargets.Clear();

    }

    public void Bounce()
    {
        gameObject.transform.SetParent(
            GameObject.Find(hasAuthority ? "PlayerOverDraw" : "EnemyOverDraw").transform,
            false);
        isDraggable = true;
        displayCardText.transform.parent.gameObject.SetActive(true);
        gameObject.GetComponent<CardZoom>().enabled = true;
        Destroy(ability);
    }

    public override void DestroyCard()
    {
        Destroy(ability);
        base.DestroyCard();
    }


    private void Expand()
    {
        Transform items = gameObject.GetComponentInParent<Hero>().items.transform;
        sibIndex = items.GetSiblingIndex();
        items.gameObject.SetActive(true);
        (items as RectTransform).sizeDelta = Vector2.right * items.childCount * (transform as RectTransform).sizeDelta;

        items.SetParent(GameObject.Find("Main Canvas").transform, true);
    }
    private void Collapse()
    {
        Transform items = gameObject.GetComponentInParent<Hero>().items.transform;
        items.SetParent(gameObject.GetComponentInParent<Hero>().transform, true);
        items.SetSiblingIndex(sibIndex);
        (items as RectTransform).sizeDelta = Vector2.zero;
        items.gameObject.SetActive(false);
    }
}
