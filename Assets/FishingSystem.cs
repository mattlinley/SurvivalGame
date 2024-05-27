using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum WaterSource
{
    Lake,
    River,
    Ocean
}

public class FishingSystem : MonoBehaviour
{
    public static FishingSystem Instance { get; set; }

    public List<FishData> lakeFishList;
    public List<FishData> riverFishList;
    public List<FishData> oceanFishList;

    public bool isThereABite;
    public bool hasPulled;

    public static event Action OnFishingEnd;

    public GameObject minigame;

    FishData fishBiting;

    public FishMovement fishMovement;

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

    internal void StartFishing(WaterSource waterSource)
    {
        StartCoroutine(FishingCoroutine(waterSource));
    }

    IEnumerator FishingCoroutine(WaterSource waterSource)
    {
        yield return new WaitForSeconds(3f);

        FishData fish = CalculateBite(waterSource);

        if (fish.fishName == "NoBite")
        {
            Debug.LogWarning("No fish caught");
            EndFishing();
        }
        else
        {
            Debug.LogWarning(fish.fishName + " is biting");
            StartCoroutine(StartFishStruggle(fish));
        }
    }

    IEnumerator StartFishStruggle(FishData fish)
    {
        isThereABite = true;

        //wait until rod pulled
        while (!hasPulled)
        {
            yield return null;
        }

        Debug.LogWarning("StartMinigame");
        fishBiting = fish;
        StartMinigame();

    }

    private void StartMinigame()
    {
        minigame.gameObject.SetActive(true);
        fishMovement.SetDifficulty(fishBiting);
    }

    public void SetHasPulled()
    {
        hasPulled = true;
    }

    private void EndFishing()
    {
        isThereABite = false;
        hasPulled = false;

        fishBiting = null;

        // trigger end fishing event
        OnFishingEnd?.Invoke();


        //reset the fishing rod model
        var slot = EquipSystem.Instance.selectedNumber;

        EquipSystem.Instance.SelectQuickSlot(slot);
        EquipSystem.Instance.SelectQuickSlot(slot);
    }

    private FishData CalculateBite(WaterSource waterSource)
    {
        List<FishData> availableFish = GetAvailableFish(waterSource);

        float totalProbability = 0f;
        foreach (FishData fish in availableFish)
        {
            totalProbability += fish.probablity;
        }

        int randomValue = UnityEngine.Random.Range(0, Mathf.FloorToInt(totalProbability) + 1);
        Debug.Log("Random value is " + randomValue);

        float cumulativeProbability = 0f;
        foreach (FishData fish in availableFish)
        {
            cumulativeProbability += fish.probablity;
            if (randomValue <= cumulativeProbability)
            {
                return fish;
            }
        }

        return null;
    }

    private List<FishData> GetAvailableFish(WaterSource waterSource)
    {
        switch (waterSource)
        {
            case WaterSource.Lake:
                return lakeFishList;
            case WaterSource.River:
                return riverFishList;
            case WaterSource.Ocean: 
                return oceanFishList;
            default:
                return null;
        }
    }

    internal void EndMinigame(bool success)
    {
        minigame.gameObject.SetActive(false);

        if (success)
        {
            Debug.Log("Fish caught");
            InventorySystem.Instance.AddToInventory(fishBiting.fishName);
        }
        else
        {
            Debug.Log("Fish escaped");
        }

        EndFishing();

    }
}
