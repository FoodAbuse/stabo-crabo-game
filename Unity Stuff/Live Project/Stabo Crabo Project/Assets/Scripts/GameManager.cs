using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level //used for level attributes
{
    public string name;
    public int sceneIndex;
    public bool completed; //has the level ever been completed in this save file
    public bool locked;
    public float timerHighScore;
    public float timerLast; //the last time that was recorded
}

public class GameManager : MonoBehaviour
{
    //purpose of this is to manage the levels and the game itself
    private static GameManager _instance; //the static holder for this script

    //tracking variables
    private static Level levelCurrent;
    public static float timerCurrent;
    public static int levelPhase; //0 - intro, 1 - puzzles, 2-escape, 3 - end

    //level variables
    private List<Level> levelList = new List<Level>();
    Level firstLevel = new Level();

    void Awake()
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
    }

    void Start()
    {
        InitialiseLevels(); //load in level data and set all the variables
        levelCurrent = firstLevel;
        timerCurrent = 0.0f; //this should move to a level load/start method

    }

    void Update()
    {
        //testing purposes area
        if(Input.GetKey("k"))
        {
            levelPhase = 2;
        }

        //actual area
        KeepTime();
    }

    private void KeepTime()//keeps track of the timer
    {
        if(levelPhase == 1 || levelPhase == 2)//during the gameplay part of the level
        {
            timerCurrent += 1 * Time.deltaTime; //tick up the timer
        }
    }

    public static void LevelWon() //called by escape zones to trigger the end of the level
    {
        Debug.Log("Level Won!");
        Debug.Log("finished in " + timerCurrent);
        levelPhase = 3; //enter the end phase
        levelCurrent.completed = true; //the player has now completed this level
        levelCurrent.timerLast = timerCurrent; //store the time
        if(timerCurrent < levelCurrent.timerHighScore)//if the current time is better than the previous one
        {
            Debug.Log("new highscore!");
            levelCurrent.timerHighScore = timerCurrent; //override the highscore
        }
        //save the game

        //check if there is a next level, and unlock it
        
        //enable UI to show endscreen stats

        //trigger any end animation or transition

        //tell level loader to return to the menu or level select or whatever


    }

    private void InitialiseLevels() //load in level data and set all the variables
    {
        levelList.Add(firstLevel); //add this level to the array
        firstLevel.name = "Crabo's revenge";
        firstLevel.sceneIndex = 1;
        firstLevel.completed = false; //in future load this from save data
        firstLevel.locked = false; //same as above
        firstLevel.timerHighScore = 999999.0f; //above
        firstLevel.timerLast = 999999.0f; //above
    }
}
