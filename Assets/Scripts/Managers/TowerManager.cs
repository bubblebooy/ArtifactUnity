using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class TowerManager : NetworkBehaviour
{
    public int health = 40, armor = 0;
    public int retaliate = 0;
    private int maxHealth, maxArmor;
    public bool ancient = false;
    private TextMeshProUGUI  displayHealth; //displayArmor,

    public List<(System.Type, GameEventSystem.EventListener)> events = new List<(System.Type, GameEventSystem.EventListener)>();

    public void OnValidate()
    {
        displayHealth = gameObject.transform.Find("TowerHealth").GetComponent<TextMeshProUGUI>();
        displayHealth.text = health.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        maxArmor = armor;
        maxHealth = health;
        //displayArmor = gameObject.transform.Find("TowerArmor").GetComponent<TextMeshProUGUI>();
        displayHealth = gameObject.transform.Find("TowerHealth").GetComponent<TextMeshProUGUI>();
        //displayArmor.text = armor.ToString();
        displayHealth.text = health.ToString();

        events.Add(GameEventSystem.Register<RoundStart_e>(RoundStart));
        events.Add(GameEventSystem.Register<GameUpdateUI_e>(TowerUpdate));

    }

    public void TowerUpdate(GameUpdateUI_e e)
    {
        displayHealth = gameObject.transform.Find("TowerHealth").GetComponent<TextMeshProUGUI>();
        displayHealth.text = health.ToString();
    }

    public void RoundStart(RoundStart_e e)
    {
        if( !ancient && health <= 0)
        {
            ancient = true;
            gameObject.GetComponent<Image>().color = new Color(0.5f, 0.0f, 0.1f, 1.0f);
            health = 40;
        }
    }

    public void Damage(int damage, bool piercing = false)
    {
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
        //return health;
    }
}
