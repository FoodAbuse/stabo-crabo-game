using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //Defining variables
    public float baseMoveSpeed = 2.5f;
    public float sprintMoveSpeed = 5.0f;
    public float baseTurnSpeed = 2.0f;
    public float sprintTurnSpeed = 4.0f;
    public float stabCoolDown = 1.0f; //how fast can stab be spammed
    private float stabTimer; //variable used to time stabs

    private float moveSpeed;
    private float turnSpeed;
    private bool isSprinting = false;
    private bool isGrabbing = false;
    private bool isDragging = false;


    //private Transform grabParent; //commented out because so far everything dropped should go under _prop afterwards
    [HideInInspector]
    public Transform grabObject;
    [HideInInspector]
    public Transform stabObject;
    [SerializeField]
    private Transform stabAirTarget; //where the stab will go if there is no other target
     [SerializeField]
    private Transform grabAirTarget; //where the stab will go if there is no other target
    [SerializeField]
    private Transform grabLightTarget; //where the arm will go when holding a light object

    //rig animation
    public Transform armTargetL; //target to verride L arm animation
    private Vector3 startPosArmTargetL;
    public Transform armTargetR; //target to verride R arm animation
    private Vector3 startPosArmTargetR;
    
    //Defining Components
    public ColliderCollection grabCollider;
    public ColliderCollection stabCollider;
    public Animator crabAnimator;

    private Rigidbody rb;
    private Vector3 moveDirection = Vector3.zero;
    private Quaternion targetRotation;


    void Start()
    {
        //Fetch component references
        rb = GetComponent<Rigidbody>();

        //set initial moveSpeeds
        moveSpeed = baseMoveSpeed;
        turnSpeed = baseTurnSpeed;

        //store initial Lhand Position
        startPosArmTargetL = armTargetL.localPosition; //the initial position for the L hand target
        startPosArmTargetR = armTargetR.localPosition; //the initial position for the L hand target
    }

    void Update()
    {
        if(GameManager.acceptPlayerInput) //the following functions require player input via keyboard or mouse and can be switched off
        {
            GrabCheck(); //check for grab input and execute it
            if(stabTimer <= 0) //if the cool down has been exhausted
            {
                StabCheck(); //check for stab input and execute it
            }
            else
            {
                stabTimer -= 1 * Time.deltaTime; //keep cooling-down the stab timer
            }
        }
        
    }

    void Movement() //takes player input to move the player character
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical")).normalized; //receive input for movement vector
            if(Input.GetKey(KeyCode.LeftShift) && moveDirection.magnitude > 0.1 && !isDragging) //sprint button on and we are moving, and we are not dragging something
            {
                isSprinting = true;
                moveSpeed = sprintMoveSpeed; //increase movement
                turnSpeed = sprintTurnSpeed;
            }
            else
            {
                isSprinting = false;
                moveSpeed = baseMoveSpeed; //reset movement
                turnSpeed = baseTurnSpeed;                
            }
    }

    void FixedUpdate()
    {
        if(GameManager.acceptPlayerInput) //the following functions require player input via keyboard or mouse and can be switched off
        {
            Movement(); //move the player
        }
        else
        {
            moveDirection = Vector3.zero;
        }
        if(moveDirection.magnitude > 0.1) //if there is some movement:
        {
            StartCoroutine(GameManager.NextHint("Move")); //disable hint
            if(isSprinting)
            {
                StartCoroutine(GameManager.NextHint("Sprint")); //disable hint
                crabAnimator.SetBool("isRunning", true); //set sprinting animation
                crabAnimator.SetBool("isWalking", true); //set walking animation > running animation wont work from standstill otherwise
            }
            else
            {
                crabAnimator.SetBool("isWalking", true); //set walking animation
                crabAnimator.SetBool("isRunning", false); //un-set sprinting animation
            }
            rb.velocity = moveDirection * moveSpeed;
        }
        else
        {
            crabAnimator.SetBool("isWalking", false); //un-set walking animation
            crabAnimator.SetBool("isRunning", false); //un-set sprinting animation
        }

        //apply rotation - this is not based on player input. This is based on current movement vector - which is based on player input
        if(moveDirection != Vector3.zero) //if there is some amount of movement
        {
            targetRotation = Quaternion.LookRotation(moveDirection); //set target rotation to match the move direction;
            if(isSprinting)
            {
                if((transform.right - moveDirection).magnitude < (-transform.right - moveDirection).magnitude) //checks which side of the crab is closest to the target direction
                {
                    targetRotation *= Quaternion.Euler(0,-90,0);//change target rotation by 90deg
                    crabAnimator.SetBool("reverseRun", true);
                }
                else
                {
                    targetRotation *= Quaternion.Euler(0,90,0);//change in the other direction by 90deg
                    crabAnimator.SetBool("reverseRun", false);
                }
            }
            else if (isDragging)
            {
                targetRotation *= Quaternion.Euler(0,180,0);//change target rotation to backwards
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed); //smoothly aligns the player facing direction
            
        }
    }

    private void GrabCheck()
    {
        if(Input.GetMouseButtonDown(0)) 
        {
            if(grabCollider.colList.Count > 0) //checks that there are actually objects to grab
            {
                StartCoroutine(GameManager.NextHint("Grab")); //disable hint
                isGrabbing = true;
                grabObject = grabCollider.colList[0].gameObject.transform; //save the prop
                grabObject.GetComponent<Interactable>().heldBy = gameObject; //we are holding the object
                //armTargetL.transform.LookAt(grabObject);
                armTargetL.position = grabCollider.colList[0].bounds.ClosestPoint(armTargetL.position); //move Lhand to grabbed object

                if(!grabObject.GetComponent<Interactable>().isHeavy)//if the object is light
                {
                    grabObject.parent = armTargetL; //make the grabbed object a child of the grabbing arm
                    armTargetL.position = grabLightTarget.position; //move the arm to the position for holding light objects
                    grabObject.GetComponent<Rigidbody>().isKinematic = true;
                }
                else //if the object is heavy
                {
                    CharacterJoint joint = gameObject.AddComponent<CharacterJoint>(); //adds a joint to the player object
                    joint.enableCollision = false; //don't let the held object collide with the player
                    joint.anchor = armTargetL.localPosition; //set the joint anchor to where the target is, which is the closest point on the held object
                    joint.connectedBody = grabObject.GetComponent<Rigidbody>();
                    isDragging = true; //turn on dragging
                    isSprinting = false; //disable sprinting
                    moveSpeed = baseMoveSpeed; //set movespeed
                }
                
                /*if (grabObject.GetComponent<OutlineManager>() != null) // Hugo - Disables the outline while the object is held
                    grabObject.GetComponent<OutlineManager>().isHeld = true; */
            }
            else
            {
                armTargetL.position = grabAirTarget.position; //grab the air
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            if(isGrabbing)
            {
                /*if (grabObject.GetComponent<OutlineManager>() != null) // Hugo - Reenables the outline after the object is dropped 
                    grabObject.GetComponent<OutlineManager>().isHeld = false; */
                
                DropObject(); //drop the currently held object  
            }
            else
            {
                armTargetL.localPosition = startPosArmTargetL; //return the arm to its start posiiton 
            }
        }
    }

    public void DropObject()
    {
            isGrabbing = false;
            if(!grabObject.GetComponent<Interactable>().isDoomed) //if the held object is not about to be destroyed
            {
                grabObject.parent = GameObject.Find("_Props").transform; //return the original parent  
                grabObject.GetComponent<Rigidbody>().isKinematic = false;
                grabObject.GetComponent<Interactable>().heldBy = null; //nothing is holding the object
            }
            else //the object is set to be destroyed
            {
                grabCollider.colList.Remove(grabCollider.colList[0]); //remove the held object from the collider collection
                stabCollider.colList.Remove(stabCollider.colList[0]); //in the situation that a different object is the [0], problems amy arise
                //i think might be good to combine both collection colliders into one, and/or have a method that can be called on the collector to remove objects from it by name
                
            }
            armTargetL.localPosition = startPosArmTargetL; //return the arm to its start posiiton 
            //armTargetL.localRotation = Quaternion.Euler(Vector3.zero);

            if(grabObject.GetComponent<Interactable>().isHeavy) //if the object was heavy
            {
                isDragging = false; //turn of dragging
                CharacterJoint joint = GetComponent<CharacterJoint>();
                Destroy(joint); //destroy the joint we created
            }

            grabObject = null; //reset grabObject
    }

    private void StabCheck()
    {
        if(Input.GetMouseButtonDown(1)) //if rmb pressed
        {
            StartCoroutine(GameManager.NextHint("Stab")); //disable hint
            if(stabCollider.colList.Count > 0) //if there is something to stab
            {
                stabObject = stabCollider.colList[0].gameObject.transform; //save the object
                //armTargetR.transform.LookAt(stabObject);
                armTargetR.position = stabCollider.colList[0].bounds.ClosestPoint(armTargetR.position); //move Rhand to stabbed object 
                stabObject.GetComponent<Interactable>().Stabbed(transform); //call the object's stabbed function
                Invoke("FinishStab",0.5f);
            }
            else
            {
                armTargetR.position = stabAirTarget.position; //stab the air
                Invoke("FinishStab",0.5f);
            }
        stabTimer = stabCoolDown; //reset the timer to the cooldown amount
        }
    }

    private void FinishStab() //in lieu of an animation or timed co-routine, invoking this function at a delay
    {
        armTargetR.localPosition = startPosArmTargetR; //return the arm to its start posiiton
    }
}
