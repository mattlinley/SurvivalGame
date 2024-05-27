using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }

    public GameObject inventoryScreenUI;
    public GameObject itemInfoUI;

    public List<GameObject> slotList = new List<GameObject>();
    public List<string> itemList = new List<string>();
    private GameObject itemToAdd;
    private GameObject whatSlotToEquip;

    public bool isOpen;
    //public bool isFull;

    // Pickup Pop UP
    public GameObject pickupAlert;
    public Text pickupName;
    public Image pickupImage;

    public bool pickupAlertIsOn = false;
    public Queue<PickupPopupData> pickupQueue = new Queue<PickupPopupData>();

    public List<string> itemsPickedup;

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


    void Start()
    {
        isOpen = false;
        PopulateSlotList();

        Cursor.visible = false;
    }

    private void PopulateSlotList()
    {
        foreach (Transform child in inventoryScreenUI.transform)
        {
            if (child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.I) && !isOpen && !ConstructionManager.Instance.inConstructionMode)
        {

            OpenUI();

        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            CloseUI();
        }
    }

    public void OpenUI()
    {
        inventoryScreenUI.SetActive(true);
        inventoryScreenUI.GetComponentInParent<Canvas>().sortingOrder = MenuManager.Instance.SetAsFront();


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SelectionManager.Instance.DisableSelection();
        SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;

        isOpen = true;

    }

    public void CloseUI()
    {
        inventoryScreenUI.SetActive(false);
        if (!CraftingSystem.Instance.isOpen && !StorageManager.Instance.storageUIOpen && !CampfireUIManager.Instance.isUIOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            SelectionManager.Instance.EnableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
        }
        isOpen = false;
    }

    public void AddToInventory(string itemName)
    {

        Debug.Log(itemName);
        whatSlotToEquip = FindNextEmptySlot();

        itemToAdd = Instantiate(Resources.Load<GameObject>(itemName),
                                whatSlotToEquip.transform.position,
                                whatSlotToEquip.transform.rotation);
        itemToAdd.transform.SetParent(whatSlotToEquip.transform);

        itemList.Add(itemName);


        if (!SaveManager.Instance || !SaveManager.Instance.isLoading) //no save manager only happens when running scene directly
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.pickupItemSound);
            TriggerPickupPopUp(itemName, itemToAdd.GetComponent<Image>().sprite);
        }



        RecalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
        QuestManager.Instance.RefreshTrackerList();

    }


    public void TriggerPickupPopUp(string itemName, Sprite itemSprite)
    {
        if (!pickupAlertIsOn)
        {
            pickupAlertIsOn = true;
            pickupAlert.transform.position = new Vector3(247, 115, 0);
            pickupAlert.SetActive(true);
            pickupName.text = itemName;
            pickupImage.sprite = itemSprite;
        } else
        {
            var temp = new PickupPopupData();
            temp.itemName = itemName;
            temp.itemSprite = itemSprite;
            pickupQueue.Enqueue(temp);
        }
        
    }

    public bool CheckSlotsAvailable(int emptyNeeded)
    {
        int counter = 0;

        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                counter++;
            }
        }

        if (counter <= (21 - emptyNeeded))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private GameObject FindNextEmptySlot()
    {
        foreach(GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }

        return new GameObject();
    }

    public void RemoveItem(string nameToRemove, int amountToRemove)
    {
        int counter = amountToRemove;

        for (var i = slotList.Count - 1; i>=0; i--)
        {
            if (slotList[i].transform.childCount > 0)
            {
                if (slotList[i].transform.GetChild(0).name == nameToRemove + "(Clone)" && counter > 0)
                {
                    DestroyImmediate(slotList[i].transform.GetChild(0).gameObject);
                    counter--;
                }
            }
        }

        RecalculateList();
        CraftingSystem.Instance.RefreshNeededItems();

        QuestManager.Instance.RefreshTrackerList();
    }

    public void RecalculateList()
    {
        //clear item list and repopulate
        itemList.Clear();

        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                string name = slot.transform.GetChild(0).name.Replace("(Clone)", "");

                itemList.Add(name);
            }
        }
    }

    public int CheckItemAmount(string name)
    {
        int itemCounter = 0;

        foreach (string item in itemList)
        {
            if (item == name)
            {
                itemCounter++;
            }
        }
        return itemCounter;
    }
}

public class PickupPopupData
{
    public string itemName;
    public Sprite itemSprite;
}
