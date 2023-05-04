using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabbable : MonoBehaviour
{
    public float force = 10.0f;
    public virtual void Stabbed() //the base stab control
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * force); //sends the object into the air according to force. Later it would be nice to have this be affected by the direction of the stab.

    }
}
