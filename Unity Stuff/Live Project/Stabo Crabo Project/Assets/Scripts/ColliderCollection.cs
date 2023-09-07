using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCollection : MonoBehaviour
{
    public List<Collider> colList = new List<Collider>();
    public List<string> tagList = new List<string>();
    public bool toggleOutline;

    void Update()
    {
        colList.RemoveAll(s => s == null); //remove all null items in list
    }

    //Script for collecting objects that are within collider bounds
    void OnTriggerEnter(Collider other)
    {
        if(tagList.Contains(other.tag)) //if it has a matching tag and has interactable script
        {
            colList.Add(other); //adds the collider to the list
            if(toggleOutline && other.GetComponent<Interactable>()) //if we want an outline, and the object is interactable
            {
                colList[0].GetComponent<Interactable>().ToggleOutline(true); //turns on the outline of the first object in the list
            }
        }        
        
    }

    void OnTriggerExit(Collider other)
    {
        if(tagList.Contains(other.tag) && other.GetComponent<Interactable>())
        {
            colList.Remove(other); //removes the collider from the list
            if(toggleOutline)
            {
                other.GetComponent<Interactable>().ToggleOutline(false); //turns off the outline of the object leaving
                if(colList.Count > 0)
                {
                    colList[0].GetComponent<Interactable>().ToggleOutline(true); //turns on the outline of the first object in the list - no change if 'other' was not colList[0]
                }
            }
        }
    }

    public void Remove(Collider self) //called by objects to manually remove themselves from the list
    {
        if(tagList.Contains(self.tag) && self.GetComponent<Interactable>())
        {
            colList.Remove(self); //removes the collider from the list
            if(toggleOutline)
            {
                self.GetComponent<Interactable>().ToggleOutline(false); //turns off the outline of the object leaving
                if(colList.Count > 0)
                {
                    colList[0].GetComponent<Interactable>().ToggleOutline(true); //turns on the outline of the first object in the list - no change if 'self' was not colList[0]
                }
            }
        }
    }
}
