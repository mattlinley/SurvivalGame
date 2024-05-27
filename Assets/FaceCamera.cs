using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform localTrans;

   // public Camera facingCamera;

    private void Start()
    {
        localTrans = GetComponent<Transform>();
    }

    private void Update()
    {
        if (Camera.main)
        {
            localTrans.LookAt(2 * localTrans.position - Camera.main.transform.position);
        }
    }
}
