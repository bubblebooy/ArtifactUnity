using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class CardList : MonoBehaviour
{
    public List<GameObject> cards = new List<GameObject>();
    public List<GameObject> heroes = new List<GameObject>();

    public static Dictionary<string, GameObject> cardDict;
    public static Dictionary<string, GameObject> heroesDict;

    // Start is called before the first frame update
    void Start()
    {
        cardDict = cards.ToDictionary(x => x.name, x => x);
        heroesDict = heroes.ToDictionary(x => x.name, x => x);
        NetworkManager networkManager = gameObject.GetComponent<NetworkManager>();
        networkManager.spawnPrefabs.AddRange(heroes);
        networkManager.spawnPrefabs.AddRange(cards);
    }
}
