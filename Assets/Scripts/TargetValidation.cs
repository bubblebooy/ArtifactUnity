using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TargetValidation
{
    [TagSelector]
    public string targetTag;
    public bool crossLane;
    [Header("Unit and Card Slot Options")]
    public bool targetOnlyEnemySide;
    public bool targetOnlyPlayerSide;
    public bool targetCaster;
    public bool targetHero;
}