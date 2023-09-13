using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //needed for navmesh
public class NPCCrabController: MonoBehaviour
{

    //Defining variables
    public NavMeshAgent agent; //the agent component on this NPC object
    public Collider oceanBounds; //area to run to
    public Animator animator;
    private Rigidbody rb;
    [SerializeField]
    private Collider col;

    private Vector3 destination; //where the crab is heading
    private bool stage2; //whether we are in stage 2 of running to ocean



    void Start()
    {
        //Fetch component references
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(Vector3.Distance(transform.position, destination) < 0.5f && !stage2) //if we reached ocean edge
        {
            stage2 = true;
            DeeperOcean(); //run to deeper ocean now
        }
    }

    void LateUpdate() //runs after update
    {
        //float speed = agent.velocity.magnitude; //grabs the current agent vector's magnitude
        //animator.SetFloat("Speed", speed); //set the animator parameter to match
    }

    public void RunToOcean() //called by puzzle object
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", true);
        animator.SetBool("reverseRun", true);
        destination = oceanBounds.ClosestPoint(transform.position);
        agent.SetDestination(destination); //set destination as closest ocean point
    }

    public void DeeperOcean() //run further into the ocean and destroy self
    {
        col.enabled = false;
        agent.SetDestination(oceanBounds.transform.position); //set destination as middle of ocean
        Invoke("Death",3.0f); //die in 5sec
    }

    private void Death()
    {
        Destroy(gameObject);
    }








}
