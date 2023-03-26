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

    private float moveSpeed;
    private float turnSpeed;
    private bool isSprinting = false;
    private bool isGrabbing = false;

    private Transform grabParent;
    private Transform grabObject;
    
    //Defining Components
    public ColliderListController grabCollider;

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
        
    }

    void Update()
    {
        //movement input
        moveDirection = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical")).normalized;
        //sprint input
        if(Input.GetKeyDown(KeyCode.LeftShift)) //sprint button on
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
        //grab input
        //this is currently hold. can use the isGrabbing bool to change it to toggle if needed
        if(Input.GetMouseButtonDown(0) && grabCollider.colList.Count > 0) //checks that there are actually objects to grab
        {
            isGrabbing = true;
            grabObject = grabCollider.colList[0].transform; //save the prop
            grabParent = grabObject.parent; //save the prop's parent
            grabObject.GetComponent<Rigidbody>().isKinematic = true;
            grabCollider.colList[0].transform.parent = grabCollider.transform; //make the grabbed object a child of the grabbing collider
            //this is a temporary solution, I will need to refine this method as I expect it will cause issues
        }
        if(Input.GetMouseButtonUp(0) && isGrabbing)
        {
            isGrabbing = false;
            grabObject.parent = grabParent; //return the original parent  
            grabObject.GetComponent<Rigidbody>().isKinematic = false;         
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
            //transform.rotation = targetRotation; //aligns the player to face the moveDirection
            //targetRotation *= Quaternion.Euler(0,-90,0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed); //smoothly aligns the player facing direction
            
        }
        
    }

    void FixedUpdate()
    {
        rb.MovePosition((rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime)); //move the player
    }
}
