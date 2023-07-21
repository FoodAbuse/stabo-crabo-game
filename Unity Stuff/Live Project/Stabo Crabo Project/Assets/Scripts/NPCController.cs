using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //needed for navmesh
using UnityEngine.Animations.Rigging; //needed to aim the head

public class NPCController : Interactable
{
    //temporary script to move NPCs around randomly
    //for now I want to set a random desitnation within navmesh bounds, and then start counting down a timer once I reach it

    public NavMeshAgent agent; //the agent component on this NPC object
    public Collider destinationBounds; //area for random position to be generated in
    private NavMeshPath path; //create an empty navmesh path
    public Animator animator;
    private Rigidbody[] ragdollRigidbodies; //stores all the ragdoll RB components

    //movement stats
    [SerializeField]
    private float speedWalk;
    [SerializeField]
    private float speedRun;
    public float newDestTimeMin = 5.0f; //the time before the NPC looks for a new destination once reaching its previous destination'
    public float newDestTimeMax = 15.0f;
    private float countdownToNewDestination = 0.0f;
    //attacking stats
    [SerializeField]
    private float shoveCDBase; //what the atack cooldown resets to
    private float shoveCD; //the current attack cooldown
    [SerializeField]
    private float shoveForce = 5.0f; //how hard the player is shoved
    


    // Hugo's Chaos Injection - Murderous Traffic Cones
    //public bool WHAMMY = false;
    //private float timerKO;

    //carried object
    public GameObject heldObject; //for now held object doesn't move with the NPC. I am focussing on code to drop it from an idle pose
    [SerializeField]
    private GameObject identifier; //object to spawn over head to identify NPC
    private GameObject myIdentifier;
    [SerializeField]
    private GameObject speechBubble; //object to spawn over head to as speech bubble
    private GameObject mySpeechBubble;
    [SerializeField]
    private Transform pointAboveHead; //point for spawning speechbubbles, identifiers etc
    [SerializeField]
    private FieldOfView FOV; //FOV script reference

    //NPC behaviour variable
    public enum Behaviours {Idle, Roaming, Sleeping, Defending, Guarding, Dead}
    public enum States {Standing, Sitting, Laying, Walking, Chasing, Fleeing, Ragdoll, Pickup, Putdown, Attacking, ReturningObj} //these are things that an NPC can do based on what the behaviour dictates
    public Behaviours myBehaviour; //The Current behaviour of the NPC
    [SerializeField]
    private States myState;
    public bool initialSpeechBubble;

    //head aiming
    [SerializeField]
    private Collider fieldOfVision;
    [SerializeField]
    private Transform headTarget; //where the head looks at
    [SerializeField]
    private Rig headRig;
    [SerializeField]
    private ColliderCollection sightCollection;
    //hand aiming
    [SerializeField]
    private Transform handTarget; //where the hand moves to]
    [SerializeField]
    private Rig handRig;
    [SerializeField]
    private Transform handR; //the right hand

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

