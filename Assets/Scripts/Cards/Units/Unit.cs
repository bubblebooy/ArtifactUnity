using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Unit : Card
{
    public int attack, armor, health;
    protected int maxArmor, maxHealth;
    private TextMeshProUGUI displayAttack, displayArmor, displayHealth;

    public int arrow = 0;
    public bool quickstrike = false;
    public int cleave = 0;
    public bool disarmed = false;
    public bool caster = false;
    public bool piercing = false;

    public string targetTag = "Card Slot";


    public override void OnValidate()
    {
        base.OnValidate();
        displayAttack = gameObject.transform.Find("Color/Attack").GetComponent<TextMeshProUGUI>();
        displayArmor = gameObject.transform.Find("Color/Armor").GetComponent<TextMeshProUGUI>();
        displayHealth = gameObject.transform.Find("Color/Health").GetComponent<TextMeshProUGUI>();
        displayAttack.text = "<sprite=0>" + attack.ToString();
        displayArmor.text = "<sprite=1>" + armor.ToString();
        displayHealth.text = "<sprite=2>" + health.ToString();
    }

    protected override void Start()
    {
        base.Start();
        maxArmor = armor;
        maxHealth = health;
        displayAttack = gameObject.transform.Find("Color/Attack").GetComponent<TextMeshProUGUI>();
        displayArmor = gameObject.transform.Find("Color/Armor").GetComponent<TextMeshProUGUI>();
        displayHealth = gameObject.transform.Find("Color/Health").GetComponent<TextMeshProUGUI>();
        displayAttack.text = "<sprite=0>" + attack.ToString();
        displayArmor.text = "<sprite=1>" + armor.ToString();
        displayHealth.text = "<sprite=2>" + health.ToString();
    }

    public override bool IsVaildPlay(GameObject target)
    {
        if (base.IsVaildPlay(target) &&
            target.tag == targetTag &&
            target.transform.parent.name == "PlayerSide")
        {
            return true;
        }
        return false;
    }

    public override void CardUpdate()
    {
        base.CardUpdate();
        displayAttack = gameObject.transform.Find("Color/Attack").GetComponent<TextMeshProUGUI>();
        displayArmor = gameObject.transform.Find("Color/Armor").GetComponent<TextMeshProUGUI>();
        displayHealth = gameObject.transform.Find("Color/Health").GetComponent<TextMeshProUGUI>();
        displayAttack.text = "<sprite=0>" + attack.ToString();
        displayArmor.text = "<sprite=1>" + armor.ToString();
        displayHealth.text = "<sprite=2>" + health.ToString();

        // is alive
        // check arrow
        if (health <= 0)
        {
            DestroyCard();
        }
    }

    public virtual void Bounce()
    {
        // Move to hand based on player ownership? or just player side?
        gameObject.transform.SetParent(
            GameObject.Find(hasAuthority ? "PlayerArea" : "EnemyArea").transform,
            false);
        isDraggable = true;
    }


    public override void RoundStart() {
        armor = maxArmor;

        CardUpdate();
    }

    public virtual void Strike(Unit target, int damage, bool piercing = false)
    {
        target.Damage(damage, piercing);
    }

    public virtual void Damage(int damage, bool piercing = false)
    {
        armor -= damage;
        if (armor < 0)
        {
            health += armor;
            armor = 0;
        } 
    }

    public virtual void Combat( bool quick = false)
    {
        if (quick == quickstrike && !disarmed)
        {
            int slotNumber = transform.parent.GetSiblingIndex();
            int numberOfSlots = transform.parent.parent.GetComponentsInChildren<CardSlot>().Length;
            bool player = transform.parent.parent.name == "PlayerSide";
            // Assume that the target is pointer at a card slot. the arrow should be managed in CardUpdate()
            Transform targetSlot = transform.parent.parent.parent.Find(player ? "EnemySide" : "PlayerSide").GetChild(slotNumber + arrow);
            //player ? "PlayerSide" : "EnemySide" + $"/Card Slot ({slotNumber + arrow})";
            Unit target = targetSlot.GetComponentInChildren<Unit>();
            //Debug.Log($"{transform.parent.name} : {slotNumber}");
            //Debug.Log(targetSlot.name);

            if (target != null)
            {
                Strike(target, attack + cleave, piercing);
            }
            else
            {
                TowerManager tower = transform.parent.parent.parent.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
                tower.Damage(attack, piercing);
            }
            if (cleave != 0)
            {
                for (int i = -1; i <= 1; i ++)
                {
                    if (i == arrow) { continue; }
                    if (slotNumber + i < 0 || slotNumber + i >= numberOfSlots) { continue; }
                    targetSlot = transform.parent.parent.parent.Find(player ? "PlayerSide" : "EnemySide").GetChild(slotNumber + i);
                    target = targetSlot.GetComponentInChildren<Unit>();
                    if (targetSlot.GetChild(0).gameObject != null)
                    {
                        Strike(target, cleave, piercing);
                    }
                }
            }
            
        }
    }

    public void quickstrikeDead()
    {
        if (health <= 0)
        {
            disarmed = true;
        }
    }


    public string GetSide()
    {
        CardSlot cardSlot = transform.parent.GetComponent<CardSlot>();
        return cardSlot.GetSide();
    }

    public TowerManager GetTower()
    {
        CardSlot cardSlot = transform.parent.GetComponent<CardSlot>();
        return cardSlot.GetTower();
    }

    public LaneManager GetLane()
    {
        CardSlot cardSlot = transform.parent.GetComponent<CardSlot>();
        return cardSlot.GetLane();
    }

}
