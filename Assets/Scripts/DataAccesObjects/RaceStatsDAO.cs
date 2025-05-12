using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public struct GateStat
{
    public float time;
    public float speed;

};

public struct RaceStats
{
    public DateTime startTime;
    public List<GateStat> gates;
}

public interface RaceStatsDAO
{
    public string SaveRaceStats(RaceStats s);

    public RaceStats FindRaceStats(string id);
}
