using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeZoneController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && GameManager.levelPhase == 2) //if the player enters the zone during escape phase
        {
            Debug.Log("Player Escaped");
            GameManager.LevelWon(); //trigger the level end

        }

    }
}
