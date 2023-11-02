using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOptimiser : MonoBehaviour
{
    Animator animator;
    Transform playerRef;
    private float animationCullDistance = 10f;

    [SerializeField]
    private float playerDistance;

    public bool playerInRange;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerRef = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        playerDistance = Vector3.Distance(playerRef.position, transform.position);
        
        if (playerDistance <= animationCullDistance)
        {
            playerInRange = true;           
        }
        else
        playerInRange = false;
        
        if (playerInRange)
        {
            animator.enabled = true;
        }

        if(!playerInRange)
        {
            animator.enabled = false;
        }
    }

    public void DisableAnimation()
    {    
            animator.enabled = false;
    }

}
