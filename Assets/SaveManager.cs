using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; set; }

    public bool isSavingToJson;
    public bool isLoading;

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

        //keep for all scenes
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(loadingScreen.gameObject);
    }

    // Json Project Save Path
    string jsonPathProject;
    // Json External/Real Save Path
    string jsonPathPersistant;
    // Binary Save Path
    string binaryPath;

    string filename = "SaveGame";

    public Canvas loadingScreen;

    private void Start()
    {
        binaryPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
        jsonPathProject = Application.dataPath + Path.AltDirectorySeparatorChar;
        jsonPathPersistant = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
    }



    #region ---------------- Saving Section ----------------
    public void SaveGame(int slotNumber)
    {
        AllGameData data = new AllGameData();

        data.playerData = GetPlayerData();

        data.environmentData = GetEnvironmentData();

        SavingTypeSwitch(data, slotNumber);
    }

    private EnvironmentData GetEnvironmentData()
    {
        List<string> itemsPickedup = InventorySystem.Instance.itemsPickedup;
        List<Quest> trackedQuests = QuestManager.Instance.allTrackedQuests;

        //Save time details
        TimeData timeData = new TimeData();
        timeData.currentWeather = WeatherSystem.Instance.currentWeather;
        timeData.daysInCurrentSeason = TimeManager.Instance.daysInCurrentSeason;
        timeData.yearInGame = TimeManager.Instance.yearInGame;
        timeData.dayInGame = TimeManager.Instance.dayInGame;
        timeData.currentDayOfWeek = TimeManager.Instance.currentDayOfWeek;
        timeData.currentTimeOfDay = DayNightSystem.Instance.currentTimeOfDay;
        timeData.currentSeason = TimeManager.Instance.currentSeason;


        //NPC and Quest progress
        List<NpcData> allNpcs = new List<NpcData>();

        foreach (Transform npc in EnvironmentManager.Instance.allNpcs.transform)
        {
            var npcd = new NpcData();
            var tempQuest = new QuestData();
            List<QuestData> npcQuests = new List<QuestData>();
            npcd.firstTimeInteraction = npc.GetComponent<NPC>().firstTimeInteraction;
            npcd.currentDialog = npc.GetComponent<NPC>().currentDialog;
            npcd.name = npc.name;
            npcd.activeQuestIndex = npc.GetComponent<NPC>().activeQuestIndex;

            foreach (Quest quest in npc.GetComponent<NPC>().quests)
            {

                tempQuest.accepted = quest.accepted;
                tempQuest.declined = quest.declined;
                tempQuest.isCompleted = quest.isCompleted;
                tempQuest.initialDialogCompleted = quest.initialDialogCompleted;
                npcQuests.Add(tempQuest);
            }
            npcd.questData = npcQuests;

            allNpcs.Add(npcd);
        }


        //Get all trees and stumps
        List<TreeData> treesToSave = new List<TreeData>();

        foreach (Transform tree in EnvironmentManager.Instance.allTrees.transform)
        {
            if (tree.CompareTag("Tree"))
            {
                var td = new TreeData();
                td.name = "TreeParent" + tree.GetChild(0).GetChild(0).GetComponent<ChoppableTree>().treeTypeName;
                td.position = tree.position;
                td.rotation = new Vector3(tree.eulerAngles.x, tree.eulerAngles.y, tree.eulerAngles.z);
                td.treeHealth = tree.GetChild(0).GetChild(0).GetComponent<ChoppableTree>().treeHealth;
                treesToSave.Add(td);
            }
            else
            {
                var td = new TreeData();
                td.name = "ChoppedTreeStump" + tree.GetChild(0).GetChild(0).GetComponent<ChoppableTree>().treeTypeName;
                td.position = tree.position;
                td.rotation = new Vector3(tree.eulerAngles.x, tree.eulerAngles.y, tree.eulerAngles.z);
                td.dayOfRegrowth = tree.GetComponent<RegrowTree>().dayOfRegrowth;
                treesToSave.Add(td);
            }
        }

        //Get all animals#
        List<string> allAnimals = new List<string>();
        foreach (Transform animalType in EnvironmentManager.Instance.allAnimals.transform)
        {
            foreach (Transform animal in animalType.transform)
            {
                allAnimals.Add(animal.gameObject.name);
            }
        }

        //Get information about storage boxes in the scene
        List<StorageData> allStorage = new List<StorageData>();
        List<CampfireData> allCampfires = new List<CampfireData>();
        List<ConstructionData> allConstructions = new List<ConstructionData>();
        List<PlanterData> allPlanters = new List<PlanterData>();

        foreach (Transform placeable in EnvironmentManager.Instance.allPlaceables.transform)
        {
            if (placeable.gameObject.GetComponent<StorageBox>())
            {
                var sd = new StorageData();
                sd.items = placeable.gameObject.GetComponent<StorageBox>().items;
                sd.position = placeable.position;
                sd.rotation = new Vector3(placeable.eulerAngles.x, placeable.eulerAngles.y, placeable.eulerAngles.z);

                allStorage.Add(sd);
            }

            //campfires
            if (placeable.gameObject.GetComponent<Campfire>())
            {
                var cd = new CampfireData();
                cd.food = placeable.gameObject.GetComponent<Campfire>().itemInFoodSlot;
                cd.fuel = placeable.gameObject.GetComponent<Campfire>().itemInFuelSlot;
                cd.position = placeable.position;
                cd.rotation = new Vector3(placeable.eulerAngles.x, placeable.eulerAngles.y, placeable.eulerAngles.z);

                allCampfires.Add(cd);
            }

            //construction
            if (placeable.name == "FoundationModel" || placeable.name == "DoorwayModel" || placeable.name == "WallModel")
            {
                var cd = new ConstructionData();
                cd.name = placeable.name;
                cd.position = placeable.position;
                cd.rotation = new Vector3(placeable.eulerAngles.x, placeable.eulerAngles.y, placeable.eulerAngles.z);

                allConstructions.Add(cd);
            }

            //planters
            if (placeable.gameObject.GetComponent<Soil>())
            {
                var pd = new PlanterData();
                if (placeable.gameObject.GetComponent<Soil>().isEmpty)
                {
                    pd.isPlanted = false;
                } else
                {
                    pd.isPlanted= true;
                    // find plant object by checking tags of children
                    foreach (Transform child in placeable.transform)
                    {
                        if (child.tag == "plant")
                        {
                            pd.plantName = child.name.Replace("(Clone)", "");
                            pd.plantAge = child.GetComponent<Plant>().plantAge;
                            pd.daysRemainingForNewProduce = child.GetComponent<Plant>().daysRemainingForNewProduce;
                            pd.isWatered = child.GetComponent<Plant>().isWatered;
                            pd.dayOfPlanting = child.GetComponent<Plant>().dayOfPlanting;
                            List<bool> spawnFull = new List<bool>();

                            foreach (GameObject produceSpawn in child.GetComponent<Plant>().plantProduceSpawns)
                            {
                                if (produceSpawn.transform.childCount > 0)
                                {
                                    spawnFull.Add(true);
                                } else
                                {
                                    spawnFull.Add(false);
                                }
                            }
                            pd.produceSpawns = spawnFull;
                        }
                    }
                }

                pd.position = placeable.position;
                pd.rotation = new Vector3(placeable.eulerAngles.x, placeable.eulerAngles.y, placeable.eulerAngles.z);

                allPlanters.Add(pd);
            }
        }

        return new EnvironmentData(itemsPickedup, treesToSave, allAnimals, allStorage, timeData, allNpcs, trackedQuests, allCampfires, allConstructions, allPlanters);
    }

    private PlayerData GetPlayerData()
    {
        float[] playerStats = new float[3];
        playerStats[0] = PlayerState.Instance.currentHealth;
        playerStats[1] = PlayerState.Instance.currentCalories;
        playerStats[2] = PlayerState.Instance.currentHydration;

        float[] playerPosAndRot = new float[6];
        playerPosAndRot[0] = PlayerState.Instance.playerBody.transform.position.x;
        playerPosAndRot[1] = PlayerState.Instance.playerBody.transform.position.y;
        playerPosAndRot[2] = PlayerState.Instance.playerBody.transform.position.z;

        playerPosAndRot[3] = PlayerState.Instance.playerBody.transform.rotation.eulerAngles.x;
        playerPosAndRot[4] = PlayerState.Instance.playerBody.transform.rotation.eulerAngles.y;
        playerPosAndRot[5] = PlayerState.Instance.playerBody.transform.rotation.eulerAngles.z;

        string[] inventory = InventorySystem.Instance.itemList.ToArray();

        string[] quickSlots = GetQuickSlotsContent();

        return new PlayerData(playerStats, playerPosAndRot, inventory, quickSlots);
    }

    private string[] GetQuickSlotsContent()
    {
        List<string> temp = new List<string>();

        foreach (GameObject slot in EquipSystem.Instance.quickSlotsList)
        {
            if (slot.transform.childCount != 0)
            {
                string name = slot.transform.GetChild(0).name.Replace("(Clone)","");
                temp.Add(name);
            }
        }

        return temp.ToArray();
    }

    public void SavingTypeSwitch(AllGameData gameData, int slotNumber)
    {
        if (isSavingToJson)
        {
           SaveGameDataToJsonFile(gameData, slotNumber);
        }
        else
        {
            SaveGameDataToBinaryFile(gameData, slotNumber);
        }
    }

    #endregion

    #region ---------------- Loading Section ---------------

    public AllGameData LoadingTypeSwitch(int slotNumber)
    {
        if (isSavingToJson)
        {
            AllGameData gameData = LoadGameDataFromJsonFile(slotNumber);
            return gameData;
        }
        else
        {
            AllGameData gameData = LoadGameDataFromBinaryFile(slotNumber);
            return gameData;
        }
    }



    public void LoadGame(int slotNumber)
    {
        //Player data
        SetPlayerData(LoadingTypeSwitch(slotNumber).playerData);

        //Enviroment data
        SetEnvironmentData(LoadingTypeSwitch(slotNumber).environmentData);

        isLoading = false;

        DisableLoadingScreen();
    }

    private void SetEnvironmentData(EnvironmentData environmentData)
    {
        //time data
        TimeData timeData = environmentData.timeData;
        WeatherSystem.Instance.currentWeather = timeData.currentWeather;
        if (timeData.currentWeather == WeatherSystem.WeatherCondition.Sunny)
        {
            WeatherSystem.Instance.StopRain();
            WeatherSystem.Instance.isSpecialWeather = false;
        }
        else
        {
            WeatherSystem.Instance.StartRain();
            WeatherSystem.Instance.isSpecialWeather = true;
        }
        DayNightSystem.Instance.currentTimeOfDay = timeData.currentTimeOfDay;
        TimeManager.Instance.currentSeason = timeData.currentSeason;
        TimeManager.Instance.currentDayOfWeek = timeData.currentDayOfWeek;
        TimeManager.Instance.dayInGame = timeData.dayInGame;
        TimeManager.Instance.daysInCurrentSeason = timeData.daysInCurrentSeason;
        TimeManager.Instance.yearInGame = timeData.yearInGame;
        TimeManager.Instance.UpdateUI();


        //npc and quest
        foreach(NpcData npc in environmentData.npcData)
        {
            Transform foundNpc = EnvironmentManager.Instance.allNpcs.transform.Find(npc.name);
            if (foundNpc != null)
            {
                foundNpc.GetComponent<NPC>().currentDialog = npc.currentDialog;
                foundNpc.GetComponent<NPC>().firstTimeInteraction = npc.firstTimeInteraction;
                foundNpc.GetComponent<NPC>().activeQuestIndex = npc.activeQuestIndex;

                for (int i = 0; i < npc.questData.Count; i++)
                {
                    foundNpc.GetComponent<NPC>().quests[i].accepted = npc.questData[i].accepted;
                    foundNpc.GetComponent<NPC>().quests[i].declined = npc.questData[i].declined;
                    foundNpc.GetComponent<NPC>().quests[i].isCompleted = npc.questData[i].isCompleted;
                    foundNpc.GetComponent<NPC>().quests[i].initialDialogCompleted = npc.questData[i].initialDialogCompleted;

                    if (npc.questData[i].accepted && !npc.questData[i].isCompleted)
                    {
                        QuestManager.Instance.AddActiveQuest(foundNpc.GetComponent<NPC>().quests[i]);
                    }
                    if (npc.questData[i].isCompleted)
                    {
                        QuestManager.Instance.allCompletedQuests.Add(foundNpc.GetComponent<NPC>().quests[i]);
                        QuestManager.Instance.RefreshQuestList();
                    }
                }
            }
        }

        QuestManager.Instance.allTrackedQuests = environmentData.trackedQuests;


        foreach (Transform itemType in EnvironmentManager.Instance.allItems.transform)
        {
            foreach (Transform item in itemType.transform)
            {
                if (environmentData.pickedupItems.Contains(item.name))
                {
                    Destroy(item.gameObject);
                }
            }
        }

        InventorySystem.Instance.itemsPickedup = environmentData.pickedupItems;

        //trees

        //first destroy all original trees
        foreach(Transform tree in EnvironmentManager.Instance.allTrees.transform)
        {
            Destroy(tree.gameObject);
        }

        //then load in trees and stumps from save
        foreach(TreeData tree in environmentData.treeData)
        {
            var treePrefab = Instantiate(Resources.Load<GameObject>(tree.name),
                                         new Vector3(tree.position.x, tree.position.y, tree.position.z),
                                         Quaternion.Euler(tree.rotation.x, tree.rotation.y, tree.rotation.z));
            if (tree.treeHealth>0)
            {
                treePrefab.transform.GetChild(0).GetChild(0).GetComponent<ChoppableTree>().treeHealth = tree.treeHealth;
            }
            if (tree.dayOfRegrowth>0)
            {
                treePrefab.GetComponent<RegrowTree>().dayOfRegrowth = tree.dayOfRegrowth;
            }
            treePrefab.transform.SetParent(EnvironmentManager.Instance.allTrees.transform);
        }

        //animals
        //Destroy animals that should not exist
        foreach (Transform animalType in EnvironmentManager.Instance.allAnimals.transform)
        {
            foreach (Transform animal in animalType.transform)
            {
                if (environmentData.animals.Contains(animal.gameObject.name) == false)
                {
                    Destroy(animal.gameObject);
                }
            }
        }

        //add storage boxes
        foreach (StorageData storage in environmentData.storage)
        {
            var storageBoxPrefab = Instantiate(Resources.Load<GameObject>("StorageBoxModel"),
                                               new Vector3(storage.position.x, storage.position.y, storage.position.z),
                                               Quaternion.Euler(storage.rotation.x, storage.rotation.y, storage.rotation.z));
            storageBoxPrefab.transform.SetParent(EnvironmentManager.Instance.allPlaceables.transform);
            storageBoxPrefab.GetComponent<StorageBox>().items = storage.items;
        }

        //add campfires
        foreach (CampfireData campfire in environmentData.campfires)
        {
            var campfirePrefab = Instantiate(Resources.Load<GameObject>("CampfireModel"),
                                             new Vector3(campfire.position.x, campfire.position.y, campfire.position.z),
                                             Quaternion.Euler(campfire.rotation.x, campfire.rotation.y, campfire.rotation.z));
            campfirePrefab.transform.SetParent(EnvironmentManager.Instance.allPlaceables.transform);
            campfirePrefab.GetComponent<Campfire>().itemInFoodSlot = campfire.food;
            campfirePrefab.GetComponent<Campfire>().itemInFuelSlot = campfire.fuel;
        }

        //add construction items
        foreach (ConstructionData construction in environmentData.constructions)
        {
            var constructionPrefab = Instantiate(Resources.Load<GameObject>(construction.name),
                                                 new Vector3(construction.position.x, construction.position.y, construction.position.z),
                                                 Quaternion.Euler(construction.rotation.x, construction.rotation.y, construction.rotation.z));
            constructionPrefab.transform.SetParent(EnvironmentManager.Instance.allPlaceables.transform);
            constructionPrefab.name = construction.name;
        }

        //add planters
        foreach (PlanterData planter in environmentData.planters)
        {
            var planterPrefab = Instantiate(Resources.Load<GameObject>("Planter_Model"),
                                            new Vector3(planter.position.x, planter.position.y, planter.position.z),
                                            Quaternion.Euler(planter.rotation.x, planter.rotation.y, planter.rotation.z));
            planterPrefab.transform.SetParent(EnvironmentManager.Instance.allPlaceables.transform);
            planterPrefab.name = "PlanterModel";

            if (planter.isPlanted)
            {
                //add plant
                //instantiate plant prefab
                GameObject instantiatedPlant = Instantiate(Resources.Load(planter.plantName) as GameObject);
                instantiatedPlant.transform.SetParent(planterPrefab.transform);
                instantiatedPlant.transform.localPosition = Vector3.zero;
                planterPrefab.GetComponent<Soil>().currentPlant = instantiatedPlant.GetComponent<Plant>();
                instantiatedPlant.GetComponent<Plant>().dayOfPlanting = planter.dayOfPlanting;

                //mark soil as occupied
                planterPrefab.GetComponent<Soil>().isEmpty = false;

                //set plant name
                planterPrefab.GetComponent<Soil>().plantName = planter.plantName;

                if (planter.isWatered)
                {
                    instantiatedPlant.GetComponent<Plant>().isWatered = true; //set bool
                    planterPrefab.GetComponent<Soil>().MakeSoilWatered(); //change texture
                }

                //set plant age
                instantiatedPlant.GetComponent<Plant>().plantAge = planter.plantAge;
                instantiatedPlant.GetComponent<Plant>().daysRemainingForNewProduce = planter.daysRemainingForNewProduce;

                //update model to current position
                instantiatedPlant.GetComponent<Plant>().CheckGrowth();

                int i = 0;

                //add any produce spawns
                foreach (GameObject spawn in instantiatedPlant.GetComponent<Plant>().plantProduceSpawns)
                {

                    if (planter.produceSpawns[i++])
                    {
                        //Instantiate the produce from the prefab
                        GameObject produce = Instantiate(instantiatedPlant.GetComponent<Plant>().producePrefab);

                        //Set produce to be a child of current spawn
                        produce.transform.SetParent(spawn.transform);

                        //Set position
                        produce.transform.localPosition = Vector3.zero;
                    }
                }


            }
        }
    }

    private void SetPlayerData(PlayerData playerData)
    {
        //Setting Player Stats
        PlayerState.Instance.currentHealth = playerData.playerStats[0];
        PlayerState.Instance.currentCalories = playerData.playerStats[1];
        PlayerState.Instance.currentHydration = playerData.playerStats[2];

        //Setting Player Position
        Vector3 loadedPosition;
        loadedPosition.x = playerData.playerPositionAndRotation[0];
        loadedPosition.y = playerData.playerPositionAndRotation[1];
        loadedPosition.z = playerData.playerPositionAndRotation[2];

        PlayerState.Instance.playerBody.transform.position = loadedPosition;

        //Setting Player Rotation
        Vector3 loadedRotation;
        loadedRotation.x = playerData.playerPositionAndRotation[3];
        loadedRotation.y = playerData.playerPositionAndRotation[4];
        loadedRotation.z = playerData.playerPositionAndRotation[5];

        //Setting Inventory content
        foreach (string item in playerData.inventoryContent)
        {
            InventorySystem.Instance.AddToInventory(item);
        }

        foreach (string item in playerData.quickSlotsContent)
        {
            //find next free quick slot
            GameObject availableSlot = EquipSystem.Instance.FindNextEmptySlot();

            var itemToAdd = Instantiate(Resources.Load<GameObject>(item));
        
            itemToAdd.transform.SetParent(availableSlot.transform, false);
        }

        PlayerState.Instance.playerBody.transform.localRotation = Quaternion.Euler(loadedRotation);

        //update the rotation variables used in mousemovement.
        PlayerState.Instance.playerBody.GetComponent<MouseMovement>().xRotation = loadedRotation.x;
        PlayerState.Instance.playerBody.GetComponent<MouseMovement>().yRotation = loadedRotation.y;


    }

    public void StartLoadedGame(int slotNumber)
    {
        ActivateLoadingScreen();
        SceneManager.LoadScene("GameScene");
        isLoading = true;
        StartCoroutine(DelayedLoading(slotNumber));
    }

    private IEnumerator DelayedLoading(int slotNumber)
    {
        yield return new WaitForSeconds(1);

        LoadGame(slotNumber);
        
    }

    public void ActivateLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(true);
        

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DisableLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(false);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    #endregion



    #region ---------------- Settings Section --------------

    #region ---------------- Volume Section ----------------

    //class of all volumes to save in one go
    [System.Serializable]
    public class VolumeSettings
    {
        public float music;
        public float effects;
        public float master;
    }

    public void SaveVolumeSettings(float _music, float _effects, float _master)
    {
        VolumeSettings volumeSettings = new VolumeSettings()
        {
            music = _music,
            effects = _effects,
            master = _master
        };

        //can only save as string (or int/float) so need to convert to json to save class
        PlayerPrefs.SetString("Volume", JsonUtility.ToJson(volumeSettings));
        PlayerPrefs.Save();

        Debug.Log("Saved to Player Pref");
    }

    public VolumeSettings LoadVolumeSettings()
    {
        return JsonUtility.FromJson<VolumeSettings>(PlayerPrefs.GetString("Volume"));
    }


    #endregion

    #endregion

    #region ---------------- To Json Section ---------------


    public void SaveGameDataToJsonFile(AllGameData gameData, int slotNumber)
    {
        string json = JsonUtility.ToJson(gameData);
        //string encrypted = EncryptionDecryption(json);


        using (StreamWriter writer = new StreamWriter(jsonPathProject + filename + slotNumber + ".json"))
        {
            writer.Write(json);
            Debug.Log("Saved to json file at: " + jsonPathProject + filename + slotNumber + ".json");
        }
    }

    public AllGameData LoadGameDataFromJsonFile(int slotNumber)
    {
        using (StreamReader reader = new StreamReader(jsonPathProject + filename + slotNumber + ".json"))
        {
            string json = reader.ReadToEnd();

            //string decrypted = EncryptionDecryption(json);

            return JsonUtility.FromJson<AllGameData>(json);
        }

    }

    public string EncryptionDecryption(string jsonString)
    {
        string keyword = "1234567";
        string result = "";

        for (int i=0; i < jsonString.Length; i++)
        {
            result += (char)(jsonString[i] ^ keyword[i % keyword.Length]);
        }

        return result;
    }

    #endregion

    #region ---------------- To Binary Section -------------


    public void SaveGameDataToBinaryFile(AllGameData gameData, int slotNumber)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = binaryPath + filename + slotNumber + ".bin";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, gameData);
        stream.Close();

        Debug.Log("Data saved to " + path);
    }

    public AllGameData LoadGameDataFromBinaryFile(int slotNumber)
    {
        string path = binaryPath + filename + slotNumber + ".bin";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            AllGameData data = formatter.Deserialize(stream) as AllGameData;
            stream.Close();

            Debug.Log("Data loaded from " + path);

            return data;
        }
        else
        {
            return null;
        }

    }

    #endregion

    #region ---------------- Utility Section ---------------

    public bool DoesFileExist(int slotNumber)
    {
        if (isSavingToJson)
        {
            if (System.IO.File.Exists(jsonPathProject + filename + slotNumber + ".json"))
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
            if (System.IO.File.Exists(binaryPath + filename + slotNumber + ".bin"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool IsSlotEmpty(int slotNumber)
    {
        if (DoesFileExist(slotNumber))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void DeselectButton()
    {
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }



    #endregion

}
