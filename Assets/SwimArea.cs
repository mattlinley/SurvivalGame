using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimArea : MonoBehaviour
{

    public GameObject oxygenBar;
    
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
}
