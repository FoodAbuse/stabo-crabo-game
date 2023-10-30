using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseballFire : MonoBehaviour
{

    public GameObject baseballRef;
    public GameObject targetRef;

    public float fireForce = 500f;

    public bool tempBool;
    public bool tempReset;
    public Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = baseballRef.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (tempBool)
        {
            FireAtWill();
            tempBool = false;
        }

        if (tempReset)
        {
            baseballRef.GetComponent<Rigidbody>().isKinematic = true;
            baseballRef.transform.position = startPosition;
            baseballRef.SetActive(false);

        }
    }

    public void FireAtWill()
    {
        //Instantiate(baseballRef, new Vector3(13,-9,-159));

        baseballRef.SetActive(true);
        startPosition = baseballRef.transform.position;        

        baseballRef.transform.LookAt(targetRef.transform.position);


        baseballRef.GetComponent<Rigidbody>().isKinematic = false;
        baseballRef.GetComponent<Rigidbody>().AddForce(baseballRef.transform.forward * fireForce);

        


    }

    private void OnCollisionEnter(Collision other) 
    {
        Debug.Log("we hit "+ other);
    }
}
