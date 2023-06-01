using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeSus : MonoBehaviour
{
    public float killingSpeed = 1.39f;
    public float activeMomentum;
    public float timer;

    private Vector3 currentPos;
    private Vector3 lastTrackedPos;

    public bool killBillMode = false;


    

    // Start is called before the first frame update
    void Start()
    {
        lastTrackedPos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        currentPos = gameObject.transform.position;


        // Tracks the position history of the gameObject for later reference of velocity
        if (timer >= 0.1)
        {
            lastTrackedPos = currentPos;
            timer = 0;
        }

        activeMomentum = Vector3.Distance(currentPos, lastTrackedPos);

        if (activeMomentum >= killingSpeed)
        {
            killBillMode = true;
        }
        else
        killBillMode = false;

        

    }

    
    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("WHAMMY");
        GameObject whatWeDoneHit;

        if (collision.gameObject.CompareTag("Stabbable"))
        {
            whatWeDoneHit = collision.gameObject;
            //Debug.Log("sussy cone: Target Set!");
        }
        else 
            whatWeDoneHit = null;

        if (killBillMode == true)
        {
            if (whatWeDoneHit.CompareTag("Stabbable"))
            {
                NPCController npcControllerRef = whatWeDoneHit.GetComponent<NPCController>();

                npcControllerRef.WHAMMY = true;
                Debug.Log("whammy passed");
            }
            else
                return;
        }
    }
}
