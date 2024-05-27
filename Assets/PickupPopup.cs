using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupPopup : MonoBehaviour
{
    public void ResetPopup()
    {
        gameObject.SetActive(false);
        InventorySystem.Instance.pickupAlertIsOn = false;

        if (InventorySystem.Instance.pickupQueue.Count > 0 )
        {
            var temp = InventorySystem.Instance.pickupQueue.Dequeue();
            InventorySystem.Instance.TriggerPickupPopUp(temp.itemName, temp.itemSprite);
        }


    }
}
