using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    public static WeatherSystem Instance { get; set; }

    [Range(0f, 1f)]
    public float chanceToRainSpring = 0.3f;
    [Range(0f, 1f)]
    public float chanceToRainSummer = 0f;
    [Range(0f, 1f)]
    public float chanceToRainAutumn = 0.4f;
    [Range(0f, 1f)]
    public float chanceToRainWinter = 0.7f;

    public GameObject rainEffect;
    public Material rainSkyBox;

    public bool isSpecialWeather;

    public AudioSource rainChannel;
    public AudioClip rainSound;

    public enum WeatherCondition
    {
        Sunny,
        Rainy
    }

    public WeatherCondition currentWeather = WeatherCondition.Sunny;


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


    // Start is called before the first frame update
    void Start()
    {
        TimeManager.Instance.OnDayPass.AddListener(GenerateRandomWeather);
    }

    private void GenerateRandomWeather()
    {
        TimeManager.Season currentSeason = TimeManager.Instance.currentSeason;

        float chanceToRain = 0f;

        switch (currentSeason)
        {
            case TimeManager.Season.Spring:
                chanceToRain = chanceToRainSpring;
                break;
            case TimeManager.Season.Summer:
                chanceToRain = chanceToRainSummer;
                break;
            case TimeManager.Season.Autumn:
                chanceToRain = chanceToRainAutumn;
                break;
            case TimeManager.Season.Winter:
                chanceToRain = chanceToRainWinter;
                break;
            default:
                break;
        }

        //generate the random number
        if (UnityEngine.Random.value <= chanceToRain)
        {
            currentWeather = WeatherCondition.Rainy;
            isSpecialWeather = true;

            

            Invoke("StartRain", 1f);
            
        }
        else
        {
            currentWeather = WeatherCondition.Sunny;
            isSpecialWeather = false;

            StopRain();
        }
    }

    public void StopRain()
    {
        if (rainChannel.isPlaying)
        {
            rainChannel.Stop();
        }

        rainEffect.SetActive(false);
    }

    public void StartRain()
    {
        if (rainChannel.isPlaying == false)
        {
            rainChannel.clip = rainSound;
            rainChannel.Play();
        }
        

        RenderSettings.skybox = rainSkyBox;
        rainEffect.SetActive(true);
    }
}
