using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCollection : MonoBehaviour
{
    public List<Collider> colList = new List<Collider>();
    public bool byTag; //whether we are grabbing based on tag
    public List<string> tagList = new List<string>();
    public bool anyInteractable; //we can fetch any interactable
    public bool toggleOutline;
    public Collider selected; //the currently selected collider out of all of the fetched ones


    void Update()
    {
        colList.RemoveAll(s => s == null); //remove all null items in list
    }

    //Script for collecting objects that are within collider bounds
    void OnTriggerEnter(Collider other)
    {
        if(byTag && tagList.Contains(other.tag)) //if it has a matching tag and has interactable script
        {
            colList.Add(other); //adds the collider to the list
            Select(other); //check priorities and select the highest item
        }
        else if(anyInteractable && other.GetComponent<Interactable>())
        {
            colList.Add(other); //adds the collider to the list
            Select(other); //check priorities and select the highest item
        }       
        
    }
    void OnTriggerExit(Collider other)
    {
        if(colList.Contains(other)) //if this is a collider we have stored
        {
            Remove(other); //remove it
        }
    }
    void Select(Collider col)
    {
        if(!selected) //if there is nothing selected at the moment
        {
            selected = col; //select this col automatically
            if(selected.GetComponent<Interactable>())
            {
                Outline(selected.GetComponent<Interactable>(), true); //enable outline on new selection
            }
        }
        else if(!col.GetComponent<Interactable>()) //if we don't have interactable, do not overtake
        {
            return;
        }
        else if(!selected.GetComponent<Interactable>()) //else if selected doesn't have intereactable, we do overtake
        {
            Outline(selected.GetComponent<Interactable>(), false); //disable outline on old selection
            selected = col;
            Outline(selected.GetComponent<Interactable>(), true);
        }
        else if(col.GetComponent<Interactable>().priority > selected.GetComponent<Interactable>().priority) //else if our priority is HIGHER, we do overtake
        {
            Outline(selected.GetComponent<Interactable>(), false);
            selected = col;
            Outline(selected.GetComponent<Interactable>(), true);
        }
        
    }
    void Outline(Interactable i, bool state)
    {
        if(toggleOutline) //continue with the action if 'toggle outline' is siwtched on
        {
            i.ToggleOutline(state);
        }
    }
    public void Remove(Collider col) //can be called by objects that are being destroyed
    {
        if(!colList.Contains(col)) //double checks we can actually remove item from the list
        {
            return;
        }
        colList.Remove(col); //removes the collider from the list
        if(selected == col) //if it was the selected item we need to determine a new one
        {
            Outline(selected.GetComponent<Interactable>(), false); //turn off the outline
            selected = null; //and remove the selection
            foreach(Collider c in colList) //run the selection algorithm on each item within our collected list
            {
                Select(c);
            }
        }
    }
}
