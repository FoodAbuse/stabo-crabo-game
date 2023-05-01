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
    [SerializeField]
    private GameObject panelEndScreen;
    [SerializeField]
    private TextMeshProUGUI timeText; //shows the time on level completion

    //animators
    [SerializeField]
    private Animator titleAnimator;
    private Animator hintAnimator;
    [SerializeField]
    private Animator escapePanel;

    private GameObject panelCurrent; //the pannel that is currently active


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
        panelCurrent = panelTitle;
        
    }

    void Update()
    {
        if(panelTitle.GetComponent<CanvasGroup>().interactable && panelTitle.activeSelf) //checks if the title screen is currently active
        {
            if(Input.anyKey) //checks for any input
            {
                /*panelTitle.SetActive(false); //disables title screen
                panelMain.SetActive(true); //enables main menu
                panelCurrent = panelMain;*/
                MenuScreen(panelMain);
            }

        }

        if(Input.GetKey(KeyCode.Escape)) //when esc is pushed
        {
            //call menuscreen method passing pause panel
            //pause the game
        }
        
    }

    public void MenuScreen(GameObject targetPanel) //changes the current menu panel
    {
        panelCurrent.SetActive(false); //disable the currently active panel
        targetPanel.SetActive(true); //activate the new panel
        panelCurrent = targetPanel;
    }

    public void ExitMenu()//closes current menu screen
    {
        panelCurrent.SetActive(false);
    }

    public void TitleAnimation() //starts an animation that ends with the title screen being visible
    {
        titleAnimator.SetTrigger("Start");
    }

    private void InitialisePanels()
    {
        //this is here in case there are static reference panels that need initialising
    }

    public void ShowHint(string hintKey) //starts a hint fade-in animation
    {
        hintAnimator = GameObject.Find("Txt_" + hintKey + "Hint").GetComponent<Animator>(); //assigns the appropriate animator
        hintAnimator.SetBool("Visible", true); //starts it's animaton
    }
    public void HideHint() //starts a hint fade-out animation
    {
        if(hintAnimator) //if the animator is not null
        {
            hintAnimator.SetBool("Visible", false); //starts the fade out - this only works if the animator has already been assigned using the ShowHint method
        }
        hintAnimator = null; //reset the animator to null
    }

    public void EscapeScreen() //called form game manager for escape phase
    {
        ShowHint("Escape"); //bring up esape text
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
