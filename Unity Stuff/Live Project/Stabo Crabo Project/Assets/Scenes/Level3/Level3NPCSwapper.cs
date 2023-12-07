using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3NPCSwapper : MonoBehaviour
{
    [SerializeField]
    private Renderer parentBody;

    [SerializeField]
    private Material parentMat;

    [SerializeField]
    private Renderer r;
    private FearfulManager fM;

    bool startupComplete;

    [SerializeField]
    HairSelection h;

    [SerializeField]
    int colourRef;
    [SerializeField]
    int styleRef;
    public void RecieveManager(FearfulManager f)
    {
        fM = f;
    }

    public void SetupSelf()
    {
        r = GetComponentInChildren<SkinnedMeshRenderer>();
        h = GetComponentInChildren<HairSelection>();
        //h.gameObject.SetActive(true);






        if (parentBody != null)
        {
            //HairSelection pH = parentBody.gameObject.GetComponentInChildren<HairSelection>();
            HairSelection pH = parentBody.gameObject.transform.parent.gameObject.transform.parent.GetComponentInChildren<HairSelection>();

            if (pH != null)
            {
                if (pH.randomizeNPC == false)
                {
                    Debug.Log("Houston, we have a problem : " + gameObject.name);
                }
                else
                {
                    //h.npcType = HairSelection.NPC_Type.standard;
                    colourRef = pH.randomColourRef;
                    styleRef = pH.randomStyleRef;
                    int Gender = 0;

                    if (pH.gender == HairSelection.NPC_gender.male) // 0 = F, 1 = M
                    {
                        Gender = 1;
                    }
                    h.SetToPreset(colourRef, styleRef, Gender);
                }
            }
                
            r.material = parentBody.material;
        }
        else
        {
            h.Randomize();

            r.material = parentMat;
        }
        fM.CompleteSetup();
    }

    public void SetParentSkin()
    {
        r.material = parentBody.material;
    }

}
