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

    void Start()
    {
        if(myTrigger == Trigger.OnStart) //immediately trigger this outcome 
        {
            Outcome();
        }
        currentTime = Random.Range(timerBase - timerVariance, timerBase + timerVariance); //initially set the timer to not be 0 (can just use on start for that) 
    }

    void Update()
    {
        switch(myTrigger)
        {
            case Trigger.Timed:
                if(currentTime <= 0) //if timer reachs 0
                {
                    Outcome();
                    currentTime = Random.Range(timerBase - timerVariance, timerBase + timerVariance); //set new random time within range
                }
                else
                {
                    currentTime -= 1 * Time.deltaTime; //tick down the timer
                }
            break;
        }
    }


    void OnTriggerEnter(Collider other) //triggering the input
    {
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
        if(myTrigger == Trigger.OnStab)
        {
            Outcome();
        }
    }

    public void Outcome() //called when trigger is succesful
    {
        myOutput.Invoke(); //call all the output events

        foreach(var x in destroyObjects) //run through the destroy objects list and destroy everything in it
        {
            Destroy(x);
        }
        if(destroyTrigger)
        {
            Debug.Log("Destroying Trigger " + triggerObject);
            Destroy(triggerObject);
        }

        foreach(var x in spawnProps) //run through the spawn props list and spawn everything in it
        {
            GameObject spawned = Instantiate(x, transform.position, transform.rotation, GameObject.Find("_Props").transform);
            if(spawned.GetComponent<Rigidbody>() && spawnVelocity > 0.0f) //launch the objects with spawn velocity
            {
              Debug.Log("Launching...");
              spawned.GetComponent<Rigidbody>().AddForce(transform.forward * spawnVelocity);
            }
        }
        if(destroyTrigger)
        {
            Debug.Log("Destroying Trigger " + triggerObject);
            Destroy(triggerObject);
        }

        // Hugo's addition - May be temporary but attempting to fix build issues
        if(destroySelf)
        {
            //Debug.Log(gameObject + "says: Destroying Myself. Well, its been real");
            Destroy(gameObject);
        }
    }
}
