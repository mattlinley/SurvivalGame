using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerState : MonoBehaviour
{

    public static PlayerState Instance { get; set; }

    // player health
    public float currentHealth;
    public float maxHealth;


    // player calories
    public float currentCalories;
    public float maxCalories;


    // player hydration
    public float currentHydration;
    public float maxHydration;

    // player oxygen
    public float currentOxygen;
    public float maxOxygen = 100f;
    public float oxygenDecreasedPerSecond = 1f;
    private float oxygenTimer = 0f;
    private float decreaseInterval = 1f;
    public float damagePerSecond = 4f;

    float distanceTravelled = 0;
    Vector3 lastPosition;

    public GameObject playerBody;

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
        currentHealth = maxHealth;
        currentCalories = maxCalories;
        currentHydration = maxHydration;

        currentOxygen = maxOxygen;

        lastPosition = playerBody.transform.position;

        StartCoroutine(decreaseHydration());
    }

    IEnumerator decreaseHydration()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            currentHydration -= 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerBody.GetComponent<PlayerMovement>().isUnderwater)
        {
            oxygenTimer += Time.deltaTime;

            if (oxygenTimer >= decreaseInterval)
            {
                DecreaseOxygen();
                oxygenTimer = 0f;
            }
        }

        distanceTravelled += Vector3.Distance(playerBody.transform.position,
                                              lastPosition);
        lastPosition = playerBody.transform.position;

        if (distanceTravelled >= 5)
        {
            distanceTravelled = 0;
            currentCalories -= 1;
        }

        //testing the health bar
        if (Input.GetKeyDown(KeyCode.N))
        {
            currentHealth -= 10;
        }
    }

    private void DecreaseOxygen()
    {
        currentOxygen -= oxygenDecreasedPerSecond * decreaseInterval;

        if (currentOxygen < 0)
        {
            //damage player
            currentOxygen = 0;

            setHealth(currentHealth - damagePerSecond);
        }
    }

    public void setHealth (float newHealth)
    {
        currentHealth = newHealth;
    }

    public void setCalories(float newCalories)
    {
        currentCalories = newCalories;
    }

    public void setHydration(float newHydration)
    {
        currentHydration = newHydration;
    }
}
