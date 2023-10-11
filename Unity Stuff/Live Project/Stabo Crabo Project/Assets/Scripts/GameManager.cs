using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    //purpose of this is to manage the levels and the game itself
    //private static GameManager _instance; //the static holder for this script

    private PlayerControls controls;

    private static UIManager ui;
    
    //objective tracking
    private static List<GameObject> targetList; //holds all of the targets
    public Transform targetParent; //all the targets in the level will be children of this

    //tracking variables
    //public static Level levelCurrent;
    public static float timerCurrent;
    public static string timerString;
    public static int levelPhase; //0 - intro, 1 - puzzles, 2-escape, 3 - end
    public string levelName; //level naame specigic to this game manager
    
    public static bool acceptPlayerInput = true;

    //tip tracking
    private static List<string> tipList; //list of controls tips to send the player
    private static string currentTip;

    private Coroutine currentRoutine;

    // Currently used for deleting unused hair, can be modified to include other variables needing deleting later
    public GameObject[] toDelete;

    void Awake()
    {        
        Application.targetFrameRate = 60;        
        QualitySettings.vSyncCount = 1;
        controls = new PlayerControls();
        controls.Gameplay.Pause.performed += ctx => EscapeBtn();
        controls.Gameplay.Hints.performed += ctx => HintsBtn();
    }

    void OnEnable()
    {
        controls.Gameplay.Enable(); //enables all of our controls
    }

    void OnDisable()
    {
        controls.Gameplay.Disable(); //enables all of our controls
    }

    void Start()
    {
        ui = GameObject.Find("UIManager").GetComponent<UIManager>();

        timerCurrent = 0.0f;

        InitialiseTargets();//initialise target list
        levelPhase = 0; //at the moment skipping straight to 1 as there is no intro set up
        tipList = new List<string>{"Move", "Stab", "Grab", "Sprint"}; //populate the tipList
        acceptPlayerInput = false;


        // Clears unused hair from the scene
        toDelete = GameObject.FindGameObjectsWithTag("InactiveHair");
        
        for (int i = 0; i < toDelete.Length; i++)
        {
            Destroy(toDelete[i]);
        }
    }

    void Update()
    {
        KeepTime(); //tracks up time in level
    }

    void EscapeBtn() //when esc or 'start' are pressed
    {
        if(levelPhase == 0) //if in intro phase
        {
            StopCoroutine(currentRoutine);
            IntroCutSceneEnd(); //End the Intro CutScene

        }
        else if(ui.panelCurrent == ui.panelHints) //exit the Hints menu
        {
            ui.ExitMenu();
        }
        else
        {
            TogglePause(); //pause the game
        }
    }

    void HintsBtn() //when q or 'xbox y' are pressed
    {
        if(ui.panelCurrent == ui.panelHints)
        {
            ui.ExitMenu(); //if we are already in the tip menu, exit it
        }
        else if(!ui.panelCurrent) //only bring up hints if no other menus are open
        {
            ui.MenuScreen(ui.panelHints); //enable hints menu
        }
    }

    public static IEnumerator NextTip(string tip) //called by other objects
    {
        if(currentTip == tip) //if called tip is being shown
        {
            //if(tipList[0] == tip) //if my tip is at the start of the list
            //{
                ui.HideTip();
                tipList.Remove(tip);
                currentTip = null;
                if(tipList.Count != 0) //if there is anything left in the tip array
                {
                    yield return new WaitForSeconds(1.0f);
                    ui.ShowTip(tipList[0]); //show the next tip
                    currentTip = tipList[0];
                }
            //}
        }
    }

    private void KeepTime()//keeps track of the timer
    {
        if(acceptPlayerInput)//during the parts of the game where player can actually do things
        {
            timerCurrent += 1 * Time.deltaTime; //tick up the timer
        }
    }

    public void IntroCutSceneEnd()
    {
        acceptPlayerInput = true;
        levelPhase = 1;
        ui.ShowTip(tipList[0]); //show first tip
        currentTip = tipList[0];
        ui.hintIcon.SetActive(true);
    }

    public static void TogglePause() //at the moment NPCs and animations etc will not bne paused, just crab will
    {
        if(acceptPlayerInput) //if game is not paused
        {
            ui.MenuScreen(ui.panelPause); //bring up the pause screen
            acceptPlayerInput = false;
        }
        else
        {
            ui.ExitMenu(); //bring up the pause screen
            acceptPlayerInput = true;
        }            
    }

    public static void LevelWon() //called by escape zones to trigger the end of the level
    {
        Debug.Log("Level Won!");

        int tMinutes = (int)timerCurrent / 60; //divide the float by 60 and convert to integer
        int tSeconds = (int)timerCurrent % 60; //get the remainder after dividing by 60
        timerString = tMinutes.ToString("00") + ":" + tSeconds.ToString("00"); //set the formatting of the time
        
        levelPhase = 3; //enter the end phase
        acceptPlayerInput = false;

        //save the game

        //check if there is a next level, and unlock it

        ui.Outro(); //outro animation and UI changes
        ui.HideTip(); //hide the escape text tip

        //zoom out camera and play crab animation or something

        //trigger any end animation or transition
    }

    public static void TargetKilled(GameObject target) //called from the target as it dies
    {
        targetList.Remove(target); //remove the dead target from the target list
        if(targetList.Count <= 0) //if all the targets are now dead
        {
            levelPhase = 2; //move to the escape phase
            LevelPhaseEscape(); //run the escape method
        }

    }

    private void InitialiseTargets() //fetch all the target gameobjects
    {
        targetList = new List<GameObject>(); //create the list
        foreach(Transform child in targetParent)
        {
            Debug.Log(child);
            targetList.Add(child.gameObject); //add the target into the list
        }
    }

    private static void LevelPhaseEscape() //run when entering phase 2 - escape
    {
        //removes sprint tip, which is the only tip possible to beat the game without solving
        ui.HideTip();

        ui.EscapeScreen(); //handles whatever Ui elements are necessary for the escape sequence
        //also include any needed changes to NPC AI here.
        //and changes to audio etc.
    }


}
