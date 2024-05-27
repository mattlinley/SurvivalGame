using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(BoxCollider))]
public class ChoppableTree : MonoBehaviour
{
    public bool playerInRange;
    public bool canBeChopped;

    public float treeMaxHealth;
    public float treeHealth = 0f;

    public string treeTypeName;

    public Animator animator;

    public float caloriesSpentChoppingWood = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void Start()
    {
        if (treeHealth == 0)
        {
            treeHealth = treeMaxHealth;
        }
        
        animator = transform.parent.transform.parent.GetComponent<Animator>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void GetHit()
    {
        animator.SetTrigger("shake");
        treeHealth -= 1;

        PlayerState.Instance.currentCalories -= caloriesSpentChoppingWood;

        if (treeHealth <= 0)
        {
            TreeIsDead();
        }
    }



    private void TreeIsDead()
    {
        Vector3 treePosition = transform.position;

        Destroy(transform.parent.transform.parent.gameObject);
        canBeChopped = false;
        SelectionManager.Instance.selectedTree = null;
        SelectionManager.Instance.chopHolder.gameObject.SetActive(false);

        GameObject brokenTree = Instantiate(Resources.Load<GameObject>("ChoppedTree" + treeTypeName),
                                            treePosition,
                                            Quaternion.Euler(0,transform.eulerAngles.y,0));
        //Set stump as child of [TREES]
        brokenTree.transform.SetParent(transform.parent.transform.parent.transform.parent);

        brokenTree.GetComponent<RegrowTree>().dayOfRegrowth = TimeManager.Instance.dayInGame + 2;


    }

    private void Update()
    {
        if (canBeChopped)
        {
            GlobalState.Instance.resourceHealth = treeHealth;
            GlobalState.Instance.resourceMaxHealth = treeMaxHealth;
        }
    }
}
