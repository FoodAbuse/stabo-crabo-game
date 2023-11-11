using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    //purpose of this manager is to run menus, tooltips most of the UI.

    private PlayerControls controls;
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

    //[HideInInspector]
    public string levelName;

    [SerializeField]
    private GameObject endL1; //panel content to be shown at the end of level 1
    [SerializeField]
    private GameObject endL2;
    [SerializeField]
    private GameObject endL3;


    void Awake()
	{
		/*if (_instance == null) //if no manager already exists
		{
			_instance = this; //store this manager
			DontDestroyOnLoad(gameObject); //And protect it across scene loads
		}
		else
		{
			if (_instance != this) //Otherwise if there is a different manager
				Destroy(gameObject); //Destroy this object, because it is a duplicate
		}*/

        controls = new PlayerControls();
        controls.Gameplay.UiStart.performed += ctx => IntroBtn();
    }

    void OnEnable()
    {
        controls.Gameplay.Enable(); //enables all of our controls
    }

    void OnDisable()
    {
        //disabled as was causing error
        //controls.Gameplay.Disable(); //enables all of our controls
    }

    void Start()
    {
        InitialisePanels();//initial set of UI variables
        //panelCurrent = panelTitle;
    }

    void IntroBtn()
    {
        if(panelTitle.GetComponent<CanvasGroup>().interactable && panelTitle.activeSelf) //checks if the title screen is currently active
        {
            panelTitle.SetActive(false); //disables title screen
            MenuScreen(panelMain);
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
        endL1.SetActive(false);
        endL2.SetActive(false);
        endL3.SetActive(false);
        if(levelName == "Level1")
        {
            endL1.SetActive(true);
        }
        else if(levelName == "Level2")
        {
            endL2.SetActive(true);
        }
        else if(levelName == "Level3")
        {
            endL3.SetActive(true);
        }
        //switch to a zoomed out camera after a little bit or something
    }

}
