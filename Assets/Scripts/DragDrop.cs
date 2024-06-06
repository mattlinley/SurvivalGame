using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public static GameObject itemBeingDragged;
    Vector3 startPosition;
    Transform startParent;
    Canvas mainCanvas;



    private void Awake()
    {

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        mainCanvas = GetComponentInParent<Canvas>();
        //Debug.Log(mainCanvas.scaleFactor);
    }


    public void OnBeginDrag(PointerEventData eventData)
    {

        Debug.Log("OnBeginDrag");
        canvasGroup.alpha = .6f;
        //So the ray cast will ignore the item itself.
        canvasGroup.blocksRaycasts = false;
        startPosition = transform.position;
        startParent = transform.parent;
        transform.SetParent(transform.root);
        itemBeingDragged = gameObject;

    }

    public void OnDrag(PointerEventData eventData)
    {
        //So the item will move with our mouse (at same speed)  and so it will be consistant if the canvas has a different scale (other then 1);
        rectTransform.anchoredPosition += eventData.delta / mainCanvas.scaleFactor;

    }



    public void OnEndDrag(PointerEventData eventData)
    {
        var  tempItemReference = itemBeingDragged;
        itemBeingDragged = null;

        if (transform.parent == startParent || transform.parent == transform.root)
        {
            //don't want to drop placeable items or seeds
            if (!tempItemReference.GetComponent<InventoryItem>().isUsable && tempItemReference.name!="TomatoSeed" && tempItemReference.name != "PumpkinSeed")
            {

                //drop item into world
                //hide icon
                tempItemReference.SetActive(false);
                AlertDialogManager dialogManager = FindObjectOfType<AlertDialogManager>();
                dialogManager.ShowDialog("Do you want to drop this item?", (response) =>
                {
                    if (response)
                    {
                        DropItemIntoWorld(tempItemReference);
                    }
                    else
                    {
                        //return item to slot
                        transform.position = startPosition;
                        transform.SetParent(startParent);
                        tempItemReference.SetActive(true);
                    }
                });

            }

            transform.position = startPosition;
            transform.SetParent(startParent);

        }

        Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    private void DropItemIntoWorld(GameObject tempItemReference)
    {
        //get clean name
        GameObject item = Instantiate(Resources.Load<GameObject>(tempItemReference.name.Replace("(Clone)", "") + "_Model"));
        //animator messes up position so disable if there is one
        if (item.GetComponent<Animator>() != null)
        {
            item.GetComponent<Animator>().enabled = false;
        }
        
        Debug.Log(tempItemReference.name.Replace("(Clone)", "") + "_Model");
        item.transform.position = Vector3.zero;
        var dropSpawnPosition = PlayerState.Instance.playerBody.transform.Find("Main Camera").transform.Find("DropSpawn").transform.position;
        item.transform.localPosition = new Vector3(dropSpawnPosition.x, dropSpawnPosition.y, dropSpawnPosition.z);
        Debug.Log(dropSpawnPosition.x.ToString());

        var itemsObject = FindObjectOfType<EnvironmentManager>().gameObject.transform.Find("[ITEMS]");
        item.transform.SetParent(itemsObject.transform);

        DestroyImmediate(tempItemReference.gameObject);
        InventorySystem.Instance.RecalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
    }
}
