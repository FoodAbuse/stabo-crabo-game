using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCollection : MonoBehaviour
{
    public List<Collider> colList = new List<Collider>();
    public List<string> tagList = new List<string>();

    //Script for collecting objects that are within collider bounds
    void OnTriggerEnter(Collider other)
    {
        if(tagList.Contains(other.tag))
        {
            colList.Add(other); //adds the collider to the list
            //other.GetComponent<Interactable>().ToggleHighlight(true); //toggle the object's highlight
        }        
        
    }

    void OnTriggerExit(Collider other)
    {
        if(tagList.Contains(other.tag))
        {
            colList.Remove(other); //removes the collider from the list
            //other.GetComponent<Interactable>().ToggleHighlight(false); //toggle the object's highlight
        }
    }
}
