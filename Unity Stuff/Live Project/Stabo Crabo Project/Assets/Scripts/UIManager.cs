using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //purpose of this manager is to run menus, tooltips most of the UI.
    private static UIManager _instance; // the manager variable

    //reference to different menu panels
    public GameObject panelTitle;
    public GameObject panelMain;
    public GameObject panelPause;
    public GameObject panelHints;
    public GameObject hintIcon;
    [SerializeField]
    private GameObject panelEndScreen;
    [SerializeField]
    private TextMeshProUGUI timeText; //shows the time on level completion

    //animators
    [SerializeField]
    private Animator titleAnimator;
    private Animator tipAnimator;
    [SerializeField]
    private Animator escapePanel;

    public GameObject panelCurrent; //the pannel that is currently active


    void Awake()
	{
		if (_instance == null) //if no manager already exists
		{
			_instance = this; //store this manager
			DontDestroyOnLoad(gameObject); //And protect it across scene loads
		}
		else
		{
			if (_instance != this) //Otherwise if there is a different manager
				Destroy(gameObject); //Destroy this object, because it is a duplicate
		}
    }

    void Start()
    {
        InitialisePanels();//initial set of UI variables
        //panelCurrent = panelTitle;
        
    }

    void Update()
    {
        if(panelTitle.GetComponent<CanvasGroup>().interactable && panelTitle.activeSelf) //checks if the title screen is currently active
        {
            if(Input.anyKey) //checks for any input
            {
                panelTitle.SetActive(false); //disables title screen
                MenuScreen(panelMain);
            }
        }

        
        
    }

    public void MenuScreen(GameObject targetPanel) //changes the current menu panel
    {
        ExitMenu(); //if there is an active panel, disable it
        targetPanel.SetActive(true); //activate the new panel
        panelCurrent = targetPanel;
    }

    public void ExitMenu()//closes current menu screen
    {
        if(!panelCurrent){return;} //if we have no active panel, return
        panelCurrent.SetActive(false);
        panelCurrent = null;
    }

    public void TitleAnimation() //starts an animation that ends with the title screen being visible
    {
        titleAnimator.SetTrigger("Start");
    }

    private void InitialisePanels()
    {
        //this is here in case there are static reference panels that need initialising
    }

    public void ShowTip(string tipKey) //starts a tip fade-in animation
    {
        tipAnimator = GameObject.Find("Txt_" + tipKey + "Tip").GetComponent<Animator>(); //assigns the appropriate animator
        tipAnimator.SetBool("Visible", true); //starts it's animaton
    }
    public void HideTip() //starts a tip fade-out animation
    {
        if(tipAnimator) //if the animator is not null
        {
            tipAnimator.SetBool("Visible", false); //starts the fade out - this only works if the animator has already been assigned using the ShowTip method
        }
        tipAnimator = null; //reset the animator to null
    }

    public void EscapeScreen() //called form game manager for escape phase
    {
        ShowTip("Escape"); //bring up esape text
        escapePanel.SetBool("Visible", true); //the escape panel animation
    }

    public void Outro() //called form game manager for outro phase
    {
        escapePanel.SetBool("Visible", false); //disable the black bars
        timeText.text = "Time: " + GameManager.timerString;

        MenuScreen(panelEndScreen); //show the end panel
        //switch to a zoomed out camera after a little bit or something
    }

}
