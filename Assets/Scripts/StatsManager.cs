using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using MongoDB.Driver;
using MongoDB.Bson;
using UnityEngine.UIElements;

public class StatsManager : MonoBehaviour
{
    public enum StorageType
    { 
        mongoDB,
        memory,
        mySQL
    }

    public StorageType storageType;

    RaceStatsDAO raceStatsDAO;

    RaceStats raceStats;

    string id;
    // Start is called before the first frame update
    void Start()
    {
        switch (storageType)
        { 
            case StorageType.mongoDB:
                raceStatsDAO = new MongoRaceStatsDAO();
                break;

            case StorageType.memory:
                raceStatsDAO = new MemoryRaceStatsDAO();
                break;

            case StorageType.mySQL:
                raceStatsDAO = new MySQLRaceStatsDAO();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnRaceStarted()
    {
        // Create race and register start date in database

        raceStats = new RaceStats();

        raceStats.startTime = DateTime.Now;
        raceStats.gates = new List<GateStat>();
    }


    public void OnGatePassed(int index, float time, float speed)
    {

        GateStat stat = new GateStat();
        stat.time = time;
        stat.speed = speed; 

        raceStats.gates.Add(stat);
    }

    public void OnRaceFinished()
    {
        id = raceStatsDAO.SaveRaceStats(raceStats);

    }


    public int GetGateStats(GateStat[] stats)
    {
        RaceStats raceStat = raceStatsDAO.FindRaceStats(id);

        for (int i = 0; i < raceStats.gates.Count; i++)
        {
            stats[i] = raceStats.gates[i];
        }

        return raceStats.gates.Count;
    }


}
