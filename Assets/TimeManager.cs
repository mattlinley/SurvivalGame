using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; set; }

    public UnityEvent OnDayPass = new UnityEvent();

    public enum Season
    {
        Spring,
        Summer,
        Autumn,
        Winter
    }

    public enum DayOfWeek
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    public Season currentSeason = Season.Spring;
    public DayOfWeek currentDayOfWeek = DayOfWeek.Monday;

    private int daysPerSeason = 5; // 2 for testing
    public int daysInCurrentSeason = 1;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public int dayInGame = 1;
    public int yearInGame = 1;

    public TextMeshProUGUI dayUI;

    private void Start()
    {
        UpdateUI();
    }

    public void TriggerNextDay()
    {
        Debug.Log("Next day");
        dayInGame++;
        daysInCurrentSeason++;

        currentDayOfWeek = (DayOfWeek)(((int)currentDayOfWeek + 1) % 7);

        if (daysInCurrentSeason > daysPerSeason)
        {
            daysInCurrentSeason = 1;
            currentSeason = GetNextSeason();
        }


        UpdateUI();
        OnDayPass.Invoke();
    }

    private Season GetNextSeason()
    {
        int currentSeasonIndex = (int)currentSeason;
        int nextSeasonIndex = (currentSeasonIndex + 1) % 4;
        if (nextSeasonIndex == 0)
        {
            yearInGame++;
        }
        return (Season)nextSeasonIndex;
    }

    public void UpdateUI()
    {
        dayUI.text = currentDayOfWeek + " " + daysInCurrentSeason + " " + currentSeason + ", year " + yearInGame;
    }
}
