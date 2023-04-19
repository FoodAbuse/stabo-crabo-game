using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    //this script will be applied to empty game objects in order to make puzzles work

    //type of trigger
    public enum Trigger {ObjectTag, HeldObject, Object}
    public Trigger myTrigger;

    //required gameobject
    public string lookForTag;
    public GameObject lookForObject;

    //resulting action
    public enum Result {Animation, NPCDestination, CreateObject}
    public Result myResult;

    //result variables
    public Animator resultingAnimator;
    public string animTriggerName;
    public Transform targetPoint; //used for spawning objects, or setting NPC destinations
    public NPCController affectedNPC;
    public GameObject spawnPrefab;

    void OnTriggerEnter(Collider other)
    {
        //trigger type
        switch(myTrigger)
        {
            case Trigger.ObjectTag:
                //look for tag
                break;
            case Trigger.HeldObject:
                //look for object and its parent / and its child or smt
                break;
            case Trigger.Object:
                //look for matching gameoject
                break;
        }
    }

    void Outcome() //called when trigger is succesful
    {
        switch(myResult)
        {
            case Result.Animation:
                //play animation
                break;
            case Result.NPCDestination:
                //set NPC navmesh destination
                break;
            case Result.CreateObject:
                //spawn a prefab
                break;
        }
    }

}
