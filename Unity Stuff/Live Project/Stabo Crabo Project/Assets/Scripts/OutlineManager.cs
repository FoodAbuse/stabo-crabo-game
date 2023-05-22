using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    public Color stabbableOutlineColor;
    public Color GrabbableOutlineColor;

    public bool isGrabbable;
    public bool isStabbable;

    private GameObject crabRef;
    private Interactable interactableRef;
    private PlayerController playerScriptRef;

    public float playerDistance;
    public float interactRange = 1.8f;

    // Start is called before the first frame update
    void Start()
    {
        interactableRef = gameObject.GetComponent<Interactable>();
        crabRef = GameObject.FindGameObjectWithTag("Player");
        playerScriptRef = crabRef.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Constantly checks the distance between player and the object
        playerDistance = Vector3.Distance(transform.position, crabRef.GetComponent<Transform>().position);

    }
}
