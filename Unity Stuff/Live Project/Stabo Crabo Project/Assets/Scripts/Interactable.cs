using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    //for controlling common functions of grabbable and stabbable objects
    //grabbable and stabbable scipts can inherit from this script potentially

    //variables for highlighting
    private new Renderer renderer; //this object's renderer.

    [HideInInspector]
    public bool isDoomed = false; //is being destroyed
    public GameObject heldBy; //which object is holding this one
    private Rigidbody rb;

    public string storedTag; //tag that can be swapped in with a function. I don't like this and would prefer a method that takes a string paramet SetTag() but that doesnt work with my puzzle controllers atm
    
    void Start()
    {
        renderer = GetComponent<Renderer>(); //initialise the renderer
        rb = GetComponent<Rigidbody>();
    }


    //doesnt seem to work right now because unlit material has no emission
    /*public void ToggleHighlight(bool toggle) //called to turn the highlight on or off
    {
        if(toggle) //if toggle parameter is true
        {
            renderer.material.EnableKeyword("_EMISSION"); //enable emission on the object
            renderer.material.SetColor("EmissionColor", Color.white); //set the emission color to white
        }
        else
        {
            renderer.material.DisableKeyword("_EMISSION"); //Disable emission on the object
        }
    }*/

    void OnDestroy() //is called when this object is destroyed
    {
        isDoomed = true;
        if(heldBy) //if this object is being held
        {
            Debug.Log("Calling Drop object");
            heldBy.SendMessage("DropObject"); //get the player to drop this object as its destroyed. USING SEND MESSAGE INCASE WE GET NPCS TO PICK THINGS UP
        }
    }

    public void ToggleKinematic() //called to change an interactable obj to kinematic or not
    {
        if(rb.isKinematic)
        {
            rb.isKinematic = false;
        }
        else
        {
            rb.isKinematic = true;
        }
    }

    public void SwapTag()
    {
        string tempTag = tag;
        tag = storedTag;
        storedTag = tempTag;
    }


}
