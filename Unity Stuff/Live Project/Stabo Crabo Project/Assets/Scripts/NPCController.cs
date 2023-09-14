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
    private float countdownToNewDestination = -1.0f;
    public float fleeTime = 5.0f; //how long NPC will flee for
    private Transform fleeFrom; //where the NPC is fleeing from
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
    private GameObject bubble; //prefab to spawn over head to as speech bubble
    private GameObject myBubble; //the instance that was created from the above prefab
    [SerializeField]
    private Transform pointAboveHead; //point for spawning hint bubbles, identifiers etc
    [SerializeField]
    private FieldOfView FOV; //FOV script reference

    //NPC behaviour variable
    public enum Behaviours {Idle = 100, Roaming = 200, Sleeping = 300, Defending = 400, Guarding = 500, Dead = 900}
    public enum States {Standing, Sitting, Lying, Walking, Chasing, Fleeing, Ragdoll, Pickup, Putdown, Attacking, ReturningObj, Swimming} //these are things that an NPC can do based on what the behaviour dictates
    public Behaviours myBehaviour; //The Current behaviour of the NPC
    [SerializeField]
    private States myState;

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
        preferredPos = transform.position; //objects initially prefer to be in their starting position. This can be changed later.
        path = new NavMeshPath(); //initialize the path

        headRig.weight = 0.0f; //initial rig weighting is 0

        //enter initial animation
        switch (myState)
        {
            case States.Standing:
                animator.Play("NPC_Idle");
                break;
            case States.Sitting:
                animator.SetFloat("IdleBehaviour", 3.0f);
                animator.Play("NPC_Idle");
                break;
            case States.Lying:
                animator.Play("NPC_LyingDown");
                break;
            case States.Swimming:
                animator.Play("NPC_DefaultSwimIdle");
                break;
        }

    }

    void Update()
    {
        //movement
        switch (myBehaviour)
        {
             case Behaviours.Idle: //when idle, countdown timer, and then return to preferred position
                OutOfPositionCheck(); //check if we are out, and return us if not
                break;
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
                            FOV.target.GetComponent<Interactable>().heldBy.GetComponent<PlayerController>().DropObject(); //cause the player to drop what they are holding
                            ShovePlayerStart(); //also shove the player
                            //assuming the NPC does not want to chase the player, their target should remain the same, and they should move to pick it up like normal
                        }
                    }
                    else
                    {
                        PickupStart(FOV.target);
                    }
                }
                else //else if we have not yet reached our target
                {
                    if(myBehaviour == Behaviours.Guarding && FOV.target.tag == "Player") //if we are guarding, and this is the player
                    {
                        Vector3 guardDest = destinationBounds.ClosestPoint(FOV.target.GetComponent<Collider>().ClosestPoint(transform.position));
                        agent.SetDestination(guardDest); //head to the closest point we can in our bounds                                              
                        Debug.DrawRay(transform.position, guardDest - transform.position, Color.green, 0.0f, false);
                        transform.rotation = Quaternion.LookRotation(FOV.target.GetComponent<Collider>().ClosestPoint(transform.position) - transform.position, Vector3.up); //always look in the direction of the drab as well
                    }
                    else
                    {
                        agent.SetDestination(FOV.target.GetComponent<Collider>().ClosestPoint(transform.position)); //go to closest point in the target's collider
                        
                    }
                    myState = States.Chasing;
                    BubbleOn(FOV.target.GetComponent<BubbleReference>().bubbleSprite); //activate a bubble above this NPC's head
                }

            }
            else if(myState == States.Chasing) //if we can't see our target anymore but for some reason we are still chasing
            {
                myState = States.Standing; //return to standing
                BubbleOff();
            }

            
        }
        //behaviours based on current State
        switch(myState)
        {
            case States.Walking:
                agent.speed = speedWalk;

                if(Vector3.Distance(transform.position, agent.destination) <= 0.1f) //if we are ever walking, and we reach our destination and stop walking...
                {
                    myState = States.Standing; //we are now standing
                }
                animator.SetFloat("WalkingBehaviour", 0.0f); //change to walking animation
            break;
            case States.Chasing:
                agent.speed = speedRun;
                animator.SetFloat("WalkingBehaviour", 0.5f); //change to running animation
            break;
            case States.Fleeing:
                agent.speed = speedRun;
                if(agent.enabled){agent.SetDestination(transform.position+(transform.position - fleeFrom.position).normalized*1.2f);} //set destination away from point of fear
                Debug.DrawRay(transform.position, agent.destination, Color.white, 0.0f, false);
                animator.SetFloat("WalkingBehaviour", 1.0f); //change to fleeing animation
            break;
            case States.Pickup:
                if(FOV.target)
                {
                    handTarget.position = FOV.target.GetComponent<Collider>().ClosestPoint(handR.position); //instantly move the hand target to the target's position
                } 
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
                animator.SetFloat("WalkingBehaviour", 0.0f); //change to walking animation
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
        heldObject.GetComponent<Interactable>().canBeStabbed = false; //player cannot interact with these an object held by an NPC
        heldObject.GetComponent<Interactable>().canBeGrabbed = false;
        heldObject.GetComponent<Rigidbody>().isKinematic = true;
        myState = States.ReturningObj;
        FOV.WipeTarget();
        BubbleOff();
    }

    private void ShovePlayerStart()
    {
        Debug.Log("ShoveCD: " + shoveCD);
        if(myState == States.Attacking || shoveCD > 0.0f) //cancel attack if we are not already attacking or attack is on cooldown
        {
            myState = States.Standing; //return to standing state
            return;
        } 
        myState = States.Attacking;
        Debug.Log("Shoving Player");
        animator.Play("NPC_Kick"); //play the kick animation
        //during the animation ShovePlayerEnd() is triggered
    }

    public void ShovePlayerEnd()
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
        BubbleOff();
    }

    void LateUpdate() //runs after update
    {
        float speed = agent.velocity.magnitude; //grabs the current agent vector's magnitude
        animator.SetFloat("Speed", speed); //set the animator parameter to match
    }

    private void Roaming() //waits, then sets a random destination and moves there - run during update
    {
        if(myState == States.Standing || myState == States.Sitting || myState == States.Lying) //if we are stationary
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
                myState = States.Walking; //we are now walking
            }
            else
            {
                countdownToNewDestination -= 1 * Time.deltaTime; //tick down countdown
            }
        }
    }

    public void SetBehaviour(int behaviourNumber)
    {
        myBehaviour = (Behaviours)behaviourNumber; //change to the new behaviour
        FOV.WipeTarget();
    }

    public void SetZone(Collider col)
    {
        destinationBounds = col; //assign a new zone wheer this NPC is allowed to roam
    }

    private Vector3 RandomPointInBounds(Bounds bounds) //picks a random location within a bounds
    {
        return new Vector3(
        Random.Range(bounds.min.x, bounds.max.x),
        0.0f,
        Random.Range(bounds.min.z, bounds.max.z)
    );
    }

    private void OutOfPositionCheck()
    {
        if(countdownToNewDestination <= -1.0f)
        {
            countdownToNewDestination = Random.Range(newDestTimeMin, newDestTimeMax); //reset the countdown initially
            return;
        }
        if(Vector3.Distance(transform.position, preferredPos) > 0.5f)//if we are out of position
        {
            if(countdownToNewDestination <= 0.0f) //if countdown is finished
            {
                agent.SetDestination(preferredPos); //return to starting destination
                countdownToNewDestination = Random.Range(newDestTimeMin, newDestTimeMax); //reset the countdown
                myState = States.Walking; //we are now walking
            }
            else
            {
                countdownToNewDestination -= 1 * Time.deltaTime; //tick down countdown
            }
        }
        else
        {
            myState = States.Standing;
        }
        
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

    public void EnableRagdoll()
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

        if (TryGetComponent<PuzzleController>(out PuzzleController pc))
        {
            pc.StabTrigger(); //sets the puzzle controller off if it was set to recieve stabs
        }

        DropObject(); //drop anything we are holding

        canBeStabbed = false;
        if(this.tag == "Killable")
        {
            EnableRagdoll();
            BubbleOff();
            if(transform.parent.name == "NPC Targets") //if this is a target
            {
                GameManager.TargetKilled(gameObject); //remove this from the targets list and trigger level phases etc
            }
        }
        else
        {
            float dir = Vector3.Dot(transform.right, stabOrigin.position - transform.position); //returns 1 if the vectors are the same direction, -1 if opposite, 0 if perpendicular
            //Debug.DrawRay(transform.position, transform.right * 10, Color.green, 15.0f, false);
            //Debug.DrawRay(transform.position, (stabOrigin.position- transform.position )*10, Color.red, 15.0f, false);
            if(dir >= 0.0f) //if Dot is the same direction, the player is on the left
            {
                animator.SetFloat("PlayerDirection", 1.0f); //right
            }
            else
            {
                animator.SetFloat("PlayerDirection", -1.0f); //left
            }
            if(agent.enabled) //if the navmesh is active
            {
                agent.isStopped = true; //pause navmesh movement
            }

            animator.SetTrigger("Stabbed"); //play the hurt animation
            StartCoroutine("Flee", stabOrigin); //after the stab animation the NPC will flee
        }        
    }

    public IEnumerator Flee(Transform crabPos)
    {
        if(fleeTime == 0.0f) //if the flee time is 0 they kick instead
        {
            yield return null;
        }
        fleeFrom = crabPos;
        myState = States.Fleeing;
        yield return new WaitForSeconds(fleeTime);
        canBeStabbed = true;
        agent.SetDestination(transform.position); //set destination to current pos stops movement
        myState = States.Standing;
        fleeFrom = null;

        


    }

    public void ResumeBehaviour()
    {

        if(agent.enabled) //if the navmesh is active
        {
            agent.isStopped = false; //resume navmesh movement
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

    public void BubbleOn(Sprite bubbleSprite) //creates a bubble above the NPC that shows an image
    {
        if(!myBubble) //if we don't yet have a bubble, make one
        {
            myBubble = Instantiate(bubble, pointAboveHead.position, Quaternion.Euler(0,0,0), pointAboveHead); //spawn the bubble prefab
        }
        myBubble.GetComponent<SpriteRenderer>().sprite = bubbleSprite; //set the image on the bubble
    }

    public void BubbleOff()
    {
        if(!myBubble){return;} //return if this was called when we don't have a bubble
        Destroy(myBubble);
    }

    public void DropObject()
    {
        if(!heldObject) //if we are not holding anything
        {
            return;
        }
        heldObject.GetComponent<Rigidbody>().isKinematic = false;
        heldObject.GetComponent<Interactable>().heldBy = null;
        heldObject.GetComponent<Interactable>().canBeStabbed = true; //re-enable player interaction with this object
        heldObject.GetComponent<Interactable>().canBeGrabbed = true;
        heldObject.transform.parent = GameObject.Find("_Props").transform;
        heldObject = null;

        if(myState == States.ReturningObj) //if we were returning an object at the time...
        {
            myState = States.Standing; //now we are just standing (can change this to fleeing if needed)
        }
    }

    public void PlayAnimation(string newAnim) //called from puzzle objects to trigger aniamtions
    {
        animator.Play(newAnim); //play the new animation
    }

    public void AddTarget(Interactable a)
    {
        FOV.targetRef.Add(a);
    }
}
