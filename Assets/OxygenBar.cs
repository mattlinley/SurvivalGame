using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OxygenBar : MonoBehaviour
{
    private Slider slider;
    public Text oxygenCounter;

    private float currentOxygen, maxOxygen;



    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        currentOxygen = PlayerState.Instance.currentOxygen;
        maxOxygen = PlayerState.Instance.maxOxygen;

        float fillValue = currentOxygen / maxOxygen;
        slider.value = fillValue;

        oxygenCounter.text = currentOxygen + "%";
    }
}
