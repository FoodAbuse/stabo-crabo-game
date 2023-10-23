using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideController : MonoBehaviour
{
    private Rigidbody playerRB;
    public float force;

    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>()) //if the collider has a player script
        {
            playerRB = other.GetComponent<Rigidbody>(); //store the player's rigid body
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<PlayerController>()) //if the collider has a player script
        {
            playerRB = null; //remove the player script
        }
    }

    void Update()
    {
        if(playerRB) //if we are currently storing the player script
        {
            playerRB.AddForce(transform.forward * -1 * force); //pushe the rb down the slide
        }
        
    }
}
