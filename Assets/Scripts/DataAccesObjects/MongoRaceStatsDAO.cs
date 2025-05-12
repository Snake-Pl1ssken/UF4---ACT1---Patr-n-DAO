using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using UnityEngine;

public class MongoRaceStatsDAO : RaceStatsDAO
{
    public string serverAddress = "mongodb://localhost:27017";
    public string database = "racer";
    public string collection = "races";

    MongoClient connection;

    IMongoDatabase _database;
    IMongoCollection<BsonDocument> _collection;

    BsonDocument document;

    public MongoRaceStatsDAO()
    {
        // Connect to database

        connection = new MongoClient(serverAddress);

        _database = connection.GetDatabase(database);
        _collection = _database.GetCollection<BsonDocument>(collection);

    }

    public RaceStats FindRaceStats(string id)
    {
        string query = "{_id: ObjectId('" + id + "') }";
        Debug.Log(query);
        IFindFluent<BsonDocument, BsonDocument> result = _collection.Find<BsonDocument>(query);

        RaceStats raceStats = new RaceStats();
        raceStats.gates = new List<GateStat>();
        if (result.CountDocuments() == 1)
        {
            BsonDocument document = result.First<BsonDocument>();
            raceStats.startTime = (DateTime)document["startTime"].AsBsonDateTime;
            foreach (BsonDocument gate in document["gates"].AsBsonArray)
            {
                int index = gate["gate"].AsInt32;

                GateStat stat = new GateStat();
                stat.time = (float)gate["time"].AsDouble;
                stat.speed = (float)gate["speed"].AsDouble;

                raceStats.gates.Add(stat);
            }
        }
        return raceStats;
    }

    public string SaveRaceStats(RaceStats s)
    {
        BsonDocument document = new BsonDocument();

        document.Add("startTime", new BsonDateTime(s.startTime));
        

        BsonArray gatesArray = new BsonArray();

        for (int i = 0; i < s.gates.Count; i++)
        {
            BsonDocument stat = new BsonDocument();
            stat.Add("gate", i);
            stat.Add("time", s.gates[i].time);
            stat.Add("speed", s.gates[i].speed);

            gatesArray.Add(stat);   
            
        }
        document.Add("gates", gatesArray);
        //Debug.Log("Registering stats of gate " + index);

        // Register gate in database


        //document.GetElement("gates").Value.AsBsonArray.Add(stat);

        Debug.Log("Race finished");

        _collection.InsertOne(document);
        string id = document["_id"].AsObjectId.ToString();

        return id;
    }
}
