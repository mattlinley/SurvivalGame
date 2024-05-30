using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class CampfireUIManager : MonoBehaviour
{

    public static CampfireUIManager Instance { get; set; }

    public Button cookButton;
    public Button exitButton;

    public GameObject foodSlot;
    public GameObject fuelSlot;

    public GameObject campfirePanel;
    public bool isUIOpen;

    public Campfire selectedCampfire;

    public CookingData cookingData;


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




    public void OpenUI()
    {
        campfirePanel.SetActive(true);
        isUIOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SelectionManager.Instance.DisableSelection();
        SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;

        var fuelSlot = campfirePanel.transform.Find("Fuel_Slot");
        var foodSlot = campfirePanel.transform.Find("Food_Slot");

        //load any items in slots
        if (selectedCampfire.itemInFuelSlot != "")
        {
            var itemToAdd = Instantiate(Resources.Load<GameObject>(selectedCampfire.itemInFuelSlot), fuelSlot.transform.position, fuelSlot.transform.rotation);

            itemToAdd.name = selectedCampfire.itemInFuelSlot;

            itemToAdd.transform.SetParent(fuelSlot.transform);
        }
        if (selectedCampfire.itemInFoodSlot != "")
        {
            var itemToAdd = Instantiate(Resources.Load<GameObject>(selectedCampfire.itemInFoodSlot), foodSlot.transform.position, fuelSlot.transform.rotation);

            itemToAdd.name = selectedCampfire.itemInFoodSlot;

            itemToAdd.transform.SetParent(foodSlot.transform);
        }

        InventorySystem.Instance.OpenUI();
    }

    public void CloseUI()
    {
        var tempSlot = campfirePanel.transform.Find("Fuel_Slot");
        if (tempSlot.childCount > 0)
        {
            selectedCampfire.itemInFuelSlot = tempSlot.GetChild(0).name;
            Destroy(tempSlot.GetChild(0).gameObject);
        } 
        else
        {
            selectedCampfire.itemInFuelSlot = "";
        }
        

        tempSlot = campfirePanel.transform.Find("Food_Slot");
        if (tempSlot.childCount > 0)
        {
            selectedCampfire.itemInFoodSlot = tempSlot.GetChild(0).name;
            Destroy(tempSlot.GetChild(0).gameObject);
        }
        else
        {
            selectedCampfire.itemInFoodSlot = "";
        }
        

        campfirePanel?.SetActive(false);
        isUIOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SelectionManager.Instance.EnableSelection();
        SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (FuelAndFoodAreValid())
        {
            cookButton.interactable = true;
        }
        else
        {
            cookButton.interactable= false;
        }
    }

    private bool FuelAndFoodAreValid()
    {
        InventoryItem fuel = fuelSlot.GetComponentInChildren<InventoryItem>();
        InventoryItem food = foodSlot.GetComponentInChildren<InventoryItem>();


        if (fuel != null && food != null)
        {
            if (cookingData.validFuels.Contains(fuel.thisName) && cookingData.validFoods.Any(cookableFood => cookableFood.name == food.thisName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public void CookButtonPressed()
    {
        InventoryItem food = foodSlot.GetComponentInChildren<InventoryItem>();

        selectedCampfire.StartCooking(food);

        InventoryItem fuel = fuelSlot.GetComponentInChildren<InventoryItem>();

        Destroy(food.gameObject);
        Destroy(fuel.gameObject);

        CloseUI();

    }

    public void closeFire ()
    {
        
    }
}
