using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    //this script will be applied to empty game objects in order to make puzzles work

    //type of trigger
    public enum Trigger {ObjectTag = 100, HeldObject = 200, Object = 300}
    public Trigger myTrigger;

    //required gameobject
    public string lookForTag;
    public GameObject lookForObject;

    //resulting action
    public enum Result {Animation = 100, NPCDestination = 200, CreateObject = 300}
    public Result myResult;

    //result variables
    public Animator animator;
    public string animTrigger;
    public Transform targetPoint; //used for spawning objects, or setting NPC destinations
    public NPCController NPC;
    public GameObject spawnPrefab;
    public List<GameObject> destroyObjects;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        //trigger type
        switch(myTrigger)
        {
            case Trigger.ObjectTag:
                if(other.tag == lookForTag) //if the tag matches
                {
                    Outcome();
                }
                break;
            case Trigger.HeldObject:
                if(other.tag == "Player") //checks for player
                {
                    Debug.Log("player found");
                    if(other.GetComponent<PlayerController>().grabObject.gameObject == lookForObject) //player is holding target object
                    {
                        Debug.Log("Object Found");
                        Outcome();
                    }
                }
                break;
            case Trigger.Object:
                if(other.gameObject == lookForObject) //if the game objects match
                {
                    Outcome();
                }
                break;
        }
    }

    void Outcome() //called when trigger is succesful
    {
        switch(myResult)
        {
            case Result.Animation:
                animator.SetTrigger(animTrigger); //play animation
                break;
            case Result.NPCDestination:
            Debug.Log("Setting Destination");
                NPC.agent.SetDestination(targetPoint.position); //set NPC navmesh destination
                break;
            case Result.CreateObject:
                Instantiate(spawnPrefab, targetPoint.position, targetPoint.rotation);//spawn a prefab
                break;
        }
        foreach(var x in destroyObjects) //run through the destroy objects list and destroy everything in it
        {
            Destroy(x);
        }
    }

}
