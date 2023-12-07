using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearfulManager : MonoBehaviour
{
    [SerializeField]
    private List<Level3NPCSwapper> childList;
    [SerializeField]
    private int childCount;
    [SerializeField]
    private int childrenSetup = 0;

    public GameObject[] toDelete;

    bool hasSetup = false;
    void Start()
    {
        foreach(Transform child in transform)
        {
            Level3NPCSwapper c = child.GetComponent<Level3NPCSwapper>();
            childList.Add(c);
        }

        childCount = childList.Count;
        foreach (Level3NPCSwapper i in childList)
        {
            i.RecieveManager(this);
            i.SetupSelf();
        }
    }

    public void CompleteSetup()
    {

        childrenSetup++;

        if (childrenSetup >= childCount)
        {
            hasSetup = true;
            gameObject.SetActive(false);
            ClearInactive();
        }
    }

    private void OnEnable()
    {
        if (hasSetup == true)
        {
            foreach (Level3NPCSwapper x in childList)
            {
                x.SetParentSkin();
            }

            ClearInactive();
        }
    }

    void ClearInactive()
    {
        toDelete = GameObject.FindGameObjectsWithTag("InactiveHair");
        for (int i = 0; i < toDelete.Length; i++)
        {
            Destroy(toDelete[i]);
        }
    }
}
