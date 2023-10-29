using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soccerGoalDetection : MonoBehaviour
{
    public GameObject ballRef;
    public GameObject Confetti;

    public Vector3 ballSpawn;

    public float respawnTime = 3f;

    
    private GameObject playerRef;
    public bool confettiReset;
    public float confettiTimer;

    // Start is called before the first frame update
    public bool hasScored = false;
    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (confettiReset == true)
        {
            confettiTimer += Time.deltaTime;

                if (confettiTimer >= 3.0f)
                {
                    Confetti.SetActive(false);
                    confettiReset = false;
                }
        }
    }
    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject == ballRef && hasScored == false)
        {
            Debug.Log("GOAL");
            if (Confetti != null)
            {
                Confetti.SetActive(true);
                confettiTimer = 0;
                confettiReset = true;                
            }

            hasScored = true;
            StartCoroutine(ResetGame());

        }
    }

    IEnumerator ResetGame()
    {
       /* float timer = 0;

        timer += Time.deltaTime;
        if (timer>= 1.0f)
        {
            ballRef.transform.position = ballSpawn;
            hasScored = false;
        }
        */

        yield return new WaitForSeconds(0.5f);

        if (playerRef.GetComponent<PlayerController>().grabObject != null)
        {
            Debug.Log("Player is holding object");
            if (playerRef.GetComponent<PlayerController>().grabObject.gameObject == ballRef)
            {
                Debug.Log("Object is ball");
                playerRef.GetComponent<PlayerController>().DropObject();
            }
        }

        ballRef.transform.position = ballSpawn;
        ballRef.GetComponent<Rigidbody>().velocity = Vector3.zero;
        hasScored = false;
    }
}
