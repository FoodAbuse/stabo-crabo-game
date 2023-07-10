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
    [Range(0,360)] //limit angle to 360
    public float angle;

    public List<Interactable> targetRef; //list of all targets

    public LayerMask targetMask; //so we are not checking over all objects in a scene
    public LayerMask obstructionMask;

    public Transform target; //the current target within our FOV
    public bool canSeeTarget;
    [HideInInspector]
    public float distanceToTarget;

    [SerializeField]
    private NPCController npc;

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    private void Update()
    {
        if(canSeeTarget)
        {
            distanceToTarget = Vector3.Distance(transform.position, target.position);
        }
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f); //used to delay the coroutine

        while (true) //infinite loop
        {
            yield return wait;
            if(targetRef.Count != 0 || npc.myBehaviour == NPCController.Behaviours.Defending) //if there are targets to look for or we are defensive against the player
            {
                FieldOfViewCheck(); //look for objects and or player
            }
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask); //creates an array of all objects in a radius around this transform
        if(rangeChecks.Length != 0) //if there are any objects in range
        {
            foreach(Collider col in rangeChecks) //iterrate through each of those objects
            {
                if(npc.myBehaviour == NPCController.Behaviours.Defending && col.tag == "Player") //if we are defending against the player and this obj is the player
                {
                    SightCheck(col.transform);
                }
                else //otherwise we are just looking for objects
                {
                    Interactable found = col.GetComponent<Interactable>();//fetch the interactable script on the found object
                    if(targetRef.Contains(found)) //if this is actually a target we care about
                    {
                        Debug.Log(Vector3.Distance(found.transform.position, found.preferredPos));
                        Debug.Log(target);
                        if(found.GetComponent<Interactable>().heldBy)  //if the object is being held
                        {
                            if(found.GetComponent<Interactable>().heldBy.tag == "Player") //if the object is currently held by the player it is a priority
                            {
                                Debug.Log("Found held by player");
                                SightCheck(found.transform);
                            }  //if it is held by another NPC we ignore it
                        }
                        else if(!target && Vector3.Distance(found.transform.position, found.preferredPos) > 1.0f) //if target has not yet been assigned, fill with any object that is not in its preferred position
                        {
                            Debug.Log("Found out of position");
                            SightCheck(found.transform);
                        }
                    }
                }
            }
        }
        else if(canSeeTarget) //if there is nothing in range, but when we last checked we could see the target...
        {
            canSeeTarget = false; //we can no longer see the target
            target = null;
        }
    }

    private void SightCheck(Transform found) //checks line of sight for the found object
    {
        Vector3 directionToFound = (found.position - transform.position).normalized; //gets direction to the found object

            if(Vector3.Angle(transform.forward, directionToFound) < angle / 2) //if the angle to our found obj exceeds our FOV angle
            {
                float distanceToFound = Vector3.Distance(transform.position, found.position); //gets distance between us and the found obj
                if(!Physics.Raycast(transform.position, directionToFound, distanceToFound, obstructionMask)) //raycast from us in the direction, at the distance, stopped by obstructions
                {
                    target = found; //our target is now this found object
                    canSeeTarget = true; //if the raycast doesnt hit anything then there is no obstruction
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

}
