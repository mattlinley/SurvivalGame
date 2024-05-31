using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] public GameObject seedModel;
    [SerializeField] public GameObject youngPlantModel;
    [SerializeField] public GameObject maturePlantModel;

    public List<GameObject> plantProduceSpawns;

    public GameObject producePrefab;

    public int dayOfPlanting;
    public int plantAge = 0;

    [SerializeField] int ageForYoungModel;
    [SerializeField] int ageForMatureModel;
    [SerializeField] int ageForFirstProduceBatch;

    [SerializeField] int daysForNewProduce;
    public int daysRemainingForNewProduce;

    [SerializeField] bool isOneTimeHarvest;
    public bool isWatered;

    private void OnEnable()
    {
        TimeManager.Instance.OnDayPass.AddListener(DayPass);
    }

    private void OnDisable()
    {
        TimeManager.Instance.OnDayPass.RemoveListener(DayPass);
    }

    private void DayPass()
    {
        if (isWatered)
        {
            plantAge++;
        }

        CheckGrowth();

        if (!isOneTimeHarvest)
        {
            CheckProduce();
        }
        
    }

    public void CheckGrowth()
    {
        seedModel.SetActive(plantAge < ageForYoungModel);
        youngPlantModel.SetActive(plantAge >= ageForYoungModel && plantAge <= ageForMatureModel);
        maturePlantModel.SetActive(plantAge >= ageForMatureModel);

        //reset to not watered on each growth stage
        if (plantAge == ageForYoungModel || (plantAge == ageForMatureModel && !isOneTimeHarvest))
        {
            isWatered = false;
            GetComponentInParent<Soil>().MakeSoilUnwatered();
        }

        if (plantAge >= ageForMatureModel && isOneTimeHarvest)
        {
            MakePlantPickable();
        }
    }

    private void MakePlantPickable()
    {
        GetComponent<InteractableObject>().enabled = true;
        GetComponent<SphereCollider>().enabled = true;

    }

    private void CheckProduce()
    {
        if (plantAge == ageForFirstProduceBatch)
        {
            GenerateProduceForEmptySpawns();
        }

        if (plantAge > ageForFirstProduceBatch)
        {
            if (daysRemainingForNewProduce == 0 )
            {
                GenerateProduceForEmptySpawns();

                daysRemainingForNewProduce = daysForNewProduce;
            }
            else
            {
                daysRemainingForNewProduce--;
            }
        }
    }

    private void GenerateProduceForEmptySpawns()
    {
        foreach (GameObject spawn in plantProduceSpawns)
        {
            if (spawn.transform.childCount == 0)
            {
                //Instantiate the produce from the prefab
                GameObject produce = Instantiate(producePrefab);

                //Set produce to be a child of current spawn
                produce.transform.SetParent(spawn.transform);

                //Set position
                Vector3 producePosition = Vector3.zero;
                produce.transform.localPosition = producePosition;

            }
        }
    }
    private void OnDestroy()
    {
        GetComponentInParent<Soil>().isEmpty = true;
        GetComponentInParent<Soil>().plantName = "";
        GetComponentInParent<Soil>().currentPlant = null;
    }
}
