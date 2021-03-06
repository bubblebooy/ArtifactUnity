using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameEventInfo
{
    
}

public struct RoundStart_e : IGameEventInfo {}
public struct PlayStart_e : IGameEventInfo  {}

public struct Scheme_e : IGameEventInfo
{
    public bool IsMyTrun { get; }
    public Scheme_e(bool turn) => IsMyTrun = turn;
    //{
    //    IsMyTrun = turn;
    //}
}
public struct AfterCombat_e : IGameEventInfo
{
    public LaneManager lane;
}
public struct GameUpdate_e : IGameEventInfo
{
    public string gameState { get; }
    public bool checkAlive { get; }
    public GameUpdate_e(string state, bool check)
    {
        gameState = state;
        checkAlive = check;
    }
}
public struct CardPlayed_e : IGameEventInfo
{
    public GameObject card;
    public GameObject caster;
    public GameObject lane;
}
public struct CardRevealed_e : IGameEventInfo
{
    public CardRevealed_e(bool revealedByOpponent, Card card)
    {
        this.revealedByOpponent = revealedByOpponent;
        this.card = card;
    }
    public bool revealedByOpponent;
    public Card card;
}
public struct TowerDestroyed_e : IGameEventInfo
{
    public TowerManager tower;
    public TowerDestroyed_e(TowerManager tower) => this.tower = tower;
}
public struct UnitKilled_e : IGameEventInfo
{
    public UnitKilled_e(Card card)
    {
        this.card = card;
    }
    public Card card;
}




public struct Auras_e : IGameEventInfo { }
public struct AuraModifiers_e : IGameEventInfo { }
public struct GameUpdateUI_e : IGameEventInfo { }
public struct EndCombatPhase_e : IGameEventInfo { }
public struct DeathEffects_e : IGameEventInfo { }
public struct VariableSlotsUpdate_e : IGameEventInfo { }
public struct NextTurn_e : IGameEventInfo { }
public struct TurnStart_e : IGameEventInfo { }

//public struct  : IGameEventInfo { }
//public struct  : IGameEventInfo { }
//public struct  : IGameEventInfo { }
//public struct  : IGameEventInfo { }
//public struct  : IGameEventInfo { }
//public struct  : IGameEventInfo { }
//public struct  : IGameEventInfo { }


