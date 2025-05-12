using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryRaceStatsDAO : RaceStatsDAO
{
    Dictionary<string, RaceStats> idToRaceStats;
    public MemoryRaceStatsDAO()
    {
        idToRaceStats = new Dictionary<string, RaceStats>();
    }

    public RaceStats FindRaceStats(string id)
    {
        return idToRaceStats[id];
    }

    public string SaveRaceStats(RaceStats s)
    {
        Guid guid = Guid.NewGuid();
        string id = guid.ToString();

        idToRaceStats.Add(id, s);
        return id;
    }
}
