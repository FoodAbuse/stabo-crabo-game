using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    [SerializeField]
    private int tally;

    private GameObject rewardObject;
   
   
    void Awake()
    {
        rewardObject = this.transform.GetChild(0).gameObject;
        rewardObject.SetActive(false);
    }

    public void TallyCrab()
    {
        tally++;

        if (tally >= 3)
        {
            DeliverReward();
        }
    }

    private void DeliverReward()
    {
        rewardObject.SetActive(true);
        rewardObject.transform.SetParent(null);
        Destroy(gameObject); // commit soduku so it can't be interacted with after reward is given 
    }

}
