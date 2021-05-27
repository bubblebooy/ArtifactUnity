using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameEventInfo
{
    
}

public struct RoundStart_e : IGameEventInfo
{

}

public struct Scheme_e : IGameEventInfo
{
    public bool IsMyTrun { get; }

    public Scheme_e(bool turn) => IsMyTrun = turn;
    //{
    //    IsMyTrun = turn;
    //}
}

