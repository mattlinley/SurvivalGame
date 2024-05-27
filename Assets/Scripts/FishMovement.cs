using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

public class FishMovement : MonoBehaviour
{
    public float maxLeft = -250;
    public float maxRight = 250;

    public float moveSpeed = 250f;
    public float changeFrequency = 0.01f;

    public float targetPosition;
    public bool movingRight = true;

    internal void SetDifficulty(FishData fishBiting)
    {
        switch (fishBiting.fishDifficulty)
        {
            case 1:
                moveSpeed = 200;
                return;
            case 2:
                moveSpeed = 300;
                return;
            case 3:
                moveSpeed = 350;
                return;
            case 0:
                moveSpeed = 250;
                Debug.LogError("Difficulty not found");
                return;
            default:
                return;

        }
    }



    // Start is called before the first frame update
    void Start()
    {
       targetPosition = Random.Range(maxLeft, maxRight);

    }

    // Update is called once per frame
    void Update()
    {
        //Move fish towards target
        transform.localPosition = Vector3.MoveTowards(transform.localPosition,
                                                      new Vector3(targetPosition, transform.localPosition.y, transform.localPosition.z),
                                                      moveSpeed * Time.deltaTime);

        //Check if fish reaches taregt
        if (Mathf.Approximately(transform.localPosition.x, targetPosition))
        {
            //choose new position
            targetPosition = Random.Range(maxLeft, maxRight);
        }

        //change direction randomly
        if (Random.value < changeFrequency)
        {
            movingRight = !movingRight;
            targetPosition = movingRight ? maxRight : maxLeft;
        }

    }
}
