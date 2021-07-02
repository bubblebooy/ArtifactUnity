using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Unit : Card
{
    public int attack, armor, health;
    protected int baseAttack;
    [HideInInspector]
    public int maxArmor, maxHealth;
    [HideInInspector]
    public int baseMaxArmor, baseMaxHealth;

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
    public bool stun = false;
    public bool caster;
    [HideInInspector]
    public bool silenced = false;
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
    public bool rooted = false;
    [HideInInspector]
    public bool untargetable = false;
    [HideInInspector]
    public int siege = 0;
    [HideInInspector]
    public int retaliate = 0;
    [HideInInspector]
    public int decay = 0;  // prob could make Decay and regen the same variable
    [HideInInspector]
    public int regeneration = 0;
    [HideInInspector]
    public int bounty = 1;

    protected string targetTag = "Card Slot";

    [HideInInspector]
    public bool inPlay = false;

    public List<(System.Type, GameEventSystem.EventListener)> inPlayEvents = new List<(System.Type, GameEventSystem.EventListener)>();

    public override void OnValidate()
    {
        base.OnValidate();
        displayAttack = gameObject.transform.Find("CardFront/Attack").GetComponent<TextMeshProUGUI>();
        displayArmor = gameObject.transform.Find("CardFront/Armor").GetComponent<TextMeshProUGUI>();
        displayHealth = gameObject.transform.Find("CardFront/Health").GetComponent<TextMeshProUGUI>();
        displayAttack.text = "<sprite=0>" + attack.ToString();
        displayArmor.text = "<sprite=1>" + armor.ToString();
        displayHealth.text = "<sprite=2>" + health.ToString();
        caster = this is Hero;
        bounty = this is Hero ? 5 : 1;
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
        displayAttack = gameObject.transform.Find("CardFront/Attack").GetComponent<TextMeshProUGUI>();
        displayArmor = gameObject.transform.Find("CardFront/Armor").GetComponent<TextMeshProUGUI>();
        displayHealth = gameObject.transform.Find("CardFront/Health").GetComponent<TextMeshProUGUI>();
        displayAttack.text = "<sprite=0>" + attack.ToString();
        displayArmor.text = "<sprite=1>" + armor.ToString();
        displayHealth.text = "<sprite=2>" + health.ToString();
        if (disarmed || stun) { displayAttack.color = Color.grey; }
        displayArrow = gameObject.transform.Find("Arrow");
        events.Add(GameEventSystem.Register<GameUpdate_e>(CardUpdate));
    }

    public override void Clone(GameObject originalGameObject)
    {
        base.Clone(originalGameObject);
        Unit originalUnit = originalGameObject.GetComponent<Unit>();
        //do not need to clone properties handled in CardUpdate
        health = originalUnit.health;
        armor = originalUnit.armor;
        //baseMaxHealth = originalUnit.baseMaxHealth;
        //baseMaxArmor = originalUnit.baseMaxHealth;
        inPlay = originalUnit.inPlay;

        //clone modifier scripts
        IModifier[] modifiers = originalGameObject.GetComponents<IModifier>();
        foreach (IModifier modifier in modifiers)
        {
            Component script = gameObject.AddComponent(modifier.GetType());
            (script as IModifier).Clone(modifier);
        }
        //clone abilities
        GetComponent<AbilitiesManager>().Clone(originalGameObject);
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
        decay = 0;
        regeneration = 0;
        bounty = this is Hero ? 5 : 1;

        quickstrike = false;
        disarmed = false;
        stun = false;
        caster = this is Hero;
        silenced = false;
        piercing = false;
        trample = false;
        feeble = false;
        damageImmunity = false;
        rooted = false;
        untargetable = false;

        foreach (IModifier mod in gameObject.GetComponentsInChildren<IModifier>())
        {
            mod.ModifyCard();
        }
        if (arrow != 0 && (
            transform.parent.GetComponent<CardSlot>() is null ||
            GetCombatTarget() is null))
        {
            arrow = 0;
        }
    }
    public void CheckAlive(GameUpdate_e e)
    {
        if (e.checkAlive && health <= 0)
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
        if (revealed)
        {
            displayAttack.text = "<sprite=0>" + attack.ToString();
            displayArmor.text = "<sprite=1>" + armor.ToString();
            displayHealth.text = "<sprite=2>" + health.ToString();

            displayMana.transform.parent.gameObject.SetActive(!(inPlay || this is Hero));

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
    }


    public override void OnPlay()
    {
        base.OnPlay();
        inPlay = true;
        inPlayEvents.Add(GameEventSystem.Register<EndCombatPhase_e>(EndCombatPhase));
        inPlayEvents.Add(GameEventSystem.Register<GameUpdate_e>(CheckAlive));
        
        displayCardText.transform.parent.gameObject.SetActive(false);
        GetComponent<AbilitiesManager>().OnPlay();
    }


    public void PlacedOnTopOf(Unit unit)
    {
        GetComponent<AbilitiesManager>().PlacedOnTopOf(unit);
    }

    public virtual void Bounce(bool ignoreRoot = false)
    {
        // Move to hand based on player ownership? or just player side?
        if (!ignoreRoot && rooted) { return;  }
        gameObject.transform.SetParent(
            GameObject.Find(hasAuthority ? "PlayerOverDraw" : "EnemyOverDraw").transform,
            false);
        isDraggable = true;
        displayCardText.transform.parent.gameObject.SetActive(true);
        arrow = 0;
        inPlay = false;
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
        DestroyCard(true);
    }

    public virtual void DestroyCard(bool killed)
    {
        if (killed) { OnKilled(); }
        GetComponent<AbilitiesManager>().DestroyCard();
        arrow = 0;
        inPlay = false;
        GameEventSystem.Unregister(inPlayEvents);
        base.DestroyCard();
    }

    public void OnKilled()
    {
        //bounty here?
        GetComponent<AbilitiesManager>().OnKilled();
        GameObject.Find(hasAuthority ? "EnemyGold" : "PlayerGold")
            .GetComponent<GoldManager>()
            .gold += bounty;
    }

    protected override void OnDestroy()
    {
        GameEventSystem.Unregister(inPlayEvents);
        //GetComponent<AbilitiesManager>().DestroyCard();
    }

    public void EndCombatPhase(EndCombatPhase_e e) {
        armor = maxArmor;
    }

    public delegate void StrikeUnitDelegate(Unit target, ref int damage, ref bool piercing);
    public event StrikeUnitDelegate StrikeUnitEvent;
    public virtual int Strike(Unit target, int damage, bool piercing = false)
    {
        if (disarmed || stun) { return 0; }
        StrikeUnitEvent?.Invoke(target, ref damage, ref piercing);
        if (target.retaliate > 0)
        {
            Damage(target.retaliate,physical: true);
        }
        return target.Damage(damage, piercing, physical: true);
    }

    public virtual void Strike(TowerManager target, int damage, bool piercing = false)
    {
        if (disarmed || stun) { return; }
        if (target.retaliate > 0)
        {
            Damage(target.retaliate, physical: true);
        }
        target.Damage(damage, piercing, physical: true);
    }

    public virtual int CalculateDamage(int damage, bool piercing = false)
    {
        if (damageImmunity) { return 0; }
        if (!piercing || armor < 0)
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
        return damage;
    }

    public delegate void DamageDelegate(ref int damage, bool piercing, bool physical);
    public event DamageDelegate DamageEvent;
    public virtual int Damage(int damage, bool piercing = false, bool physical = false)
    {
        DamageEvent?.Invoke(ref damage, piercing, physical);
        health -= CalculateDamage(damage, piercing);
        return health;
    }

    public void Heal(int heal)
    {
        if(health < maxHealth)
        {
            health = Mathf.Min(health + heal, maxHealth);
        }
        
    }

    public virtual void Battle(Unit enemy)
    {
        Battle(new Unit[] { enemy });
    }
    public virtual void Battle(Unit[] enemies)
    {
        bool quick = true;
        do
        {
            foreach (Unit enemy in enemies)
            {
                if (enemy == null) { continue; }

                if (quickstrike == quick && !(disarmed || stun)) //Stun
                {
                    Strike(enemy, attack, piercing);
                }
                if (enemy.quickstrike == quick && !(enemy.disarmed || enemy.stun)) //Stun
                {
                    enemy.Strike(this, enemy.attack, enemy.piercing);
                }
            }
            quickstrikeDead();
            foreach (Unit enemy in enemies)
            {
                enemy?.quickstrikeDead();
            }
            quick = !quick;
        } while (!quick);

    }

    int preCombatTargetHealth;
    public virtual void PreCombat(bool quick = false)
    {
        if (quick == quickstrike && !(disarmed || stun)) //Stun
        {
            Unit target = GetCombatTarget();
            if (target != null)
            {
                //factor in decay and regen
                preCombatTargetHealth = target.health - target.decay + target.regeneration;
            }
        }
    }
    public virtual void Combat( bool quick = false)
    {
        int attackTower = siege;
        if (quick == quickstrike && !(disarmed || stun)) //Stun
        {
            Unit target = GetCombatTarget();
            if (target != null)
            {
                int trampleAdj = preCombatTargetHealth - target.health;
                int targetHealth = Strike(target, attack, piercing);
                if (trample || target.feeble && targetHealth < 0)
                {
                    //preCombatTargetHealth
                    attackTower += Mathf.Min((-1 * targetHealth) - trampleAdj);
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
        if (!quick)
        {
            health = Mathf.Min(                // dont over heal
                health - decay + regeneration, 
                Mathf.Max(maxHealth,health));  // if currently has temp health
        }
    }

    public void Purge(bool oppenentEffectsOnly = false, bool triggerAuthority = true, bool temporyEffectsOnly = false)
    {
        GetComponent<AbilitiesManager>().Purge(oppenentEffectsOnly, triggerAuthority, temporyEffectsOnly);
        foreach (UnitModifier mod in gameObject.GetComponents<UnitModifier>())
        {
            //is an oppenentEffect, if it is listed as such XOR on a card owned by the oppenent 
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

    public CardSlot GetCardSlot()
    {
        return transform.parent.GetComponent<CardSlot>(); ;
    }
    public string GetSide()
    {
        CardSlot cardSlot = GetCardSlot();
        if(cardSlot == null)
        {
            Debug.LogWarning("GetSide() on Unit not in a cardSlot", gameObject);
            return null;
        }
        return cardSlot.GetSide();
    }

    public TowerManager GetTower()
    {
        CardSlot cardSlot = GetCardSlot();
        return cardSlot.GetTower();
    }

    public LaneManager GetLane()
    {
        CardSlot cardSlot = GetCardSlot();
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
