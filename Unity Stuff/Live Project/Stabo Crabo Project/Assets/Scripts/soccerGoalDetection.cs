using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soccerGoalDetection : MonoBehaviour
{
    public GameObject ballRef;
    public GameObject Confetti;

    public Vector3 ballSpawn;

    public float respawnTime = 0.5f;
    public bool isVolleyball = false;
    public GameObject returnTarget;

    
    private GameObject playerRef;
    public bool returningToSender;
    public float returnAssist = 5f;
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
        if (other.gameObject == ballRef && hasScored == false && returningToSender == false)
        {
            Debug.Log("GOAL");
            if (Confetti != null)
            {
                Confetti.SetActive(true);
                confettiTimer = 0;
                confettiReset = true;                
            }

            hasScored = true;

            if (isVolleyball)
            {
                StartCoroutine(VolleyballScore());
            }
            else
            StartCoroutine(ResetGame());

        }
    }

    public void SetReturnFalse()
    {
        Debug.Log("Return to Sender Disabled");
        returningToSender = false;
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

        yield return new WaitForSeconds(respawnTime);

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

    IEnumerator VolleyballScore()
    {
        Vector2 aimAdjust = new Vector2();

        yield return new WaitForSeconds(respawnTime);

        Vector2 ballVector2 = new Vector2(ballRef.transform.position.x, ballRef.transform.position.z);
        Vector2 targetVector2 = new Vector2(returnTarget.transform.position.x, returnTarget.transform.position.z);

        aimAdjust = ballVector2 - targetVector2;
        float angle = Vector2.Angle(aimAdjust, ballRef.transform.forward);

        Debug.Log(angle);

        ballRef.transform.LookAt (returnTarget.transform);

        /*
        Quaternion firingAngle = Quaternion.Euler(0, ballRef.transform.rotation.y, ballRef.transform.rotation.z);
        ballRef.transform.rotation = firingAngle;
        */

        float offsetForce = Vector3.Distance(ballRef.transform.position, returnTarget.transform.position) * returnAssist;
        float returnForce = 230f + offsetForce;

        ballRef.GetComponent<volleyballLauncher>().Launch();
        ballRef.GetComponent<Rigidbody>().AddForce(ballRef.transform.forward * 230);
        
        returningToSender = true;
        hasScored = false;
    }
}
