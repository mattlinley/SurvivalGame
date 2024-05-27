using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    public static DayNightSystem Instance { get; set; }

    public Light directionalLight;

    public float dayDurationInSeconds = 24.0f;
    public int currentHour;
    public float currentTimeOfDay = 0.35f;

    public List<SkyboxTimeMapping> timeMappings;

    float blendedValue = 0.0f;

    bool lockNextDayTrigger = false;

    public TextMeshProUGUI timeUI;

    public WeatherSystem weatherSystem;

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


    // Update is called once per frame
    void Update()
    {
        currentTimeOfDay += Time.deltaTime / dayDurationInSeconds;
        currentTimeOfDay %= 1;

        currentHour = Mathf.FloorToInt(currentTimeOfDay * 24);

        timeUI.text = currentHour.ToString() + ":00";

        directionalLight.transform.rotation = Quaternion.Euler(new Vector3((currentTimeOfDay * 360) - 90, 170, 0));

        //only update if not raining
        if (!weatherSystem.isSpecialWeather)
        {
            UpdateSkybox();
        }

        if (currentHour == 0 && !lockNextDayTrigger)
        {
            TimeManager.Instance.TriggerNextDay();
            lockNextDayTrigger = true;
        }

        if (currentHour > 0)
        {
            lockNextDayTrigger = false;
        }

    }

    private void UpdateSkybox()
    {
        Material currentSkybox = null;

        foreach (SkyboxTimeMapping mapping in timeMappings)
        {
            if (currentHour == mapping.hour)
            {

                currentSkybox = mapping.skyboxMaterial;

                if (currentSkybox.shader != null)
                {
                    if (currentSkybox.shader.name == "Custom/SkyboxTransition")
                    {
                        blendedValue += Time.deltaTime;
                        blendedValue = Mathf.Clamp01(blendedValue);

                        currentSkybox.SetFloat("_TransitionFactor", blendedValue);
                    }
                    else
                    {
                        blendedValue = 0;
                    }
                }

                break;
            }
        }

        

        if (currentSkybox != null)
        {
            RenderSettings.skybox = currentSkybox;
        }
    }
}

[System.Serializable]
public class SkyboxTimeMapping
{
    public string phaseName;
    public int hour;  //hour of the day
    public Material skyboxMaterial; //material for corresponding hour
}