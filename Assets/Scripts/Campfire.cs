using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    public bool playerInRange;

    public bool isCooking;
    public float cookingTimer;

    public CookableFood foodBeingCooked;
    public string readyFood;

    public GameObject fire;

    public string itemInFuelSlot;
    public string itemInFoodSlot;

    private void Update()
    {
        float distance = Vector3.Distance(PlayerState.Instance.playerBody.transform.position, transform.position);

        if (distance < 10f)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }

        if (isCooking)
        {
            cookingTimer -= Time.deltaTime;
            fire.SetActive(true);
        }

        if (cookingTimer <= 0 && isCooking)
        {
            isCooking = false;
            fire.SetActive(false);
            readyFood = GetCookedFood(foodBeingCooked);
        }
    }

    private string GetCookedFood(CookableFood food)
    {
        return food.cookedFoodName;
    }

    public void OpenUI()
    {
        
        

        if (readyFood != "")
        {
            //GameObject rf = Instantiate(Resources.Load<GameObject>(readyFood),
            //                            CampfireUIManager.Instance.foodSlot.transform.position,
            //                            CampfireUIManager.Instance.foodSlot.transform.rotation);
            //rf.transform.SetParent(CampfireUIManager.Instance.foodSlot.transform);

            itemInFoodSlot = readyFood;
            itemInFuelSlot = "";

            readyFood = "";
        }

        CampfireUIManager.Instance.selectedCampfire = this;
        CampfireUIManager.Instance.OpenUI();
    }

    public void StartCooking(InventoryItem food)
    {
        foodBeingCooked = ConvertIntoCookable(food);
        isCooking = true;
        cookingTimer = TimeToCookFood(foodBeingCooked);
    }

    private CookableFood ConvertIntoCookable(InventoryItem food)
    {
        foreach (CookableFood cookable in CampfireUIManager.Instance.cookingData.validFoods)
        {
            if (cookable.name == food.thisName)
            {
                return cookable;
            }
        }
        return new CookableFood();
    }

    private float TimeToCookFood(CookableFood food)
    {
        return food.timeToCook;
    }
}
