using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //controls
    private PlayerControls controls;
    private bool sprintBtnDown;
    private Vector2 moveControl;

    //Defining variables
    public float baseMoveSpeed = 2.5f;
    public float sprintMoveSpeed = 5.0f;
    public float baseTurnSpeed = 2.0f;
    public float sprintTurnSpeed = 4.0f;
    public float stabCoolDown = 1.0f; //how fast can stab be spammed
    public float stunned = 0.0f; //how long the player is stunned for
    private bool isStunned = false;
    private float stabTimer; //variable used to time stabs

    private float moveSpeed;
    private float turnSpeed;
    private bool isSprinting = false;
    private bool isGrabbing = false;
    private bool isDragging = false;

    public Transform cam;


    //private Transform grabParent; //commented out because so far everything dropped should go under _prop afterwards
    [HideInInspector]
    public Transform grabObject;
    public GameObject wornObject;

    [HideInInspector]
    public bool isWearingObject = false;
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
    private bool pivotRightPressed;
    private bool pivotLeftPressed;
    private bool pivoting;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Gameplay.Grab.started += ctx => GrabCheck();
        controls.Gameplay.Grab.canceled += ctx => DropObject();
        controls.Gameplay.Stab.performed += ctx => StabCheck();
        controls.Gameplay.Sprint.started += ctx => sprintBtnDown = true;
        controls.Gameplay.Sprint.canceled += ctx => sprintBtnDown = false;
        controls.Gameplay.Move.performed += ctx => moveControl = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => moveControl = Vector2.zero;
        controls.Gameplay.PivotRight.started += ctx => pivotRightPressed = true;
        controls.Gameplay.PivotRight.canceled += ctx => pivotRightPressed = false;
        controls.Gameplay.PivotLeft.started += ctx => pivotLeftPressed = true;
        controls.Gameplay.PivotLeft.canceled += ctx => pivotLeftPressed = false;
    }

    void OnEnable()
    {
        controls.Gameplay.Enable(); //enables all of our controls
    }

    void OnDisable()
    {
        controls.Gameplay.Disable(); //enables all of our controls
    }

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
        if(stabTimer > 0.0f) //if stab is on cooldown,
        {
            stabTimer -= 1 * Time.deltaTime; //tick it down
        }

        if(stunned > 0.0f) //if the player is stunned
        {
            isStunned = true; //player cannot input
            stunned -= 1 * Time.deltaTime; //reduce stun timer
        }
        else
        {
            isStunned = false;
        }
    }

    void MovementInput() //takes player input to move the player character
    {
        float horizontalAxis = moveControl.x; //Input.GetAxis("Horizontal"); //get input values
        float verticalAxis = moveControl.y; //Input.GetAxis("Vertical");

        var forward = cam.forward; //get camera directions
        var right = cam.right;
        forward.y = 0; //remove y values from camera directions
        right.y = 0;
        forward.Normalize(); //set magnitude to 1
        right.Normalize();

        moveDirection = (forward * verticalAxis + right * horizontalAxis).normalized;


        //moveDirection = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical")).normalized; //receive input for movement vector
        if(sprintBtnDown && moveDirection.magnitude > 0.1 && !isDragging) //sprint button on and we are moving, and we are not dragging something
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
        if(!isStunned) //being stunned stops normal movment calculations
        {
            if(GameManager.acceptPlayerInput) //the following functions require player input via keyboard or mouse and can be switched off
            {
                MovementInput(); //create a vector based on key presses
                PivotCheck();
            }
            else if(!isStunned) //if we ARE stunned, our direction has been written by a shove script
            {
                moveDirection = Vector3.zero;
            }
            ApplyRotation();
            ApplyMovement();       
        }
    }

    void ApplyMovement() //takes our current move direction and applys speed and force
    {
        if(moveDirection.magnitude > 0.1) //if there is some movement:
        {
            StartCoroutine(GameManager.NextTip("Move")); //disable tip
            if(isSprinting)
            {
                StartCoroutine(GameManager.NextTip("Sprint")); //disable tip
                crabAnimator.SetBool("isRunning", true); //set sprinting animation
                crabAnimator.SetBool("isWalking", true); //set walking animation > running animation wont work from standstill otherwise
            }
            else
            {
                crabAnimator.SetBool("isWalking", true); //set walking animation
                crabAnimator.SetBool("isRunning", false); //un-set sprinting animation
            }
            Vector3 netMovement = moveDirection * moveSpeed; //this is just horizontal movement in 3D space
            netMovement.y = rb.velocity.y; //adds in the y movement of the current rigidbody (so physics calculations etc)
            if(netMovement.y >= moveSpeed) netMovement.y = moveSpeed; //caps the upward velocity, but hopefully not the downward velocity, because I am not grabbing magnitude

            /*if(Quaternion.Angle(transform.rotation,targetRotation) < 10.0f || isSprinting || isDragging || rb.velocity.magnitude >= 1.0f) //start to move once we are facing the target direction, or if we are sprinting
            { 
                //Debug.Log(rb.velocity.magnitude);
                rb.velocity = netMovement;
            } OPTION 1 FOR PIVOT ON SPOT*/
            if(true)
            {
                rb.velocity = netMovement;
            }
        }
        else
        {
            if(!pivoting)
            {
                crabAnimator.SetBool("isWalking", false); //un-set walking animation
            }
            crabAnimator.SetBool("isRunning", false); //un-set sprinting animation
        }
    }

    void PivotCheck()
    {
        if(moveDirection.magnitude > 0.1){return;} //only pivot when not moving

        if(pivotRightPressed && pivotLeftPressed)
        {
            pivoting = false; //nothing
        }
        else if(pivotRightPressed)
        {
            targetRotation = transform.rotation *= Quaternion.Euler(0,turnSpeed*0.8f,0);
            pivoting = true;
        }
        else if(pivotLeftPressed)
        {
            targetRotation = transform.rotation *= Quaternion.Euler(0,-turnSpeed*0.8f,0);
            pivoting = true;
        }
        else
        {
            pivoting = false;
        }

        if(pivoting)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
            crabAnimator.SetBool("isWalking", true); //play walking animation
        }
        else
        {
            crabAnimator.SetBool("isWalking", false); //stop walking animation
        }
    }

    void ApplyRotation() //rotates the player to match the movement direction and move style
    {
        //apply rotation - this is not based on player input. This is based on current movement vector - which is based on player input
        if(moveDirection != Vector3.zero) //if there is some amount of movement
        {
            Vector3 targetDirection = moveDirection; //direction we will rotate to
            if(isSprinting)
            {
                if((transform.right - moveDirection).magnitude < (-transform.right - moveDirection).magnitude) //checks which side of the crab is closest to the target direction
                {
                    targetDirection = Quaternion.AngleAxis(-90, Vector3.up) * targetDirection;
                    crabAnimator.SetBool("reverseRun", true);
                }
                else
                {
                    targetDirection = Quaternion.AngleAxis(90, Vector3.up) * targetDirection;
                    crabAnimator.SetBool("reverseRun", false);
                }
            }
            else if (isDragging) //set reverse direction
            {
                targetDirection = Quaternion.AngleAxis(180, Vector3.up) * targetDirection;
            }
            Vector3 torque = Vector3.Cross(transform.forward, targetDirection); //torque is the cross between our forward and the target rotation
            if(Vector3.Angle(targetDirection, transform.forward) > 150)
            {
                rb.AddTorque(torque * Vector3.Angle(targetDirection, transform.forward) / 10);
            }
            else if(Vector3.Angle(targetDirection, transform.forward) > 1)
            {
                //rb.AddTorque(torque * Vector3.Angle(targetDirection, transform.forward) / 10);
                rb.angularVelocity = (torque * turnSpeed);
            }
            else
            {
                rb.angularVelocity = Vector3.zero;
                transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            }
            //rb.AddRelativeTorque(torque * turnSpeed);//(Vector3.Angle(targetDirection, transform.forward))/10);
            //Debug.Log(Vector3.Angle(targetDirection, transform.forward));
        }
    }

    private void GrabCheck()
    {
        if(GameManager.acceptPlayerInput) 
        {
            StartCoroutine(GameManager.NextTip("Grab")); //disable tip
            if(grabCollider.colList.Count > 0) //checks that there are actually objects to grab
            {
                grabObject = grabCollider.selected.transform; //save the prop
                if(!grabObject.GetComponent<Interactable>().canBeGrabbed) //if the object cannot be grabbed, return
                {
                    grabObject = null;
                    return;
                }
                if(grabObject.GetComponent<Interactable>().canBeWorn) //Cheap stand-in for now but as we are not yet planning on having multiple wearables this will work
                {
                    if(isWearingObject) //In case somehow a second wearable ends up in scene, prevents deleting the other item and returns instead
                    {
                        grabObject = null;
                        return;
                    }

                    Destroy(grabObject.gameObject); //deletes the wearable object
                    wornObject.SetActive(true);                    
                    isWearingObject = true;

                    grabObject = null;
                    return;
                }
                isGrabbing = true;
                grabObject.GetComponent<Interactable>().heldBy = gameObject; //we are holding the object
                
                //armTargetL.transform.LookAt(grabObject);
                armTargetL.position = grabCollider.selected.bounds.ClosestPoint(armTargetL.position); //move Lhand to grabbed object

                if(!grabObject.GetComponent<Interactable>().isHeavy)//if the object is light
                {
                    grabObject.parent = armTargetL; //make the grabbed object a child of the grabbing arm
                    armTargetL.position = grabLightTarget.position; //move the arm to the position for holding light objects
                    grabObject.GetComponent<Rigidbody>().isKinematic = true;

                    // TESTING
                    //Debug.Log(grabObject.GetComponent<Collider>() + "is now being held"); 
                    //grabObject.GetComponent<Collider>().enabled = false;
                    grabObject.GetComponent<Collider>().isTrigger = true;
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
    }

    public void DropObject()
    {
        if(!isGrabbing)
        {
            armTargetL.localPosition = startPosArmTargetL;
            return; //if we are not grabbing anything, return
        } 
        isGrabbing = false;
        if(!grabObject.GetComponent<Interactable>().isDoomed) //if the held object is not about to be destroyed
        {
            grabObject.parent = GameObject.Find("_Props").transform; //return the original parent 

            // TESTING
            //grabObject.GetComponent<Collider>().enabled = true;
            grabObject.GetComponent<Collider>().isTrigger = false;

            grabObject.GetComponent<Rigidbody>().isKinematic = false;
            grabObject.GetComponent<Interactable>().heldBy = null; //nothing is holding the object
        }
        else //the object is set to be destroyed
        {
            grabCollider.Remove(grabObject.GetComponent<Collider>()); //remove the held object from the collider collection
            stabCollider.Remove(grabObject.GetComponent<Collider>());
        }
        armTargetL.localPosition = startPosArmTargetL; //return the arm to its start posiiton 
        //armTargetL.localRotation = Quaternion.Euler(Vector3.zero);

        if(grabObject.GetComponent<Interactable>().isHeavy) //if the object was heavy
        {
            isDragging = false; //turn of dragging
            CharacterJoint[] joints = GetComponents<CharacterJoint>();
            foreach(CharacterJoint joint in joints) //this is here because an issue came up with alt + tab creating multiple joints
            {
                Destroy(joint); //destroy the joint(s) we created
            }
        }

        grabObject = null; //reset grabObject
    }

    private void StabCheck()
    {
        if(GameManager.acceptPlayerInput && stabTimer <= 0.0f) //if we are allowed to stab
        {
            StartCoroutine(GameManager.NextTip("Stab")); //disable tip
            if(stabCollider.colList.Count > 0) //if there is something to stab
            {
                armTargetR.position = stabCollider.selected.bounds.ClosestPoint(armTargetR.position); //move Rhand to stabbed object
                foreach(Collider col in stabCollider.colList) //stab now applies to everything in range all at once
                {
                    stabObject = col.gameObject.transform; //save the object

                    // added to fix an error when stabbing bucket prefab
                    //if (stabObject.GetComponent<Interactable>())
                    //{
                        stabObject.GetComponent<Interactable>().Stabbed(transform); //call the object's stabbed function
                    //}
                }
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
