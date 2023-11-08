using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabBubble : MonoBehaviour
{
    [SerializeField]
    private Sprite bubbleSprite;
    private PlayerController playerRef;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            playerRef = other.GetComponent<PlayerController>();

            playerRef.BubbleOn(bubbleSprite);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            playerRef.BubbleOff();            
        }
    }
    void OnDisable() 
    {
        playerRef.BubbleOff();
    }
}
