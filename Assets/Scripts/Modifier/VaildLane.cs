using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaildLane : MonoBehaviour
{
    public GameObject lane;

    private void Awake()
    {
        GetComponentInParent<Card>().IsVaildEvent += IsVaild;
    }

    public void IsVaild(ref bool valid, GameObject target)
    {
        valid &= lane == target.GetComponentInParent<LaneManager>()?.gameObject;
    }

}
