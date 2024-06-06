using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimArea : MonoBehaviour
{

    public GameObject oxygenBar;
    public bool isDrinking;
    public bool inRangeToDrink;

    private void Update()
    {
        float distance = Vector3.Distance(PlayerState.Instance.playerBody.transform.position, transform.position);

        if (distance < 4f)
        {
            inRangeToDrink = true;
        }
        else
        {
            inRangeToDrink = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>().isSwimming = true;
        }

        if (other.CompareTag("MainCamera"))
        {
            //get component in "player"
            other.GetComponentInParent<PlayerMovement>().isUnderwater = true;
            oxygenBar.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>().isSwimming = false;
        }

        if (other.CompareTag("MainCamera"))
        {
            //get component in "player"
            other.GetComponentInParent<PlayerMovement>().isUnderwater = false;
            oxygenBar.SetActive(false);
            PlayerState.Instance.currentOxygen = PlayerState.Instance.maxOxygen;
        }
    }

    internal void isDrink()
    {
        //update hydration percent
        PlayerState.Instance.currentHydration = Mathf.Min(100, PlayerState.Instance.currentHydration + 25f);
        isDrinking = true;

        //Play Sound
        SoundManager.Instance.PlaySound(SoundManager.Instance.drinkSound);

        //cooldown
        StartCoroutine(drinkCooldown());

    }

    private IEnumerator drinkCooldown()
    {
        yield return new WaitForSeconds(2f);
        isDrinking = false;

    }
}
