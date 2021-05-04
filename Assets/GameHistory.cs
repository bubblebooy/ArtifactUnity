using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHistory : MonoBehaviour
{
    [Serializable]
    public struct Record
    {
        public Record(string Action, GameObject GameObject)
        {
            action = Action;
            gameObject = GameObject;
        }

        public string action;
        public GameObject gameObject;
    }

    public List<Record> PlayerHistory = new List<Record>();
    public List<Record> EnemyHistory  = new List<Record>();
    public int randomSeed;

    public void Passed(bool isMyTurn)
    {
        Record record = new Record("Passed", null);
        if (isMyTurn)
        {
            PlayerHistory.Add(record);
        }
        else
        {
            EnemyHistory.Add(record);
        }
    }

    public void CardPlayed(GameObject obj, int? index = null, string side = null)
    {
        if(side == null && index == null)
        {
            Record record = new Record("Card Played", obj);
            if (obj.GetComponent<Card>().hasAuthority)
            {
                PlayerHistory.Add(record);
            }
            else
            {
                EnemyHistory.Add(record);
            }
        }
        else if(side == null)
        {
            GameObject ability = obj.GetComponent<AbilitiesManager>().abilities.transform.GetChild((int)index).gameObject;
            Record record = new Record("Ability Used", ability);
            if (obj.GetComponent<Card>().hasAuthority)
            {
                PlayerHistory.Add(record);
            }
            else
            {
                EnemyHistory.Add(record);
            }
        }
        else
        {
            GameObject towerEnchantment = obj.transform.Find(side + "/Enchantments").GetChild((int)index).gameObject;
            Record record = new Record("Tower Enchantment", towerEnchantment);
            if (side == "PlayerSide")
            {
                PlayerHistory.Add(record);
            }
            else
            {
                EnemyHistory.Add(record);
            }
        }

    }
}
