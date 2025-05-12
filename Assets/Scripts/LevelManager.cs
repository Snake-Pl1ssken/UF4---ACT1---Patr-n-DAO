using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public Transform statsManager;

    public float countdownTime = 3;

    public int gateCount = 5;
    public float gateExtraTime = 20;

    public Transform uiCountdown;
    public Transform uiTimer;
    public Transform uiTimeLimit;
    public Transform uiStats;
    public Transform uiOverlayCountdown;
    public Transform uiOverlayRace;
    public Transform uiOverlayFinished;


    int nextGate;

    public enum State
    {
        countdown,
        racing,
        finished
    };

    State state;
    State nextState;

    float timeLimit;

    float timer;


    StatsManager statsManagerC;

    TextMesh uiTimerC;
    TextMesh uiTimeLimitC;
    TextMesh uiStatsC;
    TextMesh uiCountdownC;

    StatsManager.Stat[] stats;



    // Start is called before the first frame update
    void Start()
    {
        statsManagerC = statsManager.GetComponent<StatsManager>();
        uiCountdownC = uiCountdown.GetComponent<TextMesh>(); ;
        uiTimerC = uiTimer.GetComponent<TextMesh>();
        uiTimeLimitC = uiTimeLimit.GetComponent<TextMesh>();
        uiStatsC = uiStats.GetComponent<TextMesh>();

        uiCountdown.gameObject.SetActive(true);
        uiTimer.gameObject.SetActive(false);
        uiTimeLimit.gameObject.SetActive(false);
        uiStats.gameObject.SetActive(false);
        uiOverlayCountdown.gameObject.SetActive(true);
        uiOverlayRace.gameObject.SetActive(false);
        uiOverlayFinished.gameObject.SetActive(false);


        state = State.countdown;
        nextState = State.countdown;

        stats = new StatsManager.Stat[gateCount];


        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (state == State.countdown)
        {
            timer += Time.deltaTime;

            uiCountdownC.text = String.Format("{0:0}", countdownTime - timer);

            if(timer >= countdownTime) { nextState = State.racing; }
        }
        else if(state == State.racing)
        {
            timer += Time.deltaTime;
            timeLimit -= Time.deltaTime;

            uiTimeLimitC.text = String.Format("{0:0}", timeLimit);
            uiTimerC.text = String.Format("Total time: {0:0.00}", timer);


            if (timeLimit <= 0) { nextState = State.finished; }
        }

        if(state != nextState)
        {
            if(nextState == State.racing)
            {
                uiCountdown.gameObject.SetActive(false);
                uiTimer.gameObject.SetActive(true);
                uiTimeLimit.gameObject.SetActive(true);
                uiStats.gameObject.SetActive(false);
                uiOverlayCountdown.gameObject.SetActive(false);
                uiOverlayRace.gameObject.SetActive(true);
                uiOverlayFinished.gameObject.SetActive(false);


                statsManagerC.OnRaceStarted();
                timeLimit = gateExtraTime;
                timer = 0;
            }
            else if(nextState == State.finished)
            {
                uiCountdown.gameObject.SetActive(false);
                uiTimer.gameObject.SetActive(false);
                uiTimeLimit.gameObject.SetActive(false);
                uiStats.gameObject.SetActive(true);
                uiOverlayCountdown.gameObject.SetActive(false);
                uiOverlayRace.gameObject.SetActive(false);
                uiOverlayFinished.gameObject.SetActive(true);


                statsManagerC.OnRaceFinished();

                int count = statsManagerC.GetStats(stats);

                uiStatsC.text = "";

                for(int i = 0; i < count; i++)
                {
                    uiStatsC.text += String.Format("Gate {0:0}: Time {1:0.00} Speed {2:0.00}\n", i + 1, stats[i].time, stats[i].speed);
                }

            }

            state = nextState;
        }

    }

    public void OnGatePassed(int index, float speed)
    {
        if(state == State.racing && index == nextGate)
        {
            statsManagerC.OnGatePassed(index, timer, speed);
            timeLimit = gateExtraTime;

            nextGate++;

            if(nextGate >= gateCount)
            {
                nextState = State.finished;
            }
        }
    }

    public bool IsRacing()
    {
        return (state == State.racing);
    }
}
