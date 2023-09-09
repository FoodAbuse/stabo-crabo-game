using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairSelection : MonoBehaviour
{
    public bool randomizeNPC = true;
    //public bool isFemale = false;

    public int randomColourRef;
    public int randomStyleRef;
    public int randomSkinRef;

    public GameObject hairBundleRef;
    public GameObject activeHair;
    public GameObject genderedBundle;

    // NPC Material Ref
    public Material activeSkin;

    // NPC Geo Ref
    public GameObject geoRef;

    // Female NPC Skins

    
    public Material femaleSkin1;
    public Material femaleSkin2;
    public Material femaleSkin3;

    // Male NPC Skins
    public Material maleSkin1;
    public Material maleSkin2;
    public Material maleSkin3;
    


    public enum NPC_gender {male, female};
    public NPC_gender gender;

    public enum NPC_HairColour {black, blonde, brown, grey, red}
    public NPC_HairColour hairColour;

    public enum NPC_HairStyle {style1, style2, style3, style4}




    void Awake()
    {
        activeSkin = geoRef.GetComponent<SkinnedMeshRenderer>().material;

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
        


        // Checks if NPC wants randomizeNPCd hair
        if (randomizeNPC)
        {
            Randomize();
        }
        
        if (!randomizeNPC && activeHair != null)
        {
            activeHair.GetComponent<MeshRenderer>().enabled = true;
            activeHair.tag = "ActiveHair";

        }


    }



    void Randomize()
    {
        // Randomizes Hair Colour
        randomColourRef = Random.Range(1, 6);
            //Debug.Log("Colour ref = " + randomColourRef);

        // Randomizes Hair Style
        randomStyleRef = Random.Range(0, 4);
        //Debug.Log("Style ref = " + randomStyleRef);

        // Randomizes NPC Skin
        randomSkinRef = Random.Range(1, 4);

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

        

        // Chooses skin based on gender
        if (gender == NPC_gender.male)
        {
            if (randomSkinRef == 1)
            {
                geoRef.GetComponent<SkinnedMeshRenderer>().material = maleSkin1;
            }
            if (randomSkinRef == 2)
            {
                geoRef.GetComponent<SkinnedMeshRenderer>().material = maleSkin2;
            }
            if (randomSkinRef == 3)
            {
                geoRef.GetComponent<SkinnedMeshRenderer>().material = maleSkin3;
            }

            //geoRef.GetComponent<SkinnedMeshRenderer>().materials[0] = activeSkin;
        }

        if (gender == NPC_gender.female)
        {
            if (randomSkinRef == 1)
            {
                geoRef.GetComponent<SkinnedMeshRenderer>().material = femaleSkin1;
            }
            if (randomSkinRef == 2)
            {
                geoRef.GetComponent<SkinnedMeshRenderer>().material = femaleSkin2;
            }
            if (randomSkinRef == 3)
            {
                geoRef.GetComponent<SkinnedMeshRenderer>().material = femaleSkin3;
            }

            
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
