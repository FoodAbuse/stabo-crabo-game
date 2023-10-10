using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*ublic class PuzzleOutput
{
    public enum OutputType {CallFunction, Destroy, Instantiate, ToggleActive}
    public OutputType myOutputType;
    public string functionName;
    //public var functionVariable; //the variable to pass through the function
    public GameObject targetObject; //the object to be destroyed, created, toggled, or function called
    public Transform instantiatePosition; //where an instantiated object will spawn


}
*/



public class PuzzleController : MonoBehaviour
{
    //triggers and inputs
    public enum Trigger {OnStart = 0, ObjectTag = 100, HeldObject = 200, NotHeldObject = 250, AnyObject = 300, AllObjects = 400, Timed = 500, OnStab = 600}
    public Trigger myTrigger;
    public string lookForTag;
    public List<GameObject> lookForObjects;
    public float timerBase;
    public float timerVariance;
    private float currentTime = 0.0f;

    private List<GameObject> heldObjects; //used for keeping track of any target objects that are within bounds
    private GameObject triggerObject; //used to track which object in particular set this off
    //results and outputs
    public UnityEvent myOutput;
    [SerializeField]
    private List<GameObject> destroyObjects; //these objects will be destroyed as part of the output
    [SerializeField]
    private bool destroyTrigger; //add in whichever object triggered this script to be destroyed
    [SerializeField]
    private bool destroySelf = false; //adding a new bool to go as the very last function - in builds the object can destroy itself before it can finish all of its outcome tasks
    
    [SerializeField]
    private List<GameObject> spawnProps; //these objects will be created at this puzzle object's positon and rotation
    [SerializeField]
    private float spawnVelocity; //launches the spawned prop

    private bool inQueue; //when outcome has been called, but there is a delay

    public bool debugMode; //enable or disable Debug.Log

    void Start()
    {
        if(myTrigger == Trigger.OnStart) //immediately trigger this outcome 
        {
            Outcome();
        }
    }

    void Update()
    {
        if(inQueue) //while in queue, we tick down time to then launch outcome
        {
            if(debugMode){Debug.Log(currentTime);}
            if(currentTime <= 0) //if timer reachs 0
            {
                Outcome();
            }
            else
            {
                currentTime -= 1 * Time.deltaTime; //tick down the timer
            }
        }
        
        if(myTrigger == Trigger.Timed && !inQueue) //trigger outcome for regular timed everytime we are not in queue
        {
            Outcome();
        }
    }


    void OnTriggerEnter(Collider other) //triggering the input
    {
        //if(debugMode){Debug.Log(gameObject + " has triggered from: " + other.gameObject);}
        if(inQueue){return;}
        triggerObject = other.gameObject;
        //selection based off trigger types
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
                    if(other.GetComponent<PlayerController>().grabObject) //if the grab object has been assigned
                    {
                        if(lookForObjects.Contains(other.GetComponent<PlayerController>().grabObject.gameObject)) //player is holding any of the target objects
                        {
                            Outcome();
                        }
                    }
                }
                break;
            case Trigger.NotHeldObject:
                if(other.tag == "Player") //checks for player
                {
                    if(other.GetComponent<PlayerController>().grabObject) //if the grab object has been assigned
                    {
                        if(!lookForObjects.Contains(other.GetComponent<PlayerController>().grabObject.gameObject)) //player is not holding any of the target object
                        {
                            Outcome();
                        }
                    }
                    else
                    {
                        Outcome(); //if it has not been assigned, still execute 
                    }
                }
                break;
            case Trigger.AnyObject:
                if(lookForObjects.Contains(other.gameObject)) //if the game object matches the list of possible objects
                {
                    if(debugMode){Debug.Log(gameObject + " has triggered from: " + other.gameObject);}
                    Outcome();
                }
                break;
            case Trigger.AllObjects:
                if(lookForObjects.Contains(other.gameObject))
                {
                    heldObjects.Add(other.gameObject); //add this object to storage
                    if(MatchLists(lookForObjects, heldObjects)) //if the storage lists contains all the required objects
                    {
                        Outcome();
                    }
                }
                break;
        }
    }

    void OnTriggerExit (Collider other)
    {
        if(inQueue){return;}
        switch(myTrigger)
        {
            case Trigger.AllObjects:
                if(heldObjects.Contains(other.gameObject)) //if the exiting object was one of our stored objects
                {
                    heldObjects.Remove(other.gameObject); //remove it from the held list
                }
            break;
        }
       
    }

    private bool MatchLists(List<GameObject> L1, List<GameObject> L2) //check that all the elements of L1 are somewhere in L2
    {
        for(int i = 0; i < L1.Count; i++)
        {
            if(!L2.Contains(L1[i])) //if the L1 element is NOT anywhere in L2
            {
                return false; //the lists don't match
            }
        }
        return true;

    }

    public void StabTrigger() //if we are looking to be stabbed, trigger the outcome
    {
        if(inQueue){return;}
        if(myTrigger == Trigger.OnStab)
        {
            Outcome();
        }
    }

    public void Outcome() //called when trigger is succesful
    {
        if(debugMode){Debug.Log("Outcome");}
        if(!inQueue)
        {
            currentTime = Random.Range(timerBase - timerVariance, timerBase + timerVariance); //set new random time within range
            inQueue = true; //set this the first time Outcome is called, preventing re-calls during the delay
        }
        if(currentTime > 0.0f) //if timer hasn't reached 0, we are outcoming early and need to stop
        {
            return;
        }

        myOutput.Invoke(); //call all the output events

        foreach(var x in destroyObjects) //run through the destroy objects list and destroy everything in it
        {
            Destroy(x);
        }
        if(destroyTrigger)
        {
            if(debugMode){Debug.Log("Destroying Trigger " + triggerObject);}
            Destroy(triggerObject);
        }

        foreach(var x in spawnProps) //run through the spawn props list and spawn everything in it
        {
            GameObject spawned = Instantiate(x, transform.position, transform.rotation, GameObject.Find("_Props").transform);
            if(debugMode){Debug.Log(gameObject + " has spawned in " + spawned.gameObject);}
            if(spawned.GetComponent<Rigidbody>() && spawnVelocity > 0.0f) //launch the objects with spawn velocity
            {
              if(debugMode){Debug.Log("Launching...");}
              spawned.GetComponent<Rigidbody>().AddForce(transform.forward * spawnVelocity);
            }
        }

        // Hugo's addition - May be temporary but attempting to fix build issues
        if(destroySelf)
        {
            //Debug.Log(gameObject + "says: Destroying Myself. Well, its been real");
            Destroy(gameObject);
        }

        inQueue = false; //reset Queue ready for next time we want to trigger
    }
}
