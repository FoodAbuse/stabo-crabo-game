using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //needed for navmesh
using UnityEngine.Animations.Rigging; //needed to aim the head

public class NPCController : Stabbable
{
    //temporary script to move NPCs around randomly
    //for now I want to set a random desitnation within navmesh bounds, and then start counting down a timer once I reach it

    public NavMeshAgent agent; //the agent component on this NPC object
    public Collider destinationBounds; //area for random position to be generated in
    private NavMeshPath path; //create an empty navmesh path
    public Animator animator;
    private Rigidbody[] ragdollRigidbodies; //stores all the ragdoll RB components

    public float newDestTimeMin = 5.0f; //the time before the NPC looks for a new destination once reaching its previous destination'
    public float newDestTimeMax = 15.0f;
    private float countdownToNewDestination = 0.0f;

    //carried object
    [SerializeField]
    private GameObject heldObject; //for now held object doesn't move with the NPC. I am focussing on code to drop it from an idle pose
    [SerializeField]
    private GameObject identifier; //object to spawn over head to identify NPC
    private GameObject myIdentifier;
    [SerializeField]
    private GameObject speechBubble; //object to spawn over head to as speech bubble
    private GameObject mySpeechBubble;
    [SerializeField]
    private Transform pointAboveHead; //point for spawning speechbubbles, identifiers etc

    //NPC behaviour variable
    public enum Behaviours {Idle = 100, Sitting = 110, Lying = 120, Searching = 130, Doorman = 140, CarryingEsky = 150, Chasing = 200, Fleeing = 300, Roaming = 400, Ragdoll = 900}
    public Behaviours behaviour; //The Current behaviour of the NPC

    //head aiming
    [SerializeField]
    private Collider fieldOfVision;
    [SerializeField]
    private Transform headTarget;
    [SerializeField]
    private Rig headRig;
    [SerializeField]
    private ColliderCollection sightCollection;

    void Awake()
    {
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>(); //get all the RB in the child limbs and joints
        DisableRagdoll(); //the ragdoll should start disabled
    }
    void Start()
    {
        path = new NavMeshPath(); //initialize the path
        ResumeIdle(); //return to an idle animation based on the current behaviour

        headRig.weight = 0.0f; //initial rig weighting is 0


    }

    void Update()
    {
        switch (behaviour)
        {
            case Behaviours.Roaming:
                Roaming();//wanders at a set interval
                break;
            default:
                break;
        }

        if(sightCollection.colList.Count > 0) //checks that there is an object to look at
        {
            headRig.weight = Mathf.Lerp(headRig.weight, 1.0f, 3 * Time.deltaTime); //increase the weight of the rig
            HeadAim(sightCollection.colList[0].gameObject.transform); //aim the head
        }
        else if (headRig.weight > 0.01)
        {
            headRig.weight = Mathf.Lerp(headRig.weight, 0.0f, 3 * Time.deltaTime); //decrease the weight of the rig
        }


    }

    void LateUpdate() //runs after update
    {
        float speed = agent.velocity.magnitude; //grabs the current agent vector's magnitude
        animator.SetFloat("Speed", speed); //set the animator parameter to match
    }

    private void Roaming() //waits, then sets a random destination and moves there - run during update
    {
        if(agent.velocity.magnitude == 0.0f) //if we are stationary
        {
            if(countdownToNewDestination <= 0.0f) //if countdown is finished
            {
                Vector3 destination = RandomPointInBounds(destinationBounds.bounds); //generate a random position within the bounds
                agent.CalculatePath(destination, path); //calculates the path
                while(path.status != NavMeshPathStatus.PathComplete) //calculates the path and then checks if it can reach the destination
                {
                    destination = RandomPointInBounds(destinationBounds.bounds); //generate a new random point
                    agent.CalculatePath(destination, path); //recalculate the path            
                }
                agent.SetDestination(destination); //set new destination
                countdownToNewDestination = Random.Range(newDestTimeMin, newDestTimeMax); //reset the countdown
            }
            else
            {
                countdownToNewDestination -= 1 * Time.deltaTime; //tick down countdown
            }
        }
    }

    private Vector3 RandomPointInBounds(Bounds bounds) //picks a random location within a bounds
    {
        return new Vector3(
        Random.Range(bounds.min.x, bounds.max.x),
        0.0f,
        Random.Range(bounds.min.z, bounds.max.z)
    );
    }

    private void DisableRagdoll() //disables ragdoll should be accompanied by a line to set behaviour to idle / roaming etc.
    {
        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = true; //set all the RB to be kinematic
        }
        animator.enabled = true;
        //agent.enabled = true; //enable navmesh - I don't want the target's navmesh to be reenabled atm
    }

    private void EnableRagdoll()
    {
        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = false; //set all the RB to be affected by physics
        }
        animator.enabled = false; //turn off the animator
        behaviour = Behaviours.Ragdoll; //set behaviour to ragdoll
        agent.enabled = false; //enable navmesh
    }

    void HeadAim(Transform lookAt) //tells the head to turn towards the lookAt target
    {
        headTarget.position = Vector3.Lerp(headTarget.position, lookAt.position, 5 * Time.deltaTime); //moves the head towards the target position
    }

    public override void Stabbed()
    {
        if(this.tag == "Killable")
        {
            EnableRagdoll();
            //also probably need to disable various tags and methods and things
            if(transform.parent.name == "NPC Targets") //if this is a target
            {
                GameManager.TargetKilled(gameObject); //remove this from the targets list and trigger level phases etc
            }
        }
        else
        {
            animator.Play("NPC_EmoteHurt"); //play the hurt animation
            if(agent.enabled) //if the navmesh is active
            {
                agent.isStopped = true; //pause navmesh movement
            }
        }

        if(heldObject) //if there is a held object
        {
            heldObject.GetComponent<Rigidbody>().isKinematic = false; //drop the object
        }
        
    }

    public void ResumeIdle()
    {
        switch(behaviour)
        {
            case Behaviours.Sitting: //initial animations based on behaviour
                animator.SetFloat("IdleBehaviour",4.0f);
                break;
            case Behaviours.Lying:
                animator.SetFloat("IdleBehaviour",2.0f);
                break;
            case Behaviours.Searching:
                animator.SetFloat("IdleBehaviour",1.0f);
                break;
            case Behaviours.Doorman:
                animator.SetFloat("IdleBehaviour",3.0f);
                break;
            case Behaviours.CarryingEsky:
                animator.SetFloat("IdleBehaviour",5.0f);
                break;
        }
            if(agent.enabled) //if the navmesh is active
            {
                agent.isStopped = false; //pause navmesh movement
            }
    }

    public void ToggleIdentify() //spawns a identifier object above the NPC
    {
        if(myIdentifier)
        {
            Destroy(myIdentifier);
        }
        else
        {
            myIdentifier = Instantiate(identifier, pointAboveHead.position, Quaternion.identity, transform); //spawn the identifier prefab
        }

    }

    public void ToggleSpeechBubble() //spawns a identifier object above the NPC
    {
        if(mySpeechBubble)
        {
            Destroy(mySpeechBubble);
        }
        else
        {
            //Position needs to face camera THIS HAS NOT BEEN IMPLEMENTED
            mySpeechBubble = Instantiate(speechBubble, pointAboveHead.position, Quaternion.Euler(0,0,0), transform); //spawn the identifier prefab
        }

    }
}
