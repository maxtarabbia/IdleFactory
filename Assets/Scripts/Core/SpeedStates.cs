using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct SpeedStates
{
    public speedinfo MinerInfo;
    public speedinfo RefineryInfo;
    public speedinfo BeltInfo;
}
[Serializable]
public struct speedinfo
{
    public float speed;
    public long cost;
    public float speedScale;
    public double costScale;
}
