using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System;

public class MySQLRaceStatsDAO : RaceStatsDAO
{
    // Datos de conexión
    [SerializeField] string server = "localhost";
    [SerializeField] string database = "racer";
    [SerializeField] string user = "user"; 
    [SerializeField] string password = ""; 
    private MySqlConnection connection;

    public MySQLRaceStatsDAO()
    {
        string connectionString = $"Server={server};Database={database};User ID={user};Password={password};SslMode=none;";
        connection = new MySqlConnection(connectionString);

        connection.Open();
        Debug.Log("Connected 2 MySQL");
    }

    public RaceStats FindRaceStats(string id)
    {
        RaceStats raceStats = new RaceStats();
        raceStats.gates = new List<GateStat>();

        string query = "SELECT startTime FROM races WHERE id = @id";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", id);

        using (MySqlDataReader reader = cmd.ExecuteReader())
        {
            if (reader.Read())
            {
                raceStats.startTime = reader.GetDateTime("startTime");
            }
            else
            {
                Debug.LogWarning("No se encontró la carrera en MySQL.");
                return raceStats;
            }
        }

        query = "SELECT time, speed FROM gates WHERE raceId = @id ORDER BY gateIndex";
        cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", id);

        using (MySqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                GateStat stat = new GateStat
                {
                    time = reader.GetFloat("time"),
                    speed = reader.GetFloat("speed")
                };

                raceStats.gates.Add(stat);
            }
        }

        return raceStats;
    }

    public string SaveRaceStats(RaceStats s)
    {
        string raceId = Guid.NewGuid().ToString();

        string query = "INSERT INTO races (id, startTime) VALUES (@id, @startTime)";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", raceId);
        cmd.Parameters.AddWithValue("@startTime", s.startTime);
        cmd.ExecuteNonQuery();

        for (int i = 0; i < s.gates.Count; i++)
        {
            GateStat gate = s.gates[i];
            query = "INSERT INTO gates (raceId, gateIndex, time, speed) VALUES (@raceId, @gateIndex, @time, @speed)";
            cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@raceId", raceId);
            cmd.Parameters.AddWithValue("@gateIndex", i);
            cmd.Parameters.AddWithValue("@time", gate.time);
            cmd.Parameters.AddWithValue("@speed", gate.speed);
            cmd.ExecuteNonQuery();
        }

        return raceId;
    }
}
