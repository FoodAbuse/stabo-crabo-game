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
        //movement input
        moveDirection = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical")).normalized;
        //sprint input
        if(Input.GetKeyDown(KeyCode.LeftShift) && moveDirection.magnitude > 0.1) //sprint button on
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

        GrabCheck(); //put the grab inputs and code into a function
        if(stabTimer <= 0) //if the cool down has been exhausted
        {
            StabCheck(); //function for checking and executing stabs
        }
        else
        {
            stabTimer -= 1 * Time.deltaTime; //keep cooling-down the stab timer
        }
        

        //apply rotation
        if(moveDirection != Vector3.zero) //if there is some amount of movement
        {
            targetRotation = Quaternion.LookRotation(moveDirection); //set target rotation to match the move direction;
            if(isSprinting)
            {
                if((transform.right - moveDirection).magnitude < (-transform.right - moveDirection).magnitude) //checks which side of the crab is closest to the target direction
                {
                    targetRotation *= Quaternion.Euler(0,-90,0);//change target rotation by 90deg
                }
                else
                {
                    targetRotation *= Quaternion.Euler(0,90,0);//change in the other direction by 90deg
                }
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed); //smoothly aligns the player facing direction
            
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
            grabObject.GetComponent<Rigidbody>().isKinematic = true;
            grabObject.parent = armTargetL; //make the grabbed object a child of the grabbing arm
            //this is a temporary solution, I will need to refine this method as I expect it will cause issues

            if(grabObject.tag == "GrabLight")//if the object is light
            {
                armTargetL.position = grabLightTarget.position; //move the arm to the position for holding light objects
            }
        }
        if(Input.GetMouseButtonUp(0) && isGrabbing)
        {
            isGrabbing = false;
            grabObject.parent = grabParent; //return the original parent  
            grabObject.GetComponent<Rigidbody>().isKinematic = false;
            armTargetL.localPosition = startPosArmTargetL; //return the arm to its start posiiton  
            grabObject = null; //reset grabObject      
        }
    }

    private void StabCheck()
    {
        if(Input.GetMouseButtonDown(1)) //if rmb pressed
        {
            if(stabCollider.colList.Count > 0) //if there is something to stab
            {
                stabObject = stabCollider.colList[0].gameObject.transform; //save the object
                armTargetR.position = stabCollider.colList[0].bounds.ClosestPoint(armTargetR.position); //move Rhand to stabbed object 
                stabObject.GetComponent<StabbableController>().Stabbed(); //call the object's stabbed function
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
