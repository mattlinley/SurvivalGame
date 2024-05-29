using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableItem : MonoBehaviour
{
    // Validation
    [SerializeField] bool isGrounded;
    [SerializeField] bool isOverlappingItems;
    public bool isValidToBeBuilt;

    [SerializeField] BoxCollider solidCollider;
    private Outline outline;

    private void Start()
    {
        outline = GetComponent<Outline>();
    }

    void Update()
    {
        if (isGrounded && isOverlappingItems == false)
        {
            isValidToBeBuilt = true;
        }
        else
        {
            isValidToBeBuilt = false;
        }

        // Raycast from the box's position towards its center

        //Height of box collider
        var boxHeight = transform.GetComponent<BoxCollider>().size.y * transform.localScale.y;

        RaycastHit groundHit;

        //Find box collider center position. Need to take object position then add the box collider centre, adjusted for object scale
        Vector3 boxColliderCenterPosition = transform.position + transform.GetComponent<BoxCollider>().center * transform.localScale.y;

        //Then a raycast distance of half box height from box center will be the bottom of the object
        if (Physics.Raycast(boxColliderCenterPosition, Vector3.down, out groundHit, boxHeight * 0.5f, LayerMask.GetMask("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

    }

    #region || --- On Triggers --- |
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground") && PlacementSystem.Instance.inPlacementMode)
        {
            // Making sure the item is parallel to the ground
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                // Align the box's rotation with the ground normal
                Quaternion newRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                transform.rotation = newRotation;

                isGrounded = true;
            }
        }

        if (other.CompareTag("Tree") || other.CompareTag("Pickable"))
        {
            Debug.Log("Overlapping: "+other.name);
            isOverlappingItems = true;
        }
    }
    

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground") && PlacementSystem.Instance.inPlacementMode)
        {
            isGrounded = false;
        }

        if (other.CompareTag("Tree") || other.CompareTag("Pickable") && PlacementSystem.Instance.inPlacementMode)
        {
            isOverlappingItems = false;
        }
    }
    #endregion


    #region || --- Set Outline Colors --- |
    public void SetInvalidColor()
    {
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.red;
        }

    }

    public void SetValidColor()
    {
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.green;
        }
    }

    public void SetDefaultColor()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
    #endregion
}
