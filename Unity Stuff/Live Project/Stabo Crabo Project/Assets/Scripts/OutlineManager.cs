using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    public Color stabbableOutlineColor;
    public Color GrabbableOutlineColor;

    public bool isGrabbable;
    public bool isStabbable;
    public bool isOutlined;
    public bool isHeld = false;

    private GameObject crabRef;
    private Interactable interactableRef;
    private PlayerController playerScriptRef;
    private GameObject outlineRef;

    public float playerDistance;
    public float interactRange = 1.8f;

    // Start is called before the first frame update
    void Start()
    {
        WarmUp();
    }

    // Update is called once per frame
    void Update()
    {
        // Constantly checks the distance between player and the object
        playerDistance = Vector3.Distance(transform.position, crabRef.GetComponent<Transform>().position);

        // Enables and Disables outline based on player distance
        if (playerDistance <= interactRange && !isHeld)
        {
            isOutlined = true;
        }
        else
        isOutlined = false;

        if (isOutlined)
        {
            outlineRef.SetActive(true);
        }
        else
        {
            outlineRef.SetActive(false);
        }


    }

    void WarmUp()
    {
        interactableRef = gameObject.GetComponent<Interactable>();
        crabRef = GameObject.FindGameObjectWithTag("Player");
        playerScriptRef = crabRef.GetComponent<PlayerController>();

        // Sets the child object that enables/disables the outline
        outlineRef = gameObject.transform.GetChild(0).gameObject;
    }
}
