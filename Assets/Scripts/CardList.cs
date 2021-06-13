using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class CardList : MonoBehaviour
{
    public List<GameObject> cards = new List<GameObject>();
    public List<GameObject> hero = new List<GameObject>();
    public List<GameObject> items = new List<GameObject>();

    public static Dictionary<string, GameObject> cardDict;
    public static Dictionary<string, GameObject> heroDict;
    public static Dictionary<string, GameObject> itemDict;

    // Start is called before the first frame update
    void Start()
    {
        cardDict = cards.ToDictionary(x => x.name, x => x);
        heroDict = hero.ToDictionary(x => x.name, x => x);
        itemDict = items.ToDictionary(x => x.name, x => x);
        NetworkManager networkManager = gameObject.GetComponent<NetworkManager>();
        networkManager.spawnPrefabs.AddRange(hero);
        networkManager.spawnPrefabs.AddRange(cards);
        networkManager.spawnPrefabs.AddRange(items);
    }
}