        if(initialSpeechBubble) Invoke("ToggleSpeechBubble",1.0f); //if initial speech bubble is set to true, toggle it upon starting
    }

    void Update()
    {
        //movement
        switch (myBehaviour)
        {
            case Behaviours.Defending:
                if(myState == States.Standing) //if we are just standing ie we have finished our defence action
                {
                    Roaming(); //wander around
                }
            break;
            case Behaviours.Roaming:
                Roaming();//wanders at a set interval
                break;
            default:
                break;
        }

        //head tilting and aiming
        if(sightCollection.colList.Count > 0) //checks that there is an object to look at
        {
            headRig.weight = Mathf.Lerp(headRig.weight, 1.0f, 3 * Time.deltaTime); //increase the weight of the rig
            HeadAim(sightCollection.colList[0].gameObject.transform); //aim the head
        }
        else if (headRig.weight > 0.01)
        {
            headRig.weight = Mathf.Lerp(headRig.weight, 0.0f, 3 * Time.deltaTime); //decrease the weight of the rig
        }

        if(shoveCD > 0.0f) //control Attack cooldown timer
        {
            shoveCD -= 1 * Time.deltaTime;
        }


        /*if (WHAMMY)
        {
            EnableRagdoll();
            timerKO += Time.deltaTime;

            if (timerKO >= 2f)
            {
                DisableRagdoll();
                timerKO = 0;
                WHAMMY = false;
            }
        }*/

        if(myBehaviour != Behaviours.Dead) //if we are not dead
        {
            if(FOV.canSeeTarget) //and the FOV script has triggered on a target that we can see
            {
                if(FOV.distanceToTarget <= 0.5f) //if we have reached our target
                {
                    if(FOV.target.tag == "Player") //if we were chasing the player
                    {
                        ShovePlayerStart();
                    }
                    else if(FOV.target.GetComponent<Interactable>().heldBy) //else if it was an object held by the player
                    {
                        if(FOV.target.GetComponent<Interactable>().heldBy.tag == "Player")
                        {
                            //player.drop object - but player does not get shoved
                            PickupStart(FOV.target);
                        }
                    }
                    else
                    {
                        PickupStart(FOV.target);
                    }
                }
                else //else if we have not yet reached our target
                {
                    agent.SetDestination(FOV.target.GetComponent<Collider>().ClosestPoint(transform.position)); //set navmesh destination to closts point in the target's collider
                    myState = States.Chasing;
                    Debug.Log(FOV.distanceToTarget);
                }

            }
            else if(myState == States.Chasing) //if we can't see our target anymore but for some reason we are still chasing
            {
                myState = States.Standing; //return to standing
            }

            
        }
        //behaviours based on current State
        switch(myState)
        {
            case States.Walking:
                agent.speed = speedWalk;
            break;
            case States.Chasing:
                agent.speed = speedRun;
            break;
            case States.Fleeing:
                agent.speed = speedRun;
            break;
            case States.Pickup:
                handTarget.position = FOV.target.GetComponent<Collider>().ClosestPoint(handR.position); //instantly move the hand target to the target's position
                handRig.weight = Mathf.Lerp(handRig.weight, 1.0f, 3 * Time.deltaTime); //increase the weight of the rig over time
            break;
            case States.ReturningObj:
                agent.SetDestination(heldObject.GetComponent<Interactable>().preferredPos); //set our destination to the objects preferred position
                handRig.weight = Mathf.Lerp(handRig.weight, 0.0f, 3 * Time.deltaTime); //decrease the weight of the rig over time
                if(Vector3.Distance(transform.position, heldObject.GetComponent<Interactable>().preferredPos) < 0.5f) //if we are near the position
                {
                    DropObject(); //drop it
                    myState = States.Standing;
                    handRig.weight = 0.0f; //completely remove the rig incase it hasnt decreased to 0 yet
                }
                agent.speed = speedWalk;
            break;
        }
    }

    private void PickupStart(Transform pTarget)
    {
        myState = States.Pickup;
        animator.Play("NPC_Pickup1"); //play the pickup animation
        heldObject = pTarget.gameObject;
    }

    public void PickupEnd()
    {
        heldObject.transform.parent = handR; //set the target as a child of our right hand
        heldObject.GetComponent<Interactable>().heldBy = this.gameObject;
        heldObject.GetComponent<Rigidbody>().isKinematic = true;
        myState = States.ReturningObj;
        FOV.WipeTarget();
    }

    private void ShovePlayerStart()
    {
        Debug.Log("ShoveCD: " + shoveCD);
        if(myState == States.Attacking || shoveCD > 0.0f){return;} //cancel attack if we are not already attacking or attack is on cooldown
        myState = States.Attacking;
        Debug.Log("Shoving Player");
        //play an animation
        //during the animation trigger the next function:
        ShovePlayerEnd();
    }

    private void ShovePlayerEnd()
    {
        myState = States.Standing; //return to standing state
        if(FOV.target.tag != "Player"){return;} //return if between the start and end of the attack player is no longer our target
        Debug.Log("still targeting player"); //FOV.target.position - transform.position - destinationBounds.ClosestPointOnBounds(FOV.target.position) - FOV.target.position
        Vector3 shoveVector = (FOV.target.position - destinationBounds.bounds.center).normalized * shoveForce;
        shoveVector.y = 0.2f * shoveForce; //give some vertical velocity
        FOV.target.GetComponent<Rigidbody>().AddForce(shoveVector); //set the player's velocity towards undefended area
        FOV.target.GetComponent<PlayerController>().stunned = 0.5f; //stun the plyer for a split second
        FOV.target.GetComponent<PlayerController>().DropObject(); //cause the player to drop things
        shoveCD = shoveCDBase; //reset our attack cooldown
    }

    void LateUpdate() //runs after update
    {
        float speed = agent.velocity.magnitude; //grabs the current agent vector's magnitude
        animator.SetFloat("Speed", speed); //set the animator parameter to match
    }

    private void Roaming() //waits, then sets a random destination and moves there - run during update
    {
        if(myState == States.Standing || myState == States.Sitting || myState == States.Laying) //if we are stationary
        {
            if(countdownToNewDestination <= 0.0f) //if countdown is finished
            {
                myState = States.Walking; //we are now walking
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
        else if(myState == States.Walking && agent.velocity.magnitude == 0.0f) //if we were walking, but we reached our destination and stopped
        {
            //insert a random chance to lay down or sit down
            myState = States.Standing; //for now just go direct to standing
        }
    }

    public void ChangeBehaviour(Behaviours newBehaviour)
    {
        myBehaviour = newBehaviour; //change to the new behaviour
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
        myState = States.Ragdoll; //set behaviour to ragdoll
        agent.enabled = false; //enable navmesh
    }

    void HeadAim(Transform lookAt) //tells the head to turn towards the lookAt target
    {
        headTarget.position = Vector3.Lerp(headTarget.position, lookAt.position, 5 * Time.deltaTime); //moves the head towards the target position
    }

    public override void Stabbed(Transform stabOrigin)
    {
        if(!canBeStabbed) //leave this function if the NPC cant be stabbed - would like to move this to a collider collection condition.
        {
            return;
        }

        DropObject(); //drop anything we are holding

        canBeStabbed = false;
        if(this.tag == "Killable")
        {
            EnableRagdoll();
            if(mySpeechBubble)
            {
                ToggleSpeechBubble();
            }
            if(transform.parent.name == "NPC Targets") //if this is a target
            {
                GameManager.TargetKilled(gameObject); //remove this from the targets list and trigger level phases etc
            }
        }
        else
        {
            animator.SetTrigger("Stabbed"); //play the hurt animation
            if(stabOrigin.position.x >= transform.position.x)
            {
                animator.SetFloat("PlayerDirection", 1.0f); //get this to be determined by if the player is on the right or left
            }
            else
            {
                animator.SetFloat("PlayerDirection", -1.0f); //get this to be determined by if the player is on the right or left
            }
            if(agent.enabled) //if the navmesh is active
            {
                agent.isStopped = true; //pause navmesh movement
            }

            //super specific case at the moment -  Ideally changing NPC behaviour off of stabs can be wrapped into the puzzle controllers
            /*if(myBehaviour == Behaviours.CarryingEsky)
            {
                myBehaviour = Behaviours.Roaming;
                agent.enabled = true;
                animator.SetFloat("IdleBehaviour",0.0f);
                countdownToNewDestination = newDestTimeMin; //reset the countdown
                ToggleSpeechBubble();
            }*/
        }        
    }

    public void ResumeIdle()
    {
        switch(myBehaviour)
        {
            /*case Behaviours.Sitting: //initial animations based on behaviour
                animator.SetFloat("IdleBehaviour",3.0f);
                break;
            case Behaviours.Lying:
                animator.Play("NPC_LyingDown");
                break;
            case Behaviours.Searching:
                animator.SetFloat("IdleBehaviour",1.0f);
                break;
            case Behaviours.Doorman:
                animator.SetFloat("IdleBehaviour",2.0f);
                break;
            case Behaviours.CarryingEsky:
                animator.SetFloat("IdleBehaviour",4.0f);
                break;*/
        }
            if(agent.enabled) //if the navmesh is active
            {
                agent.isStopped = false; //pause navmesh movement
            }
        canBeStabbed = true;
    }

    public void ToggleIdentify() //spawns a identifier object above the NPC
    {
        if(myIdentifier)
        {
            Destroy(myIdentifier);
        }
        else
        {
            myIdentifier = Instantiate(identifier, pointAboveHead.position, Quaternion.identity, pointAboveHead); //spawn the identifier prefab
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

    public void DropObject()
    {
        if(!heldObject) //if we are not holding anything
        {
            return;
        }
        heldObject.GetComponent<Rigidbody>().isKinematic = false;
        heldObject.GetComponent<Interactable>().heldBy = null;
        heldObject.transform.parent = GameObject.Find("_Props").transform;
        heldObject = null;
    }
}
