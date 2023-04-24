using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehaviour : StateMachineBehaviour
{
    [SerializeField]
    private float timeUntilBored; //how long it takes NPCs to trigger a bored animation
    [SerializeField]
    private int numBoredAnimations; //allows in future there to be multiple bored animations
    private bool isBored; //NPC is currently permoring a bored animation
    private float idleTime; //how long the NPC has been idle
    private int boredAnimation; //the animation we want to transistion to

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetIdle(); //resets all of the time tracking variables   
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!isBored)
        {
            idleTime += Time.deltaTime; //tick up the idle timer
            if(idleTime > timeUntilBored && stateInfo.normalizedTime % 1 < 0.02f) //if the timer has met the bored threshold and we are at the beginning of a loop
            {
                isBored = true;
                boredAnimation = Random.Range(1, numBoredAnimations + 1); //pick a bored animation
                boredAnimation = boredAnimation * 2 - 1; //set up for default idle animation to be between each other motion
                animator.SetFloat("BoredAnimation", boredAnimation - 1); //instantly changes the animation to the default idle closest to the bored we are about to transition to
            }
        }
        else if (stateInfo.normalizedTime % 1 > 0.8) //if an animation is about to finish
        {
            ResetIdle(); //reset all the tracking variables
        }
        animator.SetFloat("BoredAnimation", boredAnimation, 0.2f, Time.deltaTime); //set the animation parameter over 0.2 seconds
    }

    private void ResetIdle() //resets all of the variables
    {
        if(isBored)
        {
            boredAnimation--; //set to the nearest default idle animation
        }
        isBored = false;
        idleTime = 0;
        timeUntilBored = Random.Range(timeUntilBored * 0.5f, timeUntilBored * 1.5f); //set a random time until bored when resetting 
    }
}

