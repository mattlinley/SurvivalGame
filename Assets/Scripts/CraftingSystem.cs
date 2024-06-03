using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    public GameObject craftingScreenUI;
    public GameObject toolsScreenUI;
    public GameObject survivalScreenUI;
    public GameObject refineScreenUI;
    public GameObject constructionScreenUI;

    public List<string> inventoryItemList = new List<string>();

    Button toolsBTN, survivalBTN, refineBTN, constructionBTN;
    Button craftAxeBTN, craftPlankBTN, craftFoundationBTN, craftWallBTN, craftStorageBoxBTN, craftDoorwayBTN, craftCampfireBTN,craftPlanterBTN;
    Text AxeReq1, AxeReq2, PlankReq1, FoundationReq1, WallReq1, DoorwayReq1, StorageBoxReq1, CampfireReq1, CampfireReq2, PlanterReq1;

    public bool isOpen;

    //blueprints
    public Blueprint AxeBLP = new Blueprint("Axe",1 , 2, "Stone", 3, "Stick", 3);
    public Blueprint PlankBLP = new Blueprint("Plank", 2, 1, "Log", 1, "", 0);
    public Blueprint FoundationBLP = new Blueprint("Foundation", 1, 1, "Plank", 4, "", 0);
    public Blueprint WallBLP = new Blueprint("Wall", 1, 1, "Plank", 2, "", 0);
    public Blueprint DoorwayBLP = new Blueprint("Doorway", 1, 1, "Plank", 2, "", 0);
    public Blueprint StorageBoxBLP = new Blueprint("StorageBox", 1, 1, "Plank", 2, "", 0);
    public Blueprint CampfireBLP = new Blueprint("Campfire", 1, 2, "Stick", 2, "Stone", 4);
    public Blueprint PlanterBLP = new Blueprint("Planter", 1, 1, "Plank", 2, "", 0);

    public static CraftingSystem Instance { get; set; }

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
        isOpen = false;
        //crafting screens
        toolsBTN = craftingScreenUI.transform.Find("ToolsButton").GetComponent<Button>();
        toolsBTN.onClick.AddListener(delegate { OpenToolsCategory(); });

        survivalBTN = craftingScreenUI.transform.Find("SurvivalButton").GetComponent<Button>();
        survivalBTN.onClick.AddListener(delegate { OpenSurvivalCategory(); });

        refineBTN = craftingScreenUI.transform.Find("RefineButton").GetComponent<Button>();
        refineBTN.onClick.AddListener(delegate { OpenRefineCategory(); });

        constructionBTN = craftingScreenUI.transform.Find("ConstructionButton").GetComponent<Button>();
        constructionBTN.onClick.AddListener(delegate { OpenConstructionCategory(); });

        //Axe
        AxeReq1 = toolsScreenUI.transform.Find("Axe").transform.Find("Req1").GetComponent<Text>();
        AxeReq2 = toolsScreenUI.transform.Find("Axe").transform.Find("Req2").GetComponent<Text>();
        craftAxeBTN = toolsScreenUI.transform.Find("Axe").transform.Find("Button").GetComponent <Button>();
        craftAxeBTN.onClick.AddListener(delegate { CraftAnyItem(AxeBLP); });

        //Plank
        PlankReq1 = refineScreenUI.transform.Find("Plank").transform.Find("Req1").GetComponent<Text>();
        craftPlankBTN = refineScreenUI.transform.Find("Plank").transform.Find("Button").GetComponent<Button>();
        craftPlankBTN.onClick.AddListener(delegate { CraftAnyItem(PlankBLP); });

        //Foundation
        FoundationReq1 = constructionScreenUI.transform.Find("Foundation").transform.Find("Req1").GetComponent<Text>();
        craftFoundationBTN = constructionScreenUI.transform.Find("Foundation").transform.Find("Button").GetComponent<Button>();
        craftFoundationBTN.onClick.AddListener(delegate { CraftAnyItem(FoundationBLP); });

        //Wall
        WallReq1 = constructionScreenUI.transform.Find("Wall").transform.Find("Req1").GetComponent<Text>();
        craftWallBTN = constructionScreenUI.transform.Find("Wall").transform.Find("Button").GetComponent<Button>();
        craftWallBTN.onClick.AddListener(delegate { CraftAnyItem(WallBLP); });

        //Doorway
        DoorwayReq1 = constructionScreenUI.transform.Find("Doorway").transform.Find("Req1").GetComponent<Text>();
        craftDoorwayBTN = constructionScreenUI.transform.Find("Doorway").transform.Find("Button").GetComponent<Button>();
        craftDoorwayBTN.onClick.AddListener(delegate { CraftAnyItem(DoorwayBLP); });

        //Storage Box
        StorageBoxReq1 = survivalScreenUI.transform.Find("StorageBox").transform.Find("Req1").GetComponent<Text>();
        craftStorageBoxBTN = survivalScreenUI.transform.Find("StorageBox").transform.Find("Button").GetComponent<Button>();
        craftStorageBoxBTN.onClick.AddListener(delegate { CraftAnyItem(StorageBoxBLP); });

        //Campfire
        CampfireReq1 = survivalScreenUI.transform.Find("Campfire").transform.Find("Req1").GetComponent<Text>();
        CampfireReq2 = survivalScreenUI.transform.Find("Campfire").transform.Find("Req2").GetComponent<Text>();
        craftCampfireBTN = survivalScreenUI.transform.Find("Campfire").transform.Find("Button").GetComponent<Button>();
        craftCampfireBTN.onClick.AddListener(delegate { CraftAnyItem(CampfireBLP); });

        //Planter
        PlanterReq1 = survivalScreenUI.transform.Find("Planter").transform.Find("Req1").GetComponent<Text>();
        craftPlanterBTN = survivalScreenUI.transform.Find("Planter").transform.Find("Button").GetComponent<Button>();
        craftPlanterBTN.onClick.AddListener(delegate { CraftAnyItem(PlanterBLP); });
    }

    private void CraftAnyItem(Blueprint blueprintToCraft)
    {
        Debug.Log("craft");

        SoundManager.Instance.PlaySound(SoundManager.Instance.craftingSound);

        //remove items used
        InventorySystem.Instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1amount);
        if (blueprintToCraft.numOfRequirements > 1)
        {
            InventorySystem.Instance.RemoveItem(blueprintToCraft.Req2, blueprintToCraft.Req2amount);
        }
        if (blueprintToCraft.numOfRequirements > 2)
        {
            // can add 3rd requirement here...
        }

        //add item(s) to inventory
        for (int i = 0; i < blueprintToCraft.numItemsToProduce; i++)
        {
            InventorySystem.Instance.AddToInventory(blueprintToCraft.itemName);
        }
        

        StartCoroutine(calculate());

        
    }

    public IEnumerator calculate()
    {
        yield return 0;
        InventorySystem.Instance.RecalculateList();
        RefreshNeededItems();
    }

    public void RefreshNeededItems()
    {
        int stone_count = 0;
        int stick_count = 0;
        int log_count = 0;
        int plank_count = 0;

        inventoryItemList = InventorySystem.Instance.itemList;

        foreach (string itemName in inventoryItemList)
        {
            switch (itemName)
            {
                case "Stone":
                    stone_count++;
                    break;
                case "Stick":
                    stick_count++;
                    break;
                case "Log":
                    log_count++;
                    break;
                case "Plank":
                    plank_count++;
                    break;
            }
        }

        // AXE
        AxeReq1.text = "3 Stone [" + stone_count + "]";
        AxeReq2.text = "3 Stick [" + stick_count + "]";

        if (stone_count >= 3 && stick_count >= 3)
        {
            craftAxeBTN.gameObject.SetActive(true);
        }
        else
        {
            craftAxeBTN.gameObject.SetActive(false);
        }

        // CAMPFIRE
        CampfireReq1.text = "2 Stick [" + stick_count + "]";
        CampfireReq2.text = "4 Stone [" + stone_count + "]";

        if (stone_count >= 4 && stick_count >= 2)
        {
            craftCampfireBTN.gameObject.SetActive(true);
        }
        else
        {
            craftCampfireBTN.gameObject.SetActive(false);
        }

        //PLANK
        PlankReq1.text = "1 Log [" + log_count + "]";

        if (log_count >= 1 && InventorySystem.Instance.CheckSlotsAvailable(1))
        {
            craftPlankBTN.gameObject.SetActive(true);
        }
        else
        {
            craftPlankBTN.gameObject.SetActive(false);
        }

        //FOUNDATION
        FoundationReq1.text = "4 Plank [" + plank_count + "]";

        if (plank_count >= 4)
        {
            craftFoundationBTN.gameObject.SetActive(true);
        }
        else
        {
            craftFoundationBTN.gameObject.SetActive(false);
        }

        //WALL
        WallReq1.text = "2 Plank [" + plank_count + "]";

        if (plank_count >= 2)
        {
            craftWallBTN.gameObject.SetActive(true);
        }
        else
        {
            craftWallBTN.gameObject.SetActive(false);
        }

        //DOORWAY
        DoorwayReq1.text = "2 Plank [" + plank_count + "]";

        if (plank_count >= 2)
        {
            craftDoorwayBTN.gameObject.SetActive(true);
        }
        else
        {
            craftDoorwayBTN.gameObject.SetActive(false);
        }

        //STORAGE BOX
        StorageBoxReq1.text = "2 Plank [" + plank_count + "]";

        if (plank_count >= 2)
        {
            craftStorageBoxBTN.gameObject.SetActive(true);
        }
        else
        {
            craftStorageBoxBTN.gameObject.SetActive(false);
        }

        //PLANTER
        PlanterReq1.text = "2 Plank [" + plank_count + "]";

        if (plank_count >= 2)
        {
            craftPlanterBTN.gameObject.SetActive(true);
        }
        else
        {
            craftPlanterBTN.gameObject.SetActive(false);
        }
    }

    private void OpenToolsCategory()
    {
        craftingScreenUI.SetActive(false);
        survivalScreenUI.SetActive(false);
        refineScreenUI.SetActive(false);
        constructionScreenUI.SetActive(false);
        toolsScreenUI.SetActive(true);
    }

    private void OpenSurvivalCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(false);
        refineScreenUI.SetActive(false);
        constructionScreenUI.SetActive(false);
        survivalScreenUI.SetActive(true);
    }

    private void OpenRefineCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(false);
        survivalScreenUI.SetActive(false);
        constructionScreenUI.SetActive(false);
        refineScreenUI.SetActive(true);
    }

    private void OpenConstructionCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(false);
        survivalScreenUI.SetActive(false);
        refineScreenUI.SetActive(false);
        constructionScreenUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.C) && !isOpen && !ConstructionManager.Instance.inConstructionMode)
        {

            craftingScreenUI.SetActive(true);
            craftingScreenUI.GetComponentInParent<Canvas>().sortingOrder = MenuManager.Instance.SetAsFront();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SelectionManager.Instance.DisableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;

            isOpen = true;

        }
        else if (Input.GetKeyDown(KeyCode.C) && isOpen)
        {
            craftingScreenUI.SetActive(false);
            toolsScreenUI.SetActive(false );
            survivalScreenUI.SetActive(false);
            refineScreenUI.SetActive(false);
            constructionScreenUI.SetActive(false);
            if (!InventorySystem.Instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                SelectionManager.Instance.EnableSelection();
                SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
            }
            isOpen = false;
        }
    }
}
