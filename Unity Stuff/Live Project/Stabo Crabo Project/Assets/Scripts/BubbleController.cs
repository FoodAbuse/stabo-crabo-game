using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour
{
    //This script controls the bubble's rotation while they are alive
    private Transform cam; //the scene's camera
    public Transform anchor;
    public float offset;

    void Start()
    {
        cam = GameObject.Find("Main Camera").transform; //assign the camera
    }

    void Update()
    {
        transform.rotation = (Quaternion.LookRotation(cam.position - transform.position, Vector3.up)); //rotate to look at camera
        transform.position = new Vector3(anchor.position.x, anchor.position.y + offset, anchor.position.z);
    }
}
