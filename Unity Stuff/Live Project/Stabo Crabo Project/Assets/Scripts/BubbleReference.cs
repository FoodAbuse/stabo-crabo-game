using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleReference : MonoBehaviour
{
    //this script is just to hold an image reference for hint bubbles that appear above NPC heads
    //the primary reason this is not just a part of the Interactable script, is that the player needs a bubble reference as well
    //additionally, puzzle objects can use this script to assign bubbles that are not tied to specific objects, and an NPC can use this script as well

    public Sprite bubbleSprite;

}
