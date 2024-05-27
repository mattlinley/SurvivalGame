using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool playerInRange;

    public string ItemName;

    [SerializeField] float detectionRange = 10f;

    public string GetItemName()
    {
        return ItemName;
    }

    private void Update()
    {
        float distance = Vector3.Distance(PlayerState.Instance.playerBody.transform.position, transform.position);

        if (distance < detectionRange)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }


        if (Input.GetKeyDown(KeyCode.Mouse0) && playerInRange && SelectionManager.Instance.onTarget && SelectionManager.Instance.selectedObject == gameObject)
        {
               //if the inventory is not full
            if (InventorySystem.Instance.CheckSlotsAvailable(1))
            {
                InventorySystem.Instance.AddToInventory(ItemName);

                InventorySystem.Instance.itemsPickedup.Add(ItemName);

                Destroy(gameObject);
            }
            else
            {
                Debug.Log("The inventory is full");
            }
            
        }
    }

    
}
