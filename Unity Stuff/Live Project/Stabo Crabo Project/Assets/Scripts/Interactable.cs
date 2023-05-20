using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    //for controlling common functions of grabbable and stabbable objects


    [HideInInspector]
    public bool isDoomed = false; //is being destroyed
    private Rigidbody rb;

    public string storedTag; //tag that can be swapped in with a function. I don't like this and would prefer a method that takes a string paramet SetTag() but that doesnt work with my puzzle controllers atm

    public bool canBeStabbed = true;
    [SerializeField]
    private float stabForce = 1f;

    public bool canBeGrabbed = true;
    public GameObject heldBy; //which object is holding this one
    public bool isHeavy;

    //temp
    public GameObject indicator;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


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

    public virtual void Stabbed(Transform stabOrigin) //the base stab control send the object backwards
    {
        if(indicator) //if this has been assigned
        {
            Instantiate(indicator, stabOrigin.position, stabOrigin.rotation);
            Instantiate(indicator, transform.position, transform.rotation);
        }
        rb.isKinematic = false;
        if(heldBy && !isHeavy) //if the object is being held and is not heavy (so wont have a character join created)
        {
            transform.parent = GameObject.Find("_Props").transform; //reset parent
            heldBy = null; //is no longer being held
        }
        rb.AddForce((transform.position - stabOrigin.position).normalized * stabForce); //sends the object into the air according to force. Later it would be nice to have this be affected by the direction of the stab.
    }


}
