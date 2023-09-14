using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCounter : MonoBehaviour
{
    public int maxObjects; //the max number of objects allowed
    public string tagCounted;

    public void CheckObjects()
    {
        GameObject[] objectsInScene = GameObject.FindGameObjectsWithTag(tagCounted); //finds all objecrts
        Debug.Log(objectsInScene.Length);
        if(objectsInScene.Length >= maxObjects)
        {
            Destroy(objectsInScene[0]); //destroy the first object in the list
        }
    }
}
