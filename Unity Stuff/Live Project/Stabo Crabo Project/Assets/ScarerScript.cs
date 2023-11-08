using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScarerScript : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> scaredNPCList; 

    [SerializeField]
    private List<GameObject> angryPoliceList;

    [SerializeField]
    private GameObject crab;

    void Awake()
    {        
        scaredNPCList.AddRange(GameObject.FindGameObjectsWithTag("NPC")); //For some reason this doesnt work? It was working before but seemingly broke >:( 

        foreach(GameObject thisNPC in scaredNPCList)
        {
            thisNPC.GetComponent<NPCController>().PanickMode(crab.transform);
            thisNPC.GetComponent<NPCController>().canBeStabbed = true;
        }

        foreach(GameObject thisLittlePiggy in angryPoliceList)
        {
            thisLittlePiggy.tag = "Killable";
            

            NPCController pigController = thisLittlePiggy.GetComponent<NPCController>();
            pigController.canBeStabbed = true;
            pigController.SetBehaviour(400);
            pigController.PlayAnimation("NPC_Idle");
            
        }

    }
}

