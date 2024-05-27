using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HydrationBar : MonoBehaviour
{
    private Slider slider;
    public Text hydrationCounter;


    private float currentHydration, maxHydration;



    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        currentHydration = PlayerState.Instance.currentHydration;
        maxHydration = PlayerState.Instance.maxHydration;

        float fillValue = currentHydration / maxHydration;
        slider.value = fillValue;

        hydrationCounter.text = currentHydration + "%";
    }
}
