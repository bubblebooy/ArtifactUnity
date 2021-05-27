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
    public bool caster;
    [HideInInspector]
    public bool piercing = false;
    [HideInInspector]
    public bool trample = false;
    [HideInInspector]
    public bool feeble = false;
    [HideInInspector]
    public bool deathShield = false;
    [HideInInspector]
    public bool damageImmunity = false;
    [HideInInspector]
    public bool untargetable = false;
    [HideInInspector]
    public int siege = 0;
    [HideInInspector]
    public int retaliate = 0;


    protected string targetTag = "Card Slot";

    public List<(System.Type, GameEventSystem.EventListener)> inPlayEvents = new List<(System.Type, GameEventSystem.EventListener)>();

    public override void OnValidate()
    {
        base.OnValidate();
        displayAttack = gameObject.transform.Find("Color/Attack").GetComponent<TextMeshProUGUI>();
        displayArmor = gameObject.transform.Find("Color/Armor").GetComponent<TextMeshProUGUI>();
        displayHealth = gameObject.transform.Find("Color/Health").GetComponent<TextMeshProUGUI>();
        displayAttack.text = "<sprite=0>" + attack.ToString();
        displayArmor.text = "<sprite=1>" + armor.ToString();
        displayHealth.text = "<sprite=2>" + health.ToString();
        caster = this is Hero;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        maxArmor = armor;
        maxHealth = health;
        baseAttack = attack;
        caster = this is Hero;
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
        events.Add(GameEventSystem.Register<GameUpdate_e>(CardUpdate));
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

    public void CardUpdate(GameUpdate_e e)
    {
        CardUpdate();
    }

    public void CardUpdate()
    {
        GetComponent<AbilitiesManager>().CardUpdate();

        attack = baseAttack;
        maxArmor = baseMaxArmor;
        maxHealth = baseMaxHealth;

        cleave = 0;
        siege = 0;
        retaliate = 0;

        quickstrike = false;
        disarmed = false;
        caster = this is Hero;
        piercing = false;
        trample = false;
        feeble = false;
        damageImmunity = false;
        untargetable = false;

        foreach (IModifier mod in gameObject.GetComponentsInChildren<IModifier>())
        {
            mod.ModifyCard();
        }
        if (arrow != 0 && (
            transform.parent.GetComponent<CardSlot>() == null ||
            GetCombatTarget() == null))
        {
            arrow = 0;
        }
    }
    //displayArrow = gameObject.transform.Find("Color/Arrow");
    public void CheckAlive(GameUpdate_e e)
    {
        if (health <= 0)
        {
            if (deathShield)
            {
                deathShield = false;
                health = 1;
            }
            else
            {
                DestroyCard();
            }
            
        }
        
    }

    public override void CardUIUpdate(GameUpdateUI_e e)
    {
        base.CardUIUpdate(e);
        // If this is needed I think we have bigger problems
        //displayAttack = gameObject.transform.Find("Color/Attack").GetComponent<TextMeshProUGUI>();
        //displayArmor = gameObject.transform.Find("Color/Armor").GetComponent<TextMeshProUGUI>();
        //displayHealth = gameObject.transform.Find("Color/Health").GetComponent<TextMeshProUGUI>();
        displayAttack.text = "<sprite=0>" + attack.ToString();
        displayArmor.text = "<sprite=1>" + armor.ToString();
        displayHealth.text = "<sprite=2>" + health.ToString();

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
    }


    public override void OnPlay()
    {
        base.OnPlay();
        inPlayEvents.Add(GameEventSystem.Register<RoundStart_e>(RoundStart));
        inPlayEvents.Add(GameEventSystem.Register<GameUpdate_e>(CheckAlive));
        
        displayCardText.transform.parent.gameObject.SetActive(false);
        GetComponent<AbilitiesManager>().OnPlay();
    }


    public void PlacedOnTopOf(Unit unit)
    {
        GetComponent<AbilitiesManager>().PlacedOnTopOf(unit);
    }

    public virtual void Bounce()
    {
        // Move to hand based on player ownership? or just player side?
        gameObject.transform.SetParent(
            GameObject.Find(hasAuthority ? "PlayerArea" : "EnemyArea").transform,
            false);
        isDraggable = true;
        displayCardText.transform.parent.gameObject.SetActive(true);

        GameEventSystem.Unregister(inPlayEvents);
        GetComponent<AbilitiesManager>().Bounce();

        //foreach( (System.Type t, GameEventSystem.EventListener listener) in inPlayEvents)
        //{
        //    GameEventSystem.Unregister(t, listener);
        //}
        //inPlayEvents.Clear();
    }

    public override void DestroyCard()
    {
        GameEventSystem.Unregister(inPlayEvents);
        GetComponent<AbilitiesManager>().DestroyCard();
        base.DestroyCard();
    }

    public void RoundStart(RoundStart_e e) {
        //GetComponent<AbilitiesManager>().RoundStart();
        armor = maxArmor;
    }

    public virtual int Strike(Unit target, int damage, bool piercing = false)
    {
        Damage(target.retaliate);
        return target.Damage(damage, piercing);
    }

    public virtual void Strike(TowerManager target, int damage, bool piercing = false)
    {
        Damage(target.retaliate);
        target.Damage(damage, piercing);
    }

    public virtual int Damage(int damage, bool piercing = false)
    {
        if (damageImmunity) { return health; }
        if (!piercing)
        {
            armor -= damage;
            if (armor < 0)
            {
                damage = -1 * armor;
                armor = 0;
            }
            else
            {
                damage = 0;
            }
        }
        health -= damage;
        return health;
    }

    //void Battle

    public virtual void Combat( bool quick = false)
    {
        int attackTower = siege;
        if (quick == quickstrike && !disarmed)
        {
            Unit target = GetCombatTarget();
            if (target != null)
            {
                int targetHealth = Strike(target, attack + cleave, piercing);
                if ((trample || target.feeble) && targetHealth < 0)
                {
                    attackTower += -1 * targetHealth;
                    //bool player = GetSide() == "PlayerSide";
                    //TowerManager tower = GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
                    //Strike(tower, -1 * targetHealth, piercing);
                }
            }
            else
            {
                attackTower += attack;
            }
            if(attackTower > 0)
            {
                bool player = GetSide() == "PlayerSide";
                TowerManager tower = GetLane().transform.Find(player ? "EnemySide" : "PlayerSide").GetComponentInChildren<TowerManager>();
                Strike(tower, attackTower, piercing);
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

    public void Purge(bool oppenentEffectsOnly = false, bool triggerAuthority = true, bool temporyEffectsOnly = false)
    {
        GetComponent<AbilitiesManager>().Purge(oppenentEffectsOnly, triggerAuthority, temporyEffectsOnly);
        foreach (UnitModifier mod in gameObject.GetComponents<UnitModifier>())
        {
            //is an oppenentEffect if it is listed as such XOR on a card owned by the oppenent 
            bool opponentEffect = mod.opponentEffect ^ !hasAuthority;  
            if ((!oppenentEffectsOnly || (!triggerAuthority ^ opponentEffect)) &&
                (!temporyEffectsOnly ||  mod.temporary))
            {
                Destroy(mod);
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

    public Unit[] GetNeighbors()
    {
        int numberOfSlots = transform.parent.parent.GetComponentsInChildren<CardSlot>().Length;
        int slotNumber = transform.parent.GetSiblingIndex();
        bool player = GetSide() == "PlayerSide";
        Transform targetSlot;
        Unit target;
        Unit[] Neighbors = new Unit[2];
        for ((int i,int j) = (-1,0); i <= 1; i+=2,j++)
        {
            if (slotNumber + i < 0 || slotNumber + i >= numberOfSlots) { continue; }
            targetSlot = transform.parent.parent.GetChild(slotNumber + i);
            target = targetSlot.GetComponentInChildren<Unit>();
            Neighbors[j] = target;
        }
        return Neighbors;
    }

}
