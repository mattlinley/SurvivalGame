using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil : MonoBehaviour
{
    public bool isEmpty = true;

    public bool playerInRange;
    public string plantName;

    public Plant currentPlant;

    public Material defaultMaterial;
    public Material wateredMaterial;

    public void MakeSoilWatered()
    {
        GetComponent<Renderer>().material = wateredMaterial;
    }
    public void MakeSoilUnwatered()
    {
        GetComponent<Renderer>().material = defaultMaterial;
    }

    public void PlantSeed()
    {
        InventoryItem selectedSeed = EquipSystem.Instance.selectedItem.GetComponent<InventoryItem>();
        isEmpty = false;

        plantName = selectedSeed.thisName.Replace(" Seed", "");

        //instantiate plant prefab
        GameObject instantiatedPlant = Instantiate(Resources.Load(plantName + "Plant") as GameObject);

        instantiatedPlant.transform.SetParent(gameObject.transform);

        instantiatedPlant.transform.localPosition = Vector3.zero;

        currentPlant = instantiatedPlant.GetComponent<Plant>();
        currentPlant.dayOfPlanting = TimeManager.Instance.dayInGame;
    }

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
    }

}
