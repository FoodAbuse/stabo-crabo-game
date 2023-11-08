using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabBubble : MonoBehaviour
{
    [SerializeField]
    private Sprite bubbleSprite;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<PlayerController>().BubbleOn(bubbleSprite);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<PlayerController>().BubbleOff();
        }
    }
}
