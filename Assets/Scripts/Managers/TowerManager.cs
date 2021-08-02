using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class TowerManager : NetworkBehaviour
{
    public int health = 40, armor = 0, ancientHealth = 40;
    public int mana = 3, maxMana = 3;
    public float manaGrowth = 1, growthRemainder;
    public int retaliate = 0;
    public int decay = 0;
    public int regeneration = 0;
    private int maxHealth, maxArmor;
    public bool ancient = false;
    public bool playerTower = false;
    private TextMeshProUGUI  displayHealth; //displayArmor,
    [SerializeField]
    private TextMeshProUGUI displayMana, displayMaxMana;

    public List<(System.Type, GameEventSystem.EventListener)> events = new List<(System.Type, GameEventSystem.EventListener)>();

    public void OnValidate()
    {
        displayHealth = gameObject.transform.Find("TowerHealth").GetComponent<TextMeshProUGUI>();
        displayHealth.text = health.ToString();
    }

    // Start is called before the first frame update
    public override void OnStartClient()
    {
        maxArmor = armor;
        maxHealth = health;
        //displayArmor = gameObject.transform.Find("TowerArmor").GetComponent<TextMeshProUGUI>();
        displayHealth = gameObject.transform.Find("TowerHealth").GetComponent<TextMeshProUGUI>();
        //displayArmor.text = armor.ToString();
        displayHealth.text = health.ToString();

        events.Add(GameEventSystem.Register<RoundStart_e>(RoundStart));
        events.Add(GameEventSystem.Register<GameUpdateUI_e>(TowerUpdate));

        playerTower = transform.parent.name == "PlayerSide";
    }

    public void Initialize(Settings settings)
    {
        mana = (int)settings.values.towerMana;
        growthRemainder = settings.values.towerMana % 1;
        manaGrowth = settings.values.towerManaGrowth;
        maxMana = (int)settings.values.towerMana;
        TowerUpdate(new GameUpdateUI_e());
        health = settings.values.towerHealth;
        ancientHealth = settings.values.ancientHealth;
    }

    public delegate void TowerUpdateDelegate();
    public event TowerUpdateDelegate TowerUpdateEvent;
    public void TowerUpdate(GameUpdateUI_e e)
    {
        displayHealth = gameObject.transform.Find("TowerHealth").GetComponent<TextMeshProUGUI>();
        displayHealth.text = health.ToString();

        displayMana.text = mana.ToString();
        displayMaxMana.text = maxMana.ToString();
        displayMaxMana.enabled = mana != maxMana && maxMana > 0;
        displayMana.enabled = mana != 0 || maxMana > 0;

        decay = 0;
        regeneration = 0;

        TowerUpdateEvent?.Invoke();
    }

    public void RoundStart(RoundStart_e e)
    {
        if( !ancient && health <= 0)
        {
            ancient = true;
            gameObject.GetComponent<Image>().color = new Color(1f, 0.0f, 0.0f, 1.0f);
            gameObject.GetComponentInChildren<TextMeshProUGUI>().color = new Color(1f, 0.1f, 0.0f, 1.0f);
            health = ancientHealth;
        }

        growthRemainder += manaGrowth;
        int _manaGrowth = (int)growthRemainder;
        growthRemainder -= _manaGrowth;
        maxMana += _manaGrowth;
        mana = maxMana;
    }

    public void Retaliate(Unit strikingUnit)
    {
        strikingUnit.Damage(retaliate, piercing: false, physical: true);
    }

    public void Damage(int damage, bool piercing = false, bool physical = false)
    {
        bool destoyed = health <= 0;
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
        if(!destoyed && health <= 0)
        {
            GameEventSystem.Event(new TowerDestroyed_e(this));
        }
        //return health;
    }

    public virtual void Combat(bool quick = false)
    {
        health = Mathf.Min(                     // dont over heal
                health - decay + regeneration,
                Mathf.Max(maxHealth, health));  // if currently has temp health
    }

    public int PayMana(int cost)
    {
        mana -= cost;
        if (mana >= 0)
        {
            return 0;
        }
        else
        {
            cost = -mana;
            mana = 0;
            return cost;
        }
    }

    public void BurnMana(int burn)
    {
        mana = Mathf.Max(0, mana - burn);
    }

    public void RestoreMana(int restore)
    {
        mana = Mathf.Min(mana + restore, maxMana);
    }

    public int CurrentMana()
    {
        return mana;
    }
}
