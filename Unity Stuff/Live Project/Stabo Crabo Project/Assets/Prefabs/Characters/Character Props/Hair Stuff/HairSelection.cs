using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairSelection : MonoBehaviour
{
    public bool randomHair = true;
    public bool isFemale;

    public int randomColourRef;
    public int randomStyleRef;

    public GameObject hairBundleRef;
    public GameObject activeHair;
    public GameObject genderedBundle;


    void Awake()
    {
        // temporary - randomizes gender
        int genderCoinflip = Random.Range(1, 3);
        if (genderCoinflip == 1)
        {
            isFemale = true;
        }
        else
        {
            isFemale = false;
        }


        // Checks if NPC wants randomized hair
        if (randomHair)
        {
            RandomizeHair();
        }
    }



    void RandomizeHair()
    {
        // Randomizes Hair Colour
        randomColourRef = Random.Range(1, 6);
            //Debug.Log("Colour ref = " + randomColourRef);

        // Randomizes Hair Style
        randomStyleRef = Random.Range(0, 4);
            //Debug.Log("Style ref = " + randomStyleRef);

        // 1 = Black
        if (randomColourRef == 1)
        {
            BlackHairRandomize();
        }

        // 2 = Blonde
        if (randomColourRef == 2)
        {
            BlondeHairRandomize();
        }

        // 3 = Brown
        if (randomColourRef == 3)
        {
            BrownHairRandomize();
        }

        // 4 = Grey
        if (randomColourRef == 4)
        {
            GreyHairRandomize();
        }

        // 5 = Red
        if (randomColourRef == 5)
        {
            RedHairRandomize();
        }

    }

    void BlackHairRandomize()
    {
        hairBundleRef = gameObject.transform.GetChild(0).gameObject;
        
        if (isFemale == true)
        {
            // Selects only from female hair
            genderedBundle = hairBundleRef.transform.GetChild(0).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.SetActive(true);
        }
        else
        {
            // Selects only from male hair
            genderedBundle = hairBundleRef.transform.GetChild(1).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.SetActive(true);
        }
    }

    void BlondeHairRandomize()
    {
        hairBundleRef = gameObject.transform.GetChild(1).gameObject;

        if (isFemale == true)
        {
            // Selects only from female hair
            genderedBundle = hairBundleRef.transform.GetChild(0).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.SetActive(true);
        }
        else
        {
            // Selects only from male hair
            genderedBundle = hairBundleRef.transform.GetChild(1).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.SetActive(true);
        }
    }

    void BrownHairRandomize()
    {
        hairBundleRef = gameObject.transform.GetChild(2).gameObject;

        if (isFemale == true)
        {
            // Selects only from female hair
            genderedBundle = hairBundleRef.transform.GetChild(0).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.SetActive(true);
        }
        else
        {
            // Selects only from male hair
            genderedBundle = hairBundleRef.transform.GetChild(1).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.SetActive(true);
        }
    }

    void GreyHairRandomize()
    {
        hairBundleRef = gameObject.transform.GetChild(3).gameObject;

        if (isFemale == true)
        {
            // Selects only from female hair
            genderedBundle = hairBundleRef.transform.GetChild(0).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.SetActive(true);
        }
        else
        {
            // Selects only from male hair
            genderedBundle = hairBundleRef.transform.GetChild(1).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.SetActive(true);
        }
    }

    void RedHairRandomize()
    {
        hairBundleRef = gameObject.transform.GetChild(4).gameObject;

        if (isFemale == true)
        {
            // Selects only from female hair
            genderedBundle = hairBundleRef.transform.GetChild(0).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.SetActive(true);
        }
        else
        {
            // Selects only from male hair
            genderedBundle = hairBundleRef.transform.GetChild(1).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.SetActive(true);
        }
    }
}
