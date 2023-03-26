using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderListController : MonoBehaviour
{
    public List<GameObject> colList = new List<GameObject>();
    public List<string> tagList = new List<string>();

    //Script for collecting objects that are within collider bounds
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("collisions");
        if(tagList.Contains(other.tag))
        {
            colList.Add(other.gameObject); //adds the object to the list            
        }        
        
    }

    void OnTriggerExit(Collider other)
    {
        colList.Remove(other.gameObject); //removes the object from the list
    }
}
