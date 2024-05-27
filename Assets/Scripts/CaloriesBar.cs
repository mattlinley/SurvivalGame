using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaloriesBar : MonoBehaviour
{
    private Slider slider;
    public Text caloriesCounter;



    private float currentCalories, maxCalories;



    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        currentCalories = PlayerState.Instance.currentCalories;
        maxCalories = PlayerState.Instance.maxCalories;

        float fillValue = currentCalories / maxCalories;
        slider.value = fillValue;

        caloriesCounter.text = currentCalories + "/" + maxCalories;
    }
}
