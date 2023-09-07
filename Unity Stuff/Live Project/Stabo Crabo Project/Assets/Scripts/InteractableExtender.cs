using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableExtender : Interactable
{
    public Interactable npc; //the npc's controller script
    

    public override void Stabbed(Transform stabOrigin) //send the stab signal up to the main NPC controller
    {
        npc.Stabbed(stabOrigin); //transfer the stab
    }

   


}
