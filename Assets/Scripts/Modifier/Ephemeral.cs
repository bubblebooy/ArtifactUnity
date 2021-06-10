using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ephemeral : MonoBehaviour
{
    private Card card;

    public List<(System.Type, GameEventSystem.EventListener)> events = new List<(System.Type, GameEventSystem.EventListener)>();

    private void Awake()
    {
        card = GetComponentInParent<Card>();
        events.Add(GameEventSystem.Register<EndCombatPhase_e>(EndOfRound));
    }

    private void EndOfRound(EndCombatPhase_e e)
    {
        GameEventSystem.Unregister(events);
        if (card.GetComponentInParent<HandManager>() != null)
        { 
            card.DestroyCard();
        }
        Destroy(this);
    }
}
