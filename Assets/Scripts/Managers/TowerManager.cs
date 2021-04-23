using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class TowerManager : NetworkBehaviour
{
    public int health = 40, armor = 0;
    private int maxHealth, maxArmor;
    public bool ancient = false;
    private TextMeshProUGUI  displayHealth; //displayArmor,

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
    }

    public void TowerUpdate()
    {
        displayHealth = gameObject.transform.Find("TowerHealth").GetComponent<TextMeshProUGUI>();
        displayHealth.text = health.ToString();
    }

    public void RoundStart()
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
        
        armor -= damage;
        if (armor < 0)
        {
            health += armor;
            armor = 0;
            Debug.Log(health);
        }
    }
}
