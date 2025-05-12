using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Policy;
using System.Xml.Linq;
using System.Linq;

public class StatsManager : MonoBehaviour
{    

    public struct Stat
    {
        public double time;
        public double speed;

    };
    public Mesh arrowMesh;
    public Material arrowMaterial;
    public string serverAddress = "mongodb://localhost:27017";
    public string database = "racer";
    public string collection = "races";

    Dictionary<int, Stat> gateToStat;
    BsonElement e;
    BsonElement e2;
    MongoClient connection;
    BsonDocument doc;
    BsonArray gatesArray;
    static IMongoDatabase databaseMongo;
    static IMongoCollection<BsonDocument> collectionMongo;
    ObjectId idObjectId;

    // Start is called before the first frame update
    void Start()
    {
        gateToStat = new Dictionary<int, Stat>();
        

        // Connect to database

        connection = new MongoClient(serverAddress);
        databaseMongo = connection.GetDatabase(database);
        collectionMongo = databaseMongo.GetCollection<BsonDocument>(collection);

        if (connection != null)
        {
            Debug.Log("Database conected");
        }
    }

    // Update is called once per frame
    void Update()
    {
        float angle = 0;
        Vector3 position = new Vector3(0, 1f, 0);

        for (int i = 0; i < 100; i++)
        {
            position += Vector3.forward * 1f;
            angle += 1f;

            Color c = arrowMaterial.color;

            arrowMaterial.color = new Color(c.r, c.g, c.b, 1.0f - i / 100.0f);

            Graphics.DrawMesh(arrowMesh, position, Quaternion.Euler(0, angle, 0) * Quaternion.Euler(-90, 0 , 180), arrowMaterial, 0);
        }
    }

    public void OnRaceStarted()
    {
        // Create race and register start date in database
        doc = new BsonDocument();
        e2 = new BsonElement("StartTime", new BsonDateTime(DateTime.Now));
        //doc.Add(e);
        //collectionMongo.InsertOne(doc);
        gatesArray = new BsonArray();
    }

    public void OnGatePassed(int index, float time, float speed)
    {
        Debug.Log("Registering gate " + index);

        Stat s = new Stat();
        s.time = time;
        s.speed = speed;

       gateToStat[index] = s;

        // Register gate in database

        doc = new BsonDocument();
        e = new BsonElement("time", s.time);
        doc.Add(e);
        e = new BsonElement("speed", s.speed);
        doc.Add(e);
        //collectionMongo.InsertOne(doc);
        gatesArray.Add(doc);


    }

    public void OnRaceFinished()
    {

        string nombre = "Race";
       
        Debug.Log("Race finished");
        doc = new BsonDocument();
        e = e2;
        doc.Add(e);
        doc.Add(nombre, gatesArray);
        collectionMongo.InsertOne(doc);

        idObjectId = doc["_id"].AsObjectId; //BsonArray.AsBsonArray
        //List<Stat> lst = collectionMongo.Find(d => true).ToArray();
        //doc.FindOne(idObjectId);
        //la bd a añadido un id (aqui guardar el id y luego recuperarlo)
        //var id = doc.{ collectionMongo}
        //idString = id;
        doc.Clear();

        Stat stats = new Stat();

    }


    public int GetStats(Stat[] stats)
    {
        int count = 0;
        // Get race stats from database
        Debug.Log("{ _id: '" + idObjectId + "'}");
        List<BsonDocument> list = collectionMongo.Find<BsonDocument>("{ _id: ObjectId('" + idObjectId + "')}").ToList<BsonDocument>();

        //sacar del list el documento con carrera 
        //debtro de documento hacer loop que pase por todas las gates
        //en cada vuelta crear y poner stat en la posicion de outstat correspondiente
        //Retornar num stats añadidos
        BsonDocument doc = list.First();
        BsonArray racesInDoc = doc["Race"].AsBsonArray;
        //Debug.Log(racesInDoc["time"].AsDouble);
        //Debug.Log(racesInDoc["speed"].AsDouble);
        int ind = 0;
        foreach (BsonDocument r in racesInDoc)
        {
            stats[ind].time = r["time"].AsDouble;
            stats[ind].speed = r["speed"].AsDouble;
            ind++;
            count++;
        }
        return count; 
    }

    public void ClearStats()
    {
        gateToStat.Clear();
    }
}
