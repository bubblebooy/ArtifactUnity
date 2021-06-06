using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardList : MonoBehaviour
{
    public List<GameObject> cards = new List<GameObject>();
    public List<GameObject> heroes = new List<GameObject>();

    public static Dictionary<string, GameObject> cardDict;

    // Start is called before the first frame update
    void Start()
    {
        cardDict = cards.ToDictionary(x => x.name, x => x);
    }
}
