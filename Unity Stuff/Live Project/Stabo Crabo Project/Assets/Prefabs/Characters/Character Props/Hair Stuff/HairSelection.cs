using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairSelection : MonoBehaviour
{
    public bool randomHair = true;
    //public bool isFemale = false;

    public int randomColourRef;
    public int randomStyleRef;

    public GameObject hairBundleRef;
    public GameObject activeHair;
    public GameObject genderedBundle;


    public enum NPC_gender {male, female};
    public NPC_gender gender;

    public enum NPC_HairColour {black, blonde, brown, grey, red}
    public NPC_HairColour hairColour;

    public enum NPC_HairStyle {style1, style2, style3, style4}




    void Awake()
    {
        // Gender Randomization

        int genderCoinflip = Random.Range(1, 3);
        if (genderCoinflip == 1)
        {
            gender = NPC_gender.female;
        }
        else
        {
            gender = NPC_gender.male;
        }
        


        // Checks if NPC wants randomized hair
        if (randomHair)
        {
            RandomizeHair();
        }
        else
        {
            activeHair.GetComponent<MeshRenderer>().enabled = true;
            activeHair.tag = "ActiveHair";

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
            hairColour = NPC_HairColour.black;
            BlackHairRandomize();
        }

        // 2 = Blonde
        if (randomColourRef == 2)
        {
            hairColour = NPC_HairColour.blonde;
            BlondeHairRandomize();
        }

        // 3 = Brown
        if (randomColourRef == 3)
        {
            hairColour = NPC_HairColour.brown;
            BrownHairRandomize();
        }

        // 4 = Grey
        if (randomColourRef == 4)
        {
            hairColour = NPC_HairColour.grey;
            GreyHairRandomize();
        }

        // 5 = Red
        if (randomColourRef == 5)
        {
            hairColour = NPC_HairColour.red;
            RedHairRandomize();
        }

        
        
    }

    void BlackHairRandomize()
    {
        hairBundleRef = gameObject.transform.GetChild(0).gameObject;
        
        if (gender == NPC_gender.female)
        {
            // Selects only from female hair
            genderedBundle = hairBundleRef.transform.GetChild(0).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.GetComponent<MeshRenderer>().enabled = true;
            activeHair.tag = "ActiveHair";
        }
        else
        {
            // Selects only from male hair
            genderedBundle = hairBundleRef.transform.GetChild(1).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.GetComponent<MeshRenderer>().enabled = true;
            activeHair.tag = "ActiveHair";
        }
    }

    void BlondeHairRandomize()
    {
        hairBundleRef = gameObject.transform.GetChild(1).gameObject;

        if (gender == NPC_gender.female)
        {
            // Selects only from female hair
            genderedBundle = hairBundleRef.transform.GetChild(0).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.GetComponent<MeshRenderer>().enabled = true;
            activeHair.tag = "ActiveHair";
        }
        else
        {
            // Selects only from male hair
            genderedBundle = hairBundleRef.transform.GetChild(1).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.GetComponent<MeshRenderer>().enabled = true;
            activeHair.tag = "ActiveHair";
        }
    }

    void BrownHairRandomize()
    {
        hairBundleRef = gameObject.transform.GetChild(2).gameObject;

        if (gender == NPC_gender.female)
        {
            // Selects only from female hair
            genderedBundle = hairBundleRef.transform.GetChild(0).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.GetComponent<MeshRenderer>().enabled = true;
            activeHair.tag = "ActiveHair";
        }
        else
        {
            // Selects only from male hair
            genderedBundle = hairBundleRef.transform.GetChild(1).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.GetComponent<MeshRenderer>().enabled = true;
            activeHair.tag = "ActiveHair";
        }
    }

    void GreyHairRandomize()
    {
        hairBundleRef = gameObject.transform.GetChild(3).gameObject;

        if (gender == NPC_gender.female)
        {
            // Selects only from female hair
            genderedBundle = hairBundleRef.transform.GetChild(0).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.GetComponent<MeshRenderer>().enabled = true;
            activeHair.tag = "ActiveHair";
        }
        else
        {
            // Selects only from male hair
            genderedBundle = hairBundleRef.transform.GetChild(1).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.GetComponent<MeshRenderer>().enabled = true;
            activeHair.tag = "ActiveHair";
        }
    }

    void RedHairRandomize()
    {
        hairBundleRef = gameObject.transform.GetChild(4).gameObject;

        if (gender == NPC_gender.female)
        {
            // Selects only from female hair
            genderedBundle = hairBundleRef.transform.GetChild(0).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.GetComponent<MeshRenderer>().enabled = true;
            activeHair.tag = "ActiveHair";
        }
        else
        {
            // Selects only from male hair
            genderedBundle = hairBundleRef.transform.GetChild(1).gameObject;
            activeHair = genderedBundle.transform.GetChild(randomStyleRef).gameObject;

            activeHair.GetComponent<MeshRenderer>().enabled = true;
            activeHair.tag = "ActiveHair";
        }
    }
}
