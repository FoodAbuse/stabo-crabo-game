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
    
    void Start()
    {
        renderer = GetComponent<Renderer>(); //initialise the renderer
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
        if(transform.root.tag == "Player") // if this object is currently a child of the player i.e. is being grabbed
        {
            Debug.Log("Calling Drop object");
            transform.root.GetComponent<PlayerController>().DropObject(); //get the player to drop this object as its destroyed
        }
    }


}
