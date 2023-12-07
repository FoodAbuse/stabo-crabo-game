using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairSelection : MonoBehaviour
{

    public enum NPC_Type{standard, child, police, unique};
    public NPC_Type npcType;
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

    public Material childSkin1;
    public Material childSkin2;
    public Material childSkin3;

    public Material policeSkin1;
    public Material policeSkin2;
    public Material policeSkin3;
    


    public enum NPC_gender {male, female};
    public NPC_gender gender;

    public enum NPC_HairColour {black, blonde, brown, grey, red}
    public NPC_HairColour hairColour;

    public enum NPC_HairStyle {style1, style2, style3, style4}

    public int timesLooped = 0;
    [SerializeField]
    private GameObject problemHair;



    void Awake()
    {
        //activeSkin = geoRef.GetComponent<SkinnedMeshRenderer>().material;

        if (npcType == NPC_Type.standard || npcType == NPC_Type.child)
        {
           // Gender Randomization for standard NPCs and Child NPCs
            int genderCoinflip = Random.Range(1, 3);
            if (genderCoinflip == 1)
            {
                gender = NPC_gender.female;
            }
            else
            {
                gender = NPC_gender.male;
            }

            // Checks to see if NPC is a default NPC in order to randomize
            if (npcType == NPC_Type.standard)
            {
                randomizeNPC = true;
            }

            else 
            {
                randomizeNPC = false;
                randomizeChild();
            }
        }

        if (npcType == NPC_Type.police)
        {
            RandomizePolice();
        }

        
        


        // Checks if NPC wants randomized hair and textures
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



    public void Randomize()
    {

        timesLooped++;
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
                geoRef.GetComponent<SkinnedMeshRenderer>().sharedMaterial = maleSkin1;
            }
            if (randomSkinRef == 2)
            {
                geoRef.GetComponent<SkinnedMeshRenderer>().sharedMaterial = maleSkin2;
            }
            if (randomSkinRef == 3)
            {
                geoRef.GetComponent<SkinnedMeshRenderer>().sharedMaterial = maleSkin3;
            }

            activeSkin = geoRef.GetComponent<SkinnedMeshRenderer>().material;
        }

        if (gender == NPC_gender.female)
        {
            if (randomSkinRef == 1)
            {
                geoRef.GetComponent<SkinnedMeshRenderer>().sharedMaterial = femaleSkin1;
            }
            if (randomSkinRef == 2)
            {
                geoRef.GetComponent<SkinnedMeshRenderer>().sharedMaterial = femaleSkin2;
            }
            if (randomSkinRef == 3)
            {
                geoRef.GetComponent<SkinnedMeshRenderer>().sharedMaterial = femaleSkin3;
            }

            activeSkin = geoRef.GetComponent<SkinnedMeshRenderer>().material;
        }

        if (problemHair != null)
        {
            ValidateProblemHair();
        }
    }

    void randomizeChild()
    {
        // Sets the hairstyle to be randomized between those appropriate for the child NPCs
        if (gender == NPC_gender.female)
        {
            randomStyleRef = Random.Range(0,3);

            if (randomStyleRef > 0)
            {
                // Corrects for the random female hair referencing child 1 (second in sequence, Hat Hair) and adds one to the int to offset position
                randomStyleRef++;
            }
        }
        else
        {
            randomStyleRef = Random.Range(1,3);
        }

        randomSkinRef = Random.Range(1,4);
        randomColourRef = Random.Range(1,5);


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

        // 4 = Red
        if (randomColourRef == 4)
        {
            hairColour = NPC_HairColour.red;
            RedHairRandomize();
        }


        // Sets Skin from ChildNPC Skins
            if (randomSkinRef == 1)
            {
                geoRef.GetComponent<SkinnedMeshRenderer>().sharedMaterial = childSkin1;
            }
            if (randomSkinRef == 2)
            {
                geoRef.GetComponent<SkinnedMeshRenderer>().sharedMaterial = childSkin2;
            }
            if (randomSkinRef == 3)
            {
                geoRef.GetComponent<SkinnedMeshRenderer>().sharedMaterial = childSkin3;
            }


        activeSkin = geoRef.GetComponent<SkinnedMeshRenderer>().material;
    }

    void RandomizePolice ()
    {
        randomSkinRef = Random.Range(1, 4);

        if (randomSkinRef == 1)
            {
                geoRef.GetComponent<SkinnedMeshRenderer>().sharedMaterial = policeSkin1;
            }

        if (randomSkinRef == 2)
            {
                geoRef.GetComponent<SkinnedMeshRenderer>().sharedMaterial = policeSkin2;
            }

        if (randomSkinRef == 3)
            {
                geoRef.GetComponent<SkinnedMeshRenderer>().sharedMaterial = policeSkin3;
            }
        activeSkin = geoRef.GetComponent<SkinnedMeshRenderer>().material;
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
        timesLooped++;
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
        timesLooped++;
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

    public void SetToPreset(int colour, int style, int genderRef)
    {
        if (genderRef == 0) //we are f
        {
            gender = NPC_gender.female;
        }
        else //we are m
        {
            gender = NPC_gender.male;
        }

        randomStyleRef = style;

        if (colour == 1)
        {
            hairColour = NPC_HairColour.black;
            BlackHairRandomize();
        }
        // 2 = Blonde
        else if (colour == 2)
        {
            hairColour = NPC_HairColour.blonde;
            BlondeHairRandomize();
        }

        // 3 = Brown
        if (colour == 3)
        {
            hairColour = NPC_HairColour.brown;
            BrownHairRandomize();
        }

        // 4 = Grey
        if (colour == 4)
        {
            hairColour = NPC_HairColour.grey;
            GreyHairRandomize();
        }

        // 5 = Red
        if (colour == 5)
        {
            hairColour = NPC_HairColour.red;
            RedHairRandomize();
        }

        ValidateProblemHair();
    }

    public void ValidateProblemHair()
    {
        if (problemHair != activeHair)
        {
            Destroy(problemHair);
        }
    }
}


