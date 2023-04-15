using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //needed for navmesh

public class NPCController : MonoBehaviour
{
    //temporary script to move NPCs around randomly
    //for now I want to set a random desitnation within navmesh bounds, and then start counting down a timer once I reach it

    public NavMeshAgent agent; //the agent component on this NPC object
    public Collider destinationBounds; //area for random position to be generated in
    private NavMeshPath path; //create an empty navmesh path

    public float timeToNewDestination = 5.0f; //the time before the NPC looks for a new destination once reaching its previous destination
    private float countdownToNewDestination = 0.0f;

    void Start()
    {
        path = new NavMeshPath(); //initialize the path
    }

    void Update()
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
                countdownToNewDestination = timeToNewDestination; //reset the countdown
            }
            else
            {
                countdownToNewDestination -= 1 * Time.deltaTime; //tick down countdown
            }
        }
        
        
    }

    public static Vector3 RandomPointInBounds(Bounds bounds) 
    {
        return new Vector3(
        Random.Range(bounds.min.x, bounds.max.x),
        0.0f,
        Random.Range(bounds.min.z, bounds.max.z)
    );
}
}
