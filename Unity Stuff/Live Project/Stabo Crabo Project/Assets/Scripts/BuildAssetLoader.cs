using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildAssetLoader : MonoBehaviour
{
    // This script was made out of necessity as a test, when building the game some assets don't seem to load in the correct order especially when not set as active from Start

    void Awake()
    {

        gameObject.SetActive(false);

        if (gameObject.GetComponent<Rigidbody>() != null)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }

        
    }

   
}
