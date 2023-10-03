using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCatcher : MonoBehaviour
{
    private Interactable intScript;
    private Rigidbody rb;
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Object Catcher caught " + other.gameObject + " and is resetting it's position.");

        if(!other.GetComponent<Rigidbody>())
        {
            Debug.Log("Object didn't have a rigid body component, returning...");
            return;
        }
        rb = other.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero; //set object's velocity to 0

        //set object's position to their default position
        if(other.GetComponent<Interactable>())
        {
            intScript = other.GetComponent<Interactable>();
            other.transform.position = intScript.preferredPos;
            other.transform.rotation = intScript.preferredRotation;
        }
        else if(other.GetComponent<PlayerController>())
        {
            GameObject.Find("LevelLoader").GetComponent<LevelLoader>().ReloadLevel(); //if somehow the player falls through the map, just relaod the level
        }
    }
}
