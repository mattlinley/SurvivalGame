using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegrowTree : MonoBehaviour
{
    public int dayOfRegrowth;
    public string treeTypeName;

    public bool growthLock = false;

    private void Update()
    {
        if (TimeManager.Instance.dayInGame == dayOfRegrowth && growthLock == false)
        {
            growthLock = true;
            RespawnTree();
        }
    }

    private void RespawnTree()
    {
        //move any remaining logs out of object so they are not destroyed
        foreach (Transform child in gameObject.transform)
        {
            if (child.name == "Log")
            {
                child.transform.SetParent(transform.parent);
            }
        }

        gameObject.SetActive(false);

      

        GameObject newTree = Instantiate(Resources.Load<GameObject>("TreeParent" + treeTypeName),
                                         new Vector3(transform.position.x, transform.position.y, transform.position.z),
                                         Quaternion.Euler(0, transform.eulerAngles.y, 0));


        newTree.transform.SetParent(transform.parent);
        //destroy stump
        Destroy(gameObject);
    }
}
