using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OverDrawManager : MonoBehaviour
{
    public GameObject handArea;
    public bool isPlayer;
    public int maxHandSize;
    private int overDrawCount = 0;

    public List<(System.Type, GameEventSystem.EventListener)> events = new List<(System.Type, GameEventSystem.EventListener)>();

    private void Start()
    {
        events.Add(GameEventSystem.Register<GameUpdate_e>(GameUpdate));
    }

    private void OnTransformChildrenChanged()
    {
        if(!(transform.childCount > overDrawCount)) { return; }
        overDrawCount = transform.childCount;

        if (handArea.transform.childCount < maxHandSize)
        {
            MoveToHand();
            return;
        }

        // Do I want to put bounced cards 1st some how? maybe all revealed cards 1st
        foreach (Transform item in transform.Cast<Transform>()
            .Where(card => card.GetComponent<IItem>() != null))
        {
            item.SetAsFirstSibling();
        }
    }

    void GameUpdate(GameUpdate_e e)
    {
        MoveToHand();
    }

    void MoveToHand()
    {
        foreach (Transform card in transform)
        {
            if (handArea.transform.childCount >= maxHandSize) { return; }
            card.transform.rotation = Quaternion.identity;
            card.SetParent(handArea.transform, false);
            card.GetComponent<Card>().revealed |= isPlayer;
        }
    }
}
