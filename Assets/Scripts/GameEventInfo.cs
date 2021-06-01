using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameEventInfo
{
    
}

public struct RoundStart_e : IGameEventInfo {}

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
    public bool checkAlive { get; }
    public GameUpdate_e(bool check) => checkAlive = check;
}
public struct CardPlayed_e : IGameEventInfo
{
    public GameObject card;
    //public Card card
    //{
    //    get { return _card.GetComponent<Card>(); }
    //    set { _card = value.gameObject; }
    //}
    public GameObject caster;
    //public Unit caster
    //{
    //    get { return _caster.GetComponent<Unit>(); }
    //    set { _card = value.gameObject; }
    //}
    public GameObject lane;
    //public LaneManager lane
    //{
    //    get { return _lane.GetComponent<LaneManager>(); }
    //    set { _card = value.gameObject; }
    //}
}


public struct Auras_e : IGameEventInfo { }
public struct GameUpdateUI_e : IGameEventInfo { }
public struct EndCombatPhase_e : IGameEventInfo { }

//public struct  : IGameEventInfo { }
//public struct  : IGameEventInfo { }
//public struct  : IGameEventInfo { }
//public struct  : IGameEventInfo { }
//public struct  : IGameEventInfo { }
//public struct  : IGameEventInfo { }
//public struct  : IGameEventInfo { }


