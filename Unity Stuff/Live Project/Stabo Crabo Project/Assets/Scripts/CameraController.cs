using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform; //reference to the player's position

    private Vector3 positionOffset; //the offest of the camera

    void Start()
    {
        positionOffset = transform.position - playerTransform.position; //set the offset to be equal to the current offest when the scene starts
        
    }

    void Update()
    {
        transform.position = playerTransform.position + positionOffset; //maintain the offset every frame
        //this will be jumpy, but in theory if the player's movement is smooth, the camera movement will be equally smooth
        //later it may be good to add smoothing + a delay to camera movement
    }
}
