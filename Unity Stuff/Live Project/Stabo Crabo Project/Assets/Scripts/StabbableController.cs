using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StabbableController : MonoBehaviour
{
    //[SerializeField]
    //private Animator animator; //the NPC animator

    private Vector3 stabTeleport;
    void Start()
    {
        stabTeleport = new Vector3 (0,0,1);
    }

    void Update()
    {
        
    }

    public void Stabbed()
    {
        GameManager.TargetKilled(gameObject); //call the target killed script in the game manager
        //animator.SetTrigger("Dead"); //play death animation
        Death();
    }

    public void Death()
    {
        Destroy(gameObject); //destroy self
    }
}
