using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class Level //used for level attributes
{
    public string name;
    public int sceneIndex;
    public bool completed; //has the level ever been completed in this save file
    public bool locked;
    public float timerHighScore;
    public float timerLast; //the last time that was recorded
}*/

public class GameManager : MonoBehaviour
{
    //purpose of this is to manage the levels and the game itself
    //private static GameManager _instance; //the static holder for this script

    private static UIManager ui;

    [SerializeField]
    private CameraController cam;
    
    //objective tracking
    private static List<GameObject> targetList; //holds all of the targets
    public Transform targetParent; //all the targets in the level will be children of this

    //tracking variables
    //private static Level levelCurrent;
    public static float timerCurrent;
    public static string timerString;
    public static int levelPhase; //0 - intro, 1 - puzzles, 2-escape, 3 - end
    public string levelName; //level naame specigic to this game manager
    
    public static bool acceptPlayerInput = true;

    //hint tracking
    private static List<string> hintList;

    //level variables
    //private List<Level> levelList = new List<Level>();
    //Level firstLevel = new Level();

    /*void Awake()
	{
		if (_instance == null) //if no instance already exists
		{
			_instance = this; //store this level loader
			DontDestroyOnLoad(gameObject); //And protect it across scene loads
		}
		else
		{
			if (_instance != this) //Otherwise if there is a different level loader
				Destroy(gameObject); //Destroy this object, because it is a duplicate
		}
    }*/

    void Start()
    {
        ui = GameObject.Find("UIManager").GetComponent<UIManager>();

        //InitialiseLevels(); //load in level data and set all the variables
        //levelCurrent = firstLevel;
        timerCurrent = 0.0f; //this should move to a level load/start method

        InitialiseTargets();//initialise target list
        levelPhase = 0; //at the moment skipping straight to 1 as there is no intro set up
        hintList = new List<string>{"Move", "Stab", "Grab", "Sprint"}; //populate the hintList
        StartCoroutine(IntroCutScene()); //play the intro cut-scene




    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            TogglePause(); //pause the game
        }

        KeepTime(); //tracks up time in level
    }

    public static void NextHint(string hint) //called by other objects
    {
        if(hintList.Count != 0)
        {
            if(hintList[0] == hint)
            {
                ui.HideHint();
                hintList.Remove(hint);
                if(hintList.Count != 0) //if there is anything left in the hint array
                {
                    ui.ShowHint(hintList[0]); //show the next hint
                }
            }
        }
    }

    private void KeepTime()//keeps track of the timer
    {
        if(acceptPlayerInput)//during the parts of the game where player can actually do things
        {
            timerCurrent += 1 * Time.deltaTime; //tick up the timer
        }
    }

    private IEnumerator IntroCutScene() //called when the level is loaded. At the end it should set level phase to 1.
    {
        acceptPlayerInput = false;
        foreach(GameObject target in targetList) //turn on all identifiers
        {
            target.GetComponent<NPCController>().ToggleIdentify();
        }
        yield return new WaitForSeconds(3.0f);
        cam.SwitchCamera(1);
        yield return new WaitForSeconds(3.0f);
        cam.SwitchCamera(2);
        yield return new WaitForSeconds(1.0f);
        acceptPlayerInput = true;
        foreach(GameObject target in targetList) //turn off all identifiers
        {
            target.GetComponent<NPCController>().ToggleIdentify();
        }
        ui.ShowHint(hintList[0]); //show first hint
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
        //levelCurrent.completed = true; //the player has now completed this level
        //levelCurrent.timerLast = timerCurrent; //store the time
        /*if(timerCurrent < levelCurrent.timerHighScore)//if the current time is better than the previous one
        {
            Debug.Log("new highscore!");
            levelCurrent.timerHighScore = timerCurrent; //override the highscore
        }*/
        //save the game

        //check if there is a next level, and unlock it

        ui.Outro(); //outro animation and UI changes
        ui.HideHint(); //hide the escape text hint

        //zoom out camera and play crab animation or something

        //trigger any end animation or transition

        //tell level loader to return to the menu


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

    /*private void InitialiseLevels() //load in level data and set all the variables
    {
        levelList.Add(firstLevel); //add this level to the array
        firstLevel.name = "Crabo's revenge";
        firstLevel.sceneIndex = 1;
        firstLevel.completed = false; //in future load this from save data
        firstLevel.locked = false; //same as above
        firstLevel.timerHighScore = 999999.0f; //above
        firstLevel.timerLast = 999999.0f; //above
    }*/

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
        ui.EscapeScreen(); //handles whatever Ui elements are necessary for the escape sequence
        //also include any needed changes to NPC AI here.
        //and changes to audio etc.

        //zoom in camera - look at how to call cinemachine commands for this
        //will also need it for managing intro pan from crab to target and back to crab
    }

}
