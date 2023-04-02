using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderListController : MonoBehaviour
{
    public List<Collider> colList = new List<Collider>();
    public List<string> tagList = new List<string>();

    //Script for collecting objects that are within collider bounds
    void OnTriggerEnter(Collider other)
    {
        if(tagList.Contains(other.tag))
        {
            colList.Add(other); //adds the collider to the list            
        }        
        
    }

    void OnTriggerExit(Collider other)
    {
        colList.Remove(other); //removes the collider from the list
    }
}
