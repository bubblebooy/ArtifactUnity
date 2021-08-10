using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TargetValidation
{
    [TagSelector]
    public string targetTag;
    public bool crossLane;
    public bool targetCardInHand;
    [Header("Unit and Card Slot Options")]
    public bool targetOnlyEnemySide;
    public bool targetOnlyPlayerSide;
    public bool targetCaster;
    public bool targetHero;
    public bool cantTargetSelf;
    public bool cantTargetRooted;
}