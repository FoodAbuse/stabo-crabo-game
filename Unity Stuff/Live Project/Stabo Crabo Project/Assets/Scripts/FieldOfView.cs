using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    /*
    This script checks every x seconds for objects in its cone that match the target list.
    It cycels through all matching objects and picks the highest priority one to become the current target.
    This target can be used by NPC controller etc, to control movement towards or away.

    Currently it only looks for transform center of the target when checking range and obstruction, not the closest collider point.

    if a found object does not pass the sight check, the current target remains unchanged. UNLESS that object WAS our current target, in which case our current target is now null

    */


    public float radius;
        public float radiusInner; //inner radius has 360 degree vision
    [Range(0,360)] //limit angle to 360

    public float angle;

    public List<Interactable> targetRef; //list of all targets
    public List<string> tagRef; //will target objects containing this string in their name

    public LayerMask targetMask; //so we are not checking over all objects in a scene
    public LayerMask obstructionMask;
    public Transform target; //the current target within our FOV
    public bool canSeeTarget;
    [HideInInspector]
    public float distanceToTarget;

    [SerializeField]
    private NPCController npc;
    public Transform eyes; //origin of all sight checks

    private bool seekingPlayer; //whether we care about the player or not (turned on for defending and guarding)
    private bool returnedTarget; //whether we succesfully returned a target in this iteration of the FOC check

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    private void Update()
    {
        if(npc.myBehaviour == NPCController.Behaviours.Defending || npc.myBehaviour == NPCController.Behaviours.Guarding) //set seeking player bool
        {
            seekingPlayer = true;
        }
        else
        {
            seekingPlayer = false;
        }

        if(canSeeTarget)
        {
            distanceToTarget = Vector3.Distance(transform.position, target.GetComponent<Collider>().ClosestPoint(transform.position)); //distance to closest point on the target
        }
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f); //used to delay the coroutine

        while (true) //infinite loop
        {
            yield return wait;
            if(targetRef.Count != 0 || seekingPlayer) //if there are targets to look for or we are seeking the player
            {
                returnedTarget = false; //reset this tracker every iteration
                FieldOfViewCheck(); //look for objects and or player
            }
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(eyes.position, radius, targetMask); //creates an array of all objects in a radius around this transform
        if(rangeChecks.Length != 0) //if there are any objects in range
        {
            foreach(Collider col in rangeChecks) //iterrate through each of those objects
            {
                if(seekingPlayer && col.tag == "Player") //if we are seeking the player and this obj is the player
                {
                    if(npc.destinationBounds.bounds.Contains(col.transform.position)) //if the player is within defended area
                    {
                        SightCheck(col.transform);
                    }
                    else if(npc.myBehaviour == NPCController.Behaviours.Guarding) //if we are guarding we still care even if they are not in the zone
                    {
                        SightCheck(col.transform);
                    }
                    else if(target) //if the we are defending, but they are our current target we no longer target them
                    {
                        if(target.tag == "Player"){WipeTarget();} //wipe the target, if it doesnt catch a new target in the current loop, it will find a new one in the next loop
                    }
                }
                else //otherwise we are just looking for objects
                {
                    Interactable found = col.GetComponent<Interactable>();//fetch the interactable script on the found object
                    if(targetRef.Contains(found) || tagRef.Contains(col.tag)) //if this is actually a target we care about
                    {
                        //Debug.Log(Vector3.Distance(found.transform.position, found.preferredPos));
                        //Debug.Log(target);
                        if(found.GetComponent<Interactable>().heldBy)  //if the object is being held
                        {
                            if(found.GetComponent<Interactable>().heldBy.tag == "Player") //if the object is currently held by the player it is a priority
                            {
                                Debug.Log("Found held by player");
                                SightCheck(found.transform);
                            }  //if it is held by another NPC we ignore it
                        }
                        else if(!target //if target has not yet been assigned
                        && Vector3.Distance(found.transform.position, found.preferredPos) > 1.0 //and this object is not in its preferred position
                        && !npc.heldObject) //and we are not holding an object currently
                        {
                            //fill with the first object that this loops through
                            SightCheck(found.transform);
                        }
                    }
                }
            }
        }
        else if(canSeeTarget) //if there are not targets in range, but when we last checked, we had a target we could see (very unlikley to trigger)
        {
            WipeTarget(); //we can no longer see the target
        }

        if(canSeeTarget && !returnedTarget) //if this iteration did not return a target, but we had a target when we last checked,
        {
            WipeTarget();
        }
    }

    private void SightCheck(Transform found) //checks line of sight for the found object
    {
        Vector3 directionToFound = (found.position - eyes.position).normalized; //gets direction to the found object
            //if the angle to our found obj does not exceed our FOV angle, or if it is in our inner radius
            if(Vector3.Angle(transform.forward, directionToFound) < angle / 2 || Vector3.Distance(transform.position, found.position) < radiusInner)
            {
                float distanceToFound = Vector3.Distance(eyes.position, found.position); //gets distance between us and the found obj
                if(!Physics.Raycast(eyes.position, directionToFound, distanceToFound, obstructionMask)) //raycast from us in the direction, at the distance, stopped by obstructions
                {
                    NewTarget(found); //our target is now this found object
                    canSeeTarget = true; //if the raycast doesnt hit anything then there is no obstruction
                    returnedTarget = true;
                }
                else if(target == found) //if we cant see it, but this was our current target...
                {
                    canSeeTarget = false;
                    target = null; //this is no longer able to be our active target
                }

            }
            else if (target == found) //if its out of angle range, but this was our current target...
            {
                canSeeTarget = false;
                target = null;
            }


    }

    public void WipeTarget()
    {
        target = null;
        canSeeTarget = false;
    }

    public void NewTarget(Transform t) //call to change the target to this transform
    {
        target = t;
        npc.DropObject(); //ensures the NPC doesnt double up on objects they are trying to carry
    }

}
