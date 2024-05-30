using System.Collections.Generic;
using UnityEngine;
using static TimeManager;
using static WeatherSystem;

[System.Serializable]
public class EnvironmentData
{
    public List<string> pickedupItems;

    public List<TreeData> treeData;

    public List<string> animals;

    public List<StorageData> storage;

    public List<CampfireData> campfires;

    public TimeData timeData;

    public List<NpcData> npcData;

    public List<Quest> trackedQuests;

    public EnvironmentData(List<string> _pickedupItems,
                           List<TreeData> _treeData,
                           List<string> _animals,
                           List<StorageData> _storage,
                           TimeData _timeData,
                           List<NpcData> _npcData,
                           List<Quest> _trackedQuests,
                           List<CampfireData> _campfires)
    {
        pickedupItems = _pickedupItems;
        treeData = _treeData;
        animals = _animals;
        storage = _storage;
        timeData = _timeData;
        npcData = _npcData;
        trackedQuests = _trackedQuests;
        campfires = _campfires;
    }

}

[System.Serializable]
public class TreeData
{
    public string name;
    public Vector3 position;
    public Vector3 rotation;
    public float treeHealth;
    public int dayOfRegrowth;
}

[System.Serializable]
public class StorageData
{
    public List<string> items;
    public Vector3 position;
    public Vector3 rotation;
}

[System.Serializable]
public class CampfireData
{
    public string fuel;
    public string food;
    public Vector3 position;
    public Vector3 rotation;
}


[System.Serializable]
public class TimeData
{
    public Season currentSeason;
    public DayOfWeek currentDayOfWeek;
    public int daysInCurrentSeason;
    public int dayInGame;
    public int yearInGame;
    public float currentTimeOfDay;
    public WeatherCondition currentWeather;
}

[System.Serializable]
public class NpcData
{
    public string name;
    public bool firstTimeInteraction;
    public int currentDialog;
    public int activeQuestIndex;
    public List<QuestData> questData;
}

[System.Serializable]
public class QuestData
{
    public bool initialDialogCompleted;
    public bool accepted;
    public bool declined;
    public bool isCompleted;
}