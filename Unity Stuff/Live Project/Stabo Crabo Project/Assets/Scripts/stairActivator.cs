using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stairActivator : MonoBehaviour
{
    public GameObject activatedObject;
    public GameObject deletedObject;
    public Transform playerTransform;
    public Transform teleportLocation;
    public void ActivateSelf()
    {
        Debug.Log("Activated");
        activatedObject.SetActive(true);

        //playerTransform.position = teleportLocation.position;
        Destroy(deletedObject);
    }
}
