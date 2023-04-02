using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StabbableController : MonoBehaviour
{
    private Vector3 stabTeleport;
    void Start()
    {
        stabTeleport = new Vector3 (0,0,1);
    }

    void Update()
    {
        
    }

    public void Stabbed()
    {
        transform.position += stabTeleport; //placeholder effect that just teleprots the target a lil bit
    }
}
