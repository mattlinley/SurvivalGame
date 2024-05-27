using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{

    public float mouseSensitivity;

    public float xRotation = 0f;
    public float yRotation = 0f;

    void Start()
    {
        //Locking the cursor to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (InventorySystem.Instance.isOpen == false && 
            CraftingSystem.Instance.isOpen == false &&
            MenuManager.Instance.isMenuOpen == false && 
            DialogSystem.Instance.dialogUIActive == false &&
            QuestManager.Instance.isQuestMenuOpen == false &&
            StorageManager.Instance.storageUIOpen == false &&
            CampfireUIManager.Instance.isUIOpen == false)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            //control rotation around x axis (Look up and down)
            xRotation -= mouseY;

            //we clamp the rotation so we cant Over-rotate (like in real life)
            xRotation = Mathf.Clamp(xRotation,
                                    -90f,
                                    90f);

            //control rotation around y axis (Look up and down)
            yRotation += mouseX;

            //applying both rotations
            transform.localRotation = Quaternion.Euler(0f,
                                                       yRotation,
                                                       0f);
            transform.Find("Main Camera").localRotation = Quaternion.Euler(xRotation,
                                                       0f,
                                                       0f);
            //Debug.Log(xRotation + ", " + yRotation);
        }
        
    }
}
