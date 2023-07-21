using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour
{
    //This script controls the bubble's rotation while they are alive
    private Transform cam; //the scene's camera

    void Start()
    {
        cam = GameObject.Find("Main Camera").transform; //assign the camera
    }

    void Update()
    {
        transform.rotation = (Quaternion.LookRotation(cam.position - transform.position, Vector3.up)); //rotate to look at camera
    }
}
