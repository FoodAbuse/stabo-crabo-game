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


    private Transform grabParent;
    [HideInInspector]
    public Transform grabObject;
    [HideInInspector]
    public Transform stabObject;
    [SerializeField]
    private Transform stabAirTarget; //where the stab will go if there is no other target
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
            Movement(); //move the player
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
        else
        {
            moveDirection = Vector3.zero;
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

    void Movement() //takes player input to move the player character
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical")).normalized;
        if(Input.GetKeyDown(KeyCode.LeftShift) && moveDirection.magnitude > 0.1 && !isDragging) //sprint button on and we are moving, and we are not dragging something
        {
            isSprinting = true;
            moveSpeed = sprintMoveSpeed; //increase movement
            turnSpeed = sprintTurnSpeed;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift)) //sprint button off
        {
            isSprinting = false;
            moveSpeed = baseMoveSpeed; //reset movement
            turnSpeed = baseTurnSpeed;
        }
    }

    void FixedUpdate()
    {
        if(moveDirection.magnitude > 0.1) //if there is some movement:
        {
            if(isSprinting)
            {
                crabAnimator.SetBool("isRunning", true); //set sprinting animation
                crabAnimator.SetBool("isWalking", true); //set walking animation > running animation wont work from standstill otherwise
            }
            else
            {
                crabAnimator.SetBool("isWalking", true); //set walking animation
                crabAnimator.SetBool("isRunning", false); //un-set sprinting animation
            }
            rb.MovePosition((rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime)); //move the player
        }
        else
        {
            crabAnimator.SetBool("isWalking", false); //un-set walking animation
            crabAnimator.SetBool("isRunning", false); //un-set sprinting animation
        }
    }

    private void GrabCheck()
    {
        if(Input.GetMouseButtonDown(0) && grabCollider.colList.Count > 0) //checks that there are actually objects to grab
        {
            isGrabbing = true;
            grabObject = grabCollider.colList[0].gameObject.transform; //save the prop
            grabParent = grabObject.parent; //save the prop's parent
            armTargetL.position = grabCollider.colList[0].bounds.ClosestPoint(armTargetL.position); //move Lhand to grabbed object
            grabObject.parent = armTargetL; //make the grabbed object a child of the grabbing arm
            //this is a temporary solution, I will need to refine this method as I expect it will cause issues

            if(grabObject.tag == "GrabLight")//if the object is light
            {
                armTargetL.position = grabLightTarget.position; //move the arm to the position for holding light objects
                grabObject.GetComponent<Rigidbody>().isKinematic = true;
            }
            else if(grabObject.tag == "GrabHeavy") //if the object is heavy
            {
                HingeJoint joint = gameObject.AddComponent<HingeJoint>(); //adds a joint to the player object
                joint.enableCollision = false; //don't let the held object collide with the player
                Debug.Log(armTargetL.position);
                Debug.Log(joint.anchor);
                joint.anchor = armTargetL.position; //set the joint anchor to where the target is, which is the closest point on the held object
                joint.connectedBody = grabObject.GetComponent<Rigidbody>();
                isDragging = true; //turn on dragging
                isSprinting = false; //disable sprinting
                moveSpeed = baseMoveSpeed; //set movespeed
            }
        }
        if(Input.GetMouseButtonUp(0) && isGrabbing)
        {
            DropObject(); //drop the currently held object  
        }
    }

    public void DropObject()
    {
            isGrabbing = false;
            if(!grabObject.GetComponent<Interactable>().isDoomed) //if the held object is not about to be destroyed
            {
                grabObject.parent = grabParent; //return the original parent  
                grabObject.GetComponent<Rigidbody>().isKinematic = false;
            }
            else //the object is set to be destroyed
            {
                grabCollider.colList.Remove(grabCollider.colList[0]); //remove the held object from the collider collection
            }
            armTargetL.localPosition = startPosArmTargetL; //return the arm to its start posiiton 

            if(grabObject.tag == "GrabHeavy") //if the object was heavy
            {
                isDragging = false; //turn of dragging
                HingeJoint joint = GetComponent<HingeJoint>();
                Destroy(joint); //destroy the joint we created

            }

            grabObject = null; //reset grabObject
    }

    private void StabCheck()
    {
        if(Input.GetMouseButtonDown(1)) //if rmb pressed
        {
            if(stabCollider.colList.Count > 0) //if there is something to stab
            {
                stabObject = stabCollider.colList[0].gameObject.transform; //save the object
                armTargetR.position = stabCollider.colList[0].bounds.ClosestPoint(armTargetR.position); //move Rhand to stabbed object 
                stabObject.GetComponent<Stabbable>().Stabbed(); //call the object's stabbed function
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
