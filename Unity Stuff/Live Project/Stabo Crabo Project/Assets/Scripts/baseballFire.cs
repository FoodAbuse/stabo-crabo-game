using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseballFire : MonoBehaviour
{

    public GameObject baseballRef;
    public GameObject targetRef;

    public float fireForce = 500f;
    public Vector3 startPosition;
    private Interactable interactable; //this objects Interactable script
    public Collider invisWall; //the wall that is in the way at the edge of the map

    void Start()
    {
        startPosition = baseballRef.transform.position;
        interactable = GetComponent<Interactable>();
        Physics.IgnoreCollision(baseballRef.GetComponent<Collider>(), invisWall, true); //ignore the wall that is in the way
    }

    public void FireAtWill()
    {
        //Instantiate(baseballRef, new Vector3(13,-9,-159));

          
        baseballRef.SetActive(true); //then enable it
        Vector3 fireDirection = targetRef.transform.position - baseballRef.transform.position;


        baseballRef.GetComponent<Rigidbody>().isKinematic = false;
        baseballRef.GetComponent<Rigidbody>().AddForce(fireDirection * fireForce);
        Invoke("ResetBall", 3.0f); //reset the ball in x seconds
    }

    void ResetBall()
    {
        baseballRef.transform.position = startPosition; //move the baseball to the start position
        baseballRef.GetComponent<Rigidbody>().velocity = Vector3.zero;
        baseballRef.GetComponent<Rigidbody>().isKinematic = true;
        baseballRef.SetActive(false);
        interactable.ToggleInteraction(true); //re-enable interaction of the pitching machine
    }

    public void DisableInteraction() //this has to be here to be called by the animation because the normal one that takes a bool value does not show up in animation events
    {
        interactable.ToggleInteraction(false);
    }
}
