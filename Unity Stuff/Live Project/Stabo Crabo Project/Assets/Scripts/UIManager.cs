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

    [SerializeField]
    private Animator titleAnimator;

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
        
    }

    void Update()
    {
        if(panelTitle.GetComponent<CanvasGroup>().interactable && panelTitle.activeSelf) //checks if the title screen is currently active
        {
            if(Input.anyKey) //checks for any input
            {
                panelTitle.SetActive(false); //disables title screen
                panelMain.SetActive(true); //enables main menu
                panelCurrent = panelMain;
            }

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

}
