using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class EquipSystem : MonoBehaviour
{
    public static EquipSystem Instance { get; set; }

    // -- UI -- //
    public GameObject quickSlotsPanel;

    public List<GameObject> quickSlotsList = new List<GameObject>();

    public GameObject numbersHolder;

    public int selectedNumber = -1;
    public GameObject selectedItem;

    public GameObject toolHolder;
    public GameObject selectedItemModel;


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


    private void Start()
    {
        PopulateSlotList();
    }

    private void PopulateSlotList()
    {
        foreach (Transform child in quickSlotsPanel.transform)
        {
            if (child.CompareTag("QuickSlot"))
            {
                quickSlotsList.Add(child.gameObject);
            }
        }
    }

    public void AddToQuickSlots(GameObject itemToEquip)
    {
        // Find next free slot
        GameObject availableSlot = FindNextEmptySlot();
        // Set transform of our object
        itemToEquip.transform.SetParent(availableSlot.transform, false);
        // Getting clean name
        string cleanName = itemToEquip.name.Replace("(Clone)", "");

        InventorySystem.Instance.RecalculateList();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectQuickSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectQuickSlot(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectQuickSlot(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectQuickSlot(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectQuickSlot(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SelectQuickSlot(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SelectQuickSlot(7);
        }
    }

    public void SelectQuickSlot(int slotNumber)
    {
        if (checkIfSlotIsFull(slotNumber) == true)
        {
            if (selectedNumber != slotNumber) {
                selectedNumber = slotNumber;
                if (selectedItem != null)
                {
                    selectedItem.GetComponent<InventoryItem>().isSelected = false;
                }

                selectedItem = GetSelectedItem(slotNumber);
                selectedItem.GetComponent<InventoryItem>().isSelected = true;

                SetEquippedModel(selectedItem);

                //changing color
                foreach (Transform number in numbersHolder.transform)
                {
                    number.GetChild(0).transform.GetComponent<Text>().color = new Color32(178, 164, 164, 255);
                }
                numbersHolder.transform.GetChild(slotNumber - 1).transform.GetChild(0).transform.GetComponent<Text>().color = Color.yellow;
            }
            else
            {
                //trying to select same slot, so unselect
                selectedNumber = -1;

                if (selectedItem != null)
                {
                    selectedItem.GetComponent<InventoryItem>().isSelected = false;
                    selectedItem = null;
                }
                if (selectedItemModel != null)
                {
                    DestroyImmediate(selectedItemModel);
                    selectedItemModel = null;
                }
                numbersHolder.transform.GetChild(slotNumber - 1).transform.GetChild(0).transform.GetComponent<Text>().color = new Color32(178, 164, 164, 255);
            }
        }
    }

    private void SetEquippedModel(GameObject selectedItem)
    {
        if (selectedItemModel != null)
        {
            DestroyImmediate(selectedItemModel);
            selectedItemModel = null;
        }

        string selectedItemName = selectedItem.name.Replace("(Clone)","");
        //selectedItemModel = Instantiate(Resources.Load<GameObject>(selectedItemName + "_Model"),
        //                                   new Vector3(0.42f, 0.5f, 0.78f),
        //                                   Quaternion.Euler(-0.17f, 2.31f, -18.15f));

        selectedItemModel = Instantiate(Resources.Load<GameObject>(CalculateItemModel(selectedItemName)));
        selectedItemModel.transform.SetParent(toolHolder.transform,
                                      false);
        if (selectedItemName == "WateringCan" || selectedItemName == "Axe")
        {
            //disable box collider for gravity enabled items (ones that you can drop and pick up)
            selectedItemModel.transform.GetComponent<BoxCollider>().enabled = false;
        }
        
    }

    private string CalculateItemModel(string selectedItemName)
    {
        switch (selectedItemName)
        {
            case "Axe":
                return "Axe_Model";
            case "TomatoSeed":
                return "Hand_Model";
            case "PumpkinSeed":
                return "Hand_Model";
            case "WateringCan":
                return "WateringCan_Model";
            case "FishingRod":
                return "FishingRod_Model";
            default:
                return null;
        }
    }

    private GameObject GetSelectedItem(int slotNumber)
    {
        return quickSlotsList[slotNumber - 1].transform.GetChild(0).gameObject;
    }

    private bool checkIfSlotIsFull(int slotNumber)
    {
        if (quickSlotsList[slotNumber-1].transform.childCount >0)
        {
            return true;
        }

        return false;

    }

    public GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return new GameObject();
    }

    public bool CheckIfFull()
    {

        int counter = 0;

        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                counter += 1;
            }
        }

        if (counter == 7)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetWeaponDamage()
    {
        if (selectedItem != null)
        {
            return selectedItem.GetComponent<Weapon>().WeaponDamage;
        }
        else
        {
            return 0;
        }
    }

    public bool IsHoldingSeed()
    {
        if (selectedItemModel != null)
        {
            switch (selectedItemModel.gameObject.name)
            {
                case "Hand_Model(Clone)":
                    return true;
                case "Hand_Model":
                    return true;
                default:
                    return false;
            }
        }
        else
        {
            return false;
        }
    }

    public bool IsHoldingWeapon()
    {
        if (selectedItem != null)
        {
            if (selectedItem.GetComponent<Weapon>() != null)
            {
                return true;
            }
            else
            {
                return false;

            }
        }
        else
        {
            return false;
        }
    }

    public bool IsThereASwingLock()
    {
        if (selectedItemModel && selectedItemModel.GetComponent<EquipableItem>())
        {
            return selectedItemModel.GetComponent<EquipableItem>().swingWait;
        }
        else
        {
            return false;
        }
    }

    internal bool IsPlayerHoldingWateringCan()
    {
        if (selectedItem != null)
        {
            
            switch (selectedItem.GetComponent<InventoryItem>().thisName)
            {
                case "Watering Can(Clone)":
                    return true;
                case "Watering Can":
                    return true;
                default:
                    return false;
            }
        }
        else
        {
            return false;
        }
    }
}
