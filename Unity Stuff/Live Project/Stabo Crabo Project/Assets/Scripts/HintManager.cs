using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Hint
{
    public string content;
    public int index;
    //[HideInInspector]
    public bool achieved;
    //[HideInInspector]
    public bool triggered;
}
public class HintManager : MonoBehaviour
{
    public List<Hint> hintList;
    private int alertNum;
    [SerializeField]
    private string alertIconRef;
    private GameObject alertIcon; //the icon that shows the number of active hints
    [SerializeField]
    private string hintTextRef;
    private TextMeshProUGUI hintText; //the text that lists all the hints
    [SerializeField]
    private string alertTextRef;
    private TextMeshProUGUI alertText; //the text shows the number of active hints

    private Transform ui;

    void Start()
    {
        //hint managers are local to the level scene, and so cannot drag and drop connections to the ui manager
        ui = GameObject.Find("UIManager").GetComponent<Transform>();
        CanvasRenderer[] uiTransforms = ui.GetComponentsInChildren<CanvasRenderer>(true);
        foreach(CanvasRenderer t in uiTransforms)
        {
            if(t.name == alertIconRef)
            {
                alertIcon = t.gameObject;
            }
            else if(t.name == hintTextRef)
            {
                hintText = t.GetComponent<TextMeshProUGUI>();
            }
            else if(t.name == alertTextRef)
            {
                alertText = t.GetComponent<TextMeshProUGUI>();
            }
        }
        //have to do this long winded approach because the hint panel starts as inactive, so gameobject.find doesn't work
    }

    void HintUpdate() //update the text in the hint menu to match the current hint stats
    {
        hintText.text = ""; //erase the existing text

        foreach(Hint hint in hintList)
        {
            if(hint.triggered && !hint.achieved) //if the hint has been triggered but not achieved
            {
                hintText.text += "\n\u2022 " + hint.content; //add the content to the text
            }
            else if(hint.triggered && hint.achieved) //if the hint has been triggered and achieved
            {
                hintText.text += "\n\u2022 " + "<s>" + hint.content + "</s>"; //add the content to the text with strikethrough or green + complete or something
            }
            else
            {
                hintText.text += "\n\u2022 " + "???"; //if it has not been triggered ad ???? so that players can see how many hints remain to uncover
            }
        }

        //update the text object with the accumulated text
    }

    public void HintTrigger(int hintIndex) //turn on the hint
    {
        foreach(Hint hint in hintList)
        {
            if(hint.index == hintIndex) //if it is the matching hint
            {
                if(hint.triggered || hint.achieved){return;} //if it has already been triggered or achieved there is a mistake, just return

                //play a sound

                hint.triggered = true;
                HintAlert(1); //increase the hint alert by 1
                HintUpdate(); //update the hint menu text
            }
        }
    }

    public void HintAchieve(int hintIndex) //turn off the hint
    {
        foreach(Hint hint in hintList)
        {
            if(hint.index == hintIndex) //if it is the matching hint
            {
                if(hint.achieved){return;} //if it has already been achieved there is a mistake, just return

                //play a sound

                hint.triggered = true; //if it hadn't been triggered yet (due to delay), just count it as triggered
                hint.achieved = true;
                HintAlert(-1); //decrease the hint alert by 1
                HintUpdate(); //update the hint menu text
            }
        }
    }

    private void HintAlert(int n) //increase or decrease the alert icon
    {
        alertNum += n; //increment the amount

        alertText.text = alertNum.ToString(); //update the text on the object to match

        if(alertNum <=0) //if there are no hints to show
        {
            alertNum = 0; //numeric correction

            alertIcon.SetActive(false); //hide the alertIcon
        }
        else
        {
            alertIcon.SetActive(true); //show the alertIcon (it may already be showing but that's fine)
            //play a sound
        }
    }
}


