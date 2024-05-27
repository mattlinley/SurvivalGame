
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; set; }

    public bool onTarget;

    public GameObject selectedObject;

    public GameObject interaction_Info_UI;
    Text interaction_text;

    public Image centerDotImage;
    public Image handIcon;

    public bool handIsVisible;

    public GameObject selectedTree;
    public GameObject chopHolder;

    public GameObject selectedStorageBox;
    public GameObject selectedCampfire;
    public GameObject selectedSoil;

    private void Start()
    {
        onTarget = false;
        interaction_text = interaction_Info_UI.GetComponent<Text>();
    }

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

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray,
                            out hit))
        {
            var selectionTransform = hit.transform;

            

            

            NPC npc = selectionTransform.GetComponent<NPC>();

            if (npc && npc.playerInRange)
            {
                interaction_text.text = "Talk";
                interaction_Info_UI.SetActive(true);

                if (Input.GetMouseButtonDown(0) && npc.isTalkingWithPlayer==false)
                {
                    npc.StartConversation();
                }

                if (DialogSystem.Instance.dialogUIActive)
                {
                    interaction_Info_UI.SetActive(false);
                    centerDotImage.gameObject.SetActive(false);
                }
            }

            ChoppableTree choppableTree = selectionTransform.GetComponent<ChoppableTree>();

            if (choppableTree && choppableTree.playerInRange)
            {
                choppableTree.canBeChopped = true;
                selectedTree = choppableTree.gameObject;
                chopHolder.gameObject.SetActive(true);

            }
            else
            {
                if (selectedTree != null)
                {
                    selectedTree.gameObject.GetComponent<ChoppableTree>().canBeChopped = false;
                    selectedTree = null;
                    chopHolder.gameObject.SetActive(false);
                }
            }

            InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();

            if (interactable && interactable.playerInRange)
            {

                onTarget = true;
                selectedObject = interactable.gameObject;
                interaction_text.text = interactable.GetItemName();
                interaction_Info_UI.SetActive(true);

                centerDotImage.gameObject.SetActive(false);
                handIcon.gameObject.SetActive(true);

                handIsVisible = true;


            }

            StorageBox storageBox = selectionTransform.GetComponent<StorageBox>();

            if (storageBox && storageBox.playerInRange && !PlacementSystem.Instance.inPlacementMode)
            {
                interaction_text.text = "Open";
                interaction_Info_UI.SetActive(true);

                selectedStorageBox = storageBox.gameObject;

                if (Input.GetMouseButtonDown(0))
                {
                    StorageManager.Instance.OpenBox(storageBox);
                }
            }
            else
            {
                if (selectedStorageBox != null)
                {
                    selectedStorageBox = null;
                }
            }


            Campfire campfire = selectionTransform.GetComponent<Campfire>();

            if (campfire && campfire.playerInRange && !PlacementSystem.Instance.inPlacementMode)
            {
                interaction_text.text = "Interact";
                interaction_Info_UI.SetActive(true);

                selectedCampfire= campfire.gameObject;

                if (Input.GetMouseButtonDown(0) && campfire.isCooking == false)
                {
                    campfire.OpenUI();
                }
            }
            else
            {
                if (selectedCampfire != null)
                {
                    selectedCampfire = null;
                }
            }


            Animal animal = selectionTransform.GetComponent<Animal>();

            if (animal &&  animal.playerInRange)
            {
                if (animal.isDead )
                {
                    interaction_text.text = "Loot";
                    interaction_Info_UI.SetActive(true);

                    centerDotImage.gameObject.SetActive(false);
                    handIcon.gameObject.SetActive(true);

                    handIsVisible = true;

                    if (Input.GetMouseButtonDown(0))
                    {
                        Lootable lootable = animal.GetComponent<Lootable>();
                        Loot(lootable);
                    }
                }
                else
                {
                    interaction_text.text = animal.animalName;
                    interaction_Info_UI.SetActive(true);

                    centerDotImage.gameObject.SetActive(true);
                    handIcon.gameObject.SetActive(false);

                    handIsVisible = false;

                    if (Input.GetMouseButtonDown(0) && EquipSystem.Instance.IsHoldingWeapon() && EquipSystem.Instance.IsThereASwingLock()==false)
                    {
                        StartCoroutine(DealDamageTo(animal, 0.3f, EquipSystem.Instance.GetWeaponDamage()));
                    }
                }
                
                
            }

            Soil soil = selectionTransform.GetComponent<Soil>();

            if (soil && soil.playerInRange)
            {
                if (soil.isEmpty && EquipSystem.Instance.IsHoldingSeed())
                {
                    interaction_text.text = "Plant " + EquipSystem.Instance.selectedItem.GetComponent<InventoryItem>().thisName.Replace(" Seed", "");
                    interaction_Info_UI.SetActive(true);

                    if (Input.GetMouseButtonDown(0))
                    {
                        soil.PlantSeed();
                        Destroy(EquipSystem.Instance.selectedItem);
                        Destroy(EquipSystem.Instance.selectedItemModel);
                        EquipSystem.Instance.SelectQuickSlot(EquipSystem.Instance.selectedNumber);
                    }
                }
                else if (soil.isEmpty)
                {
                    interaction_text.text = "Soil";
                    interaction_Info_UI.SetActive(true);
                }
                else
                {
                    if (EquipSystem.Instance.IsPlayerHoldingWateringCan())
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            Debug.Log("test");
                        }
                            if (soil.currentPlant.isWatered)
                        {
                            interaction_text.text = soil.plantName;
                            interaction_Info_UI.SetActive(true);
                        }
                        else
                        {
                            interaction_text.text = "Water Plant";
                            interaction_Info_UI.SetActive(true);

                            if (Input.GetMouseButtonDown(0))
                            {
                                SoundManager.Instance.wateringChannel.PlayOneShot(SoundManager.Instance.wateringCan);
                                soil.currentPlant.isWatered = true;
                                soil.MakeSoilWatered();
                            }
                        }

                    }
                    else
                    {
                        interaction_text.text = soil.plantName;
                        interaction_Info_UI.SetActive(true);
                    }
                    
                }

                selectedSoil = soil.gameObject;
            }
            else
            {
                if (selectedSoil != null)
                {
                    selectedSoil = null;
                }
            }

            if (!interactable && !animal)
            {
                onTarget = false;
                handIsVisible = false;

                centerDotImage.gameObject.SetActive(true);
                handIcon.gameObject.SetActive(false);
            }

            if (!npc && !interactable && !animal && !choppableTree && !storageBox && !campfire && !soil)
            {
                interaction_text.text = "";
                interaction_Info_UI.SetActive(false);
            }

        }

    }

    private void Loot(Lootable lootable)
    {
        if (lootable.wasLootCalculated == false)
        {
            List<LootReceived> receivedLoot = new List<LootReceived>();

            foreach (LootPossibility loot in lootable.possibleLoot)
            {

                var lootAmount = UnityEngine.Random.Range(loot.amountMin, loot.amountMax + 1);
                if (lootAmount != 0)
                {
                    LootReceived lt = new LootReceived();
                    lt.item = loot.item;
                    lt.amount = lootAmount;

                    receivedLoot.Add(lt);
                }
            }

            lootable.finalLoot = receivedLoot;
            lootable.wasLootCalculated = true;
        }

        Vector3 lootSpawnPosition = lootable.gameObject.transform.position;

        foreach (LootReceived lootReceived in lootable.finalLoot)
        {
            for (int i = 0; i<lootReceived.amount; i++)
            {
                Debug.Log(lootReceived.item.name + "_Model");
                GameObject lootSpawn = Instantiate(Resources.Load<GameObject>(lootReceived.item.name + "_Model"),
                                                   new Vector3(lootSpawnPosition.x, lootSpawnPosition.y + 0.2f, lootSpawnPosition.z),
                                                   Quaternion.Euler(0, 0, 0));
            }
        }

        Destroy(lootable.gameObject);


    }

    private IEnumerator DealDamageTo(Animal animal, float delay, int damage)
    {
        yield return new WaitForSeconds(delay);
        animal.TakeDamage(damage);
    }

    public void DisableSelection()
    {
        handIcon.enabled = false;
        centerDotImage.enabled = false;
        interaction_Info_UI.SetActive(false);
        selectedObject = null;

    }

    public void EnableSelection()
    {
        handIcon.enabled = true;
        centerDotImage.enabled = true;
        interaction_Info_UI.SetActive(true);
    }
}
