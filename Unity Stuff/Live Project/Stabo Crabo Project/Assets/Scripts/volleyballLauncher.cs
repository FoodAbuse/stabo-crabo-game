using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class volleyballLauncher : MonoBehaviour
{
    public float  upForce = 15;
    
    public void Launch()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(0, upForce, 0);
    }
}
