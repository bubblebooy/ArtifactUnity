using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Unit : Card
{
    public int attack, armor, health;
    protected int baseAttack;//, baseArmor, baseHealth;
    [HideInInspector]
    public int maxArmor, maxHealth;
    protected int baseMaxArmor, baseMaxHealth;

    private TextMeshProUGUI displayAttack, displayArmor, displayHealth;
    private Transform displayArrow;

    // for now think it is best to have these all be the base values and if a card has one of these inherently add it to the onStart and onUpdate
    [HideInInspector]
    public int arrow = 0;
    [HideInInspector]
    public bool quickstrike = false;
    [HideInInspector]
    public int cleave = 0;
    [HideInInspector]
    public bool disarmed = false;
    [HideInInspector]
    public bool caster = false;
    [HideInInspector]
    public bool piercing = false;
    [HideInInspector]
    public bool trample = false;
    [HideInInspector]
    public bool feeble = false;


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

    public override void OnSpawn()
    {
        base.OnSpawn();
        maxArmor = armor;
        maxHealth = health;
        baseAttack = attack;
        //baseArmor = armor;
        //baseHealth = health;
        baseMaxArmor = maxArmor;
        baseMaxHealth = maxHealth;
        displayAttack = gameObject.transform.Find("Color/Attack").GetComponent<TextMeshProUGUI>();
        displayArmor = gameObject.transform.Find("Color/Armor").GetComponent<TextMeshProUGUI>();
        displayHealth = gameObject.transform.Find("Color/Health").GetComponent<TextMeshProUGUI>();
        displayAttack.text = "<sprite=0>" + attack.ToString();
        displayArmor.text = "<sprite=1>" + armor.ToString();
        displayHealth.text = "<sprite=2>" + health.ToString();
        if (disarmed) { displayAttack.color = Color.grey; }
        displayArrow = gameObject.transform.Find("Color/Arrow");
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

        GetComponent<AbilitiesManager>().CardUpdate();

        attack = baseAttack;
        //armor = baseArmor;
        //health = baseHealth;
        maxArmor = baseMaxArmor;
        maxHealth = baseMaxHealth;

        cleave = 0;

        quickstrike = false;
        disarmed = false;
        //caster = false;
        piercing = false;
        trample = false;
        feeble = false;

        foreach (IModifier mod in gameObject.GetComponentsInChildren<IModifier>())
        {
            mod.ModifyCard();
        }
        // If this is needed I think we have bigger problems
        //displayAttack = gameObject.transform.Find("Color/Attack").GetComponent<TextMeshProUGUI>();
        //displayArmor = gameObject.transform.Find("Color/Armor").GetComponent<TextMeshProUGUI>();
        //displayHealth = gameObject.transform.Find("Color/Health").GetComponent<TextMeshProUGUI>();
        displayAttack.text = "<sprite=0>" + attack.ToString();
        displayArmor.text = "<sprite=1>" + armor.ToString();
        displayHealth.text = "<sprite=2>" + health.ToString();

        if (arrow != 0 && GetCombatTarget() == null)
        {
            arrow = 0;
        }
        //displayArrow = gameObject.transform.Find("Color/Arrow");
        if (arrow != 0)
        {
            displayArrow.gameObject.SetActive(true);
            int side = GetSide() == "PlayerSide" ? 1 : -1;
            displayArrow.localPosition = new Vector3(20 * arrow, 70 * side, 0);
            displayArrow.localEulerAngles = new Vector3(0, 0, arrow * (-90 + 60 * side));
        }
        else { displayArrow.gameObject.SetActive(false); }

        if (disarmed) { displayAttack.color = Color.grey; }
        else { displayAttack.color = Color.white; }

        if (health <= 0)
        {
            DestroyCard();
        }

    }

    public override void OnPlay()
    {
        base.OnPlay();
        displayCardText.transform.parent.gameObject.SetActive(false);
    }

    public virtual void Bounce()
    {
        // Move to hand based on player ownership? or just player side?
        gameObject.transform.SetParent(
            GameObject.Find(hasAuthority ? "PlayerArea" : "EnemyArea").transform,
            false);
        isDraggable = true;
        displayCardText.transform.parent.gameObject.SetActive(true);
    }


    public override void RoundStart() {
        armor = maxArmor;

        CardUpdate();
    }

    public virtual int Strike(Unit target, int damage, bool piercing = false)
    {
        return target.Damage(damage, piercing);
    }

    public virtual int Damage(int damage, bool piercing = false)
    {
        armor -= damage;
        if (armor < 0)
        {
            health += armor;
            armor = 0;
        }
        return health;
    }

    //void Battle

    public virtual void Combat( bool quick = false)
    {
        if (quick == quickstrike && !disarmed)
        {
            Unit target = GetCombatTarget();
            if (target != null)
            {
                int targetHealth = Strike(target, attack + cleave, piercing);
                if ((trample || target.feeble) && targetHealth < 0)
                {
                    bool player = GetSide() == "PlayerSide";
                    TowerManager tower = GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
                    tower.Damage(-1 * targetHealth, piercing);
                }
            }
            else
            {
                bool player = GetSide() == "PlayerSide";
                TowerManager tower = GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
                tower.Damage(attack, piercing);
            }

            if (cleave != 0)
            {
                Unit[] AdjacentEnemies = GetAdjacentEnemies();
                for (int i = -1; i <= 1; i++)
                {
                    if (i == arrow) { continue; }
                    target = AdjacentEnemies[i+1];
                    if (target != null)
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

    public Unit GetCombatTarget()
    {
        int slotNumber = transform.parent.GetSiblingIndex();
        bool player = GetSide() == "PlayerSide";
        Transform targetSlot = GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetChild(slotNumber + arrow);
        Unit target = targetSlot.GetComponentInChildren<Unit>();
        return target;
    }

    public Unit[] GetAdjacentEnemies()
    {
        int numberOfSlots = transform.parent.parent.GetComponentsInChildren<CardSlot>().Length;
        int slotNumber = transform.parent.GetSiblingIndex();
        bool player = GetSide() == "PlayerSide";
        Transform targetSlot;
        Unit target;
        Unit[] AdjEnemies = new Unit[3] ;
        for (int i = -1; i <= 1; i++)
        {
            if (slotNumber + i < 0 || slotNumber + i >= numberOfSlots) {continue;}
            targetSlot = GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetChild(slotNumber + i);
            target = targetSlot.GetComponentInChildren<Unit>();
            AdjEnemies[i + 1] = target;
        }
        return AdjEnemies;
    }

}
