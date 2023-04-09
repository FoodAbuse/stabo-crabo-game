using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    //purpose of this is to manage UI transitions between scenes, and the actual loading of those scenes
    private static LevelLoader _instance; //the static holder for this script

    public Animator transition; //which transition is being used
    public float transitionPause = 1.0f; //timing between the fade in, and fade out
    public float openingFade = 3.0f; //speed of the fade at the start of the game
    public float transitionSpeed = 3.0f; //speed of the fade between scenes
    public UIManager uiManager; //reference the UI manager

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
        //when the game first starts, it will play a fade-in animation
        transition.SetFloat("TransitionSpeed", 1/openingFade);
        StartCoroutine(InitialiseTitle()); //after the animation, set the title to be active

    }

    public void LoadNextLevel(string levelName) //function accessible to other scripts
    { //might eventually change level name to level index??
        StartCoroutine(LoadLevel(levelName, transitionSpeed));

    }

    IEnumerator LoadLevel(string levelName, float transitionSpeed)
    {
        transition.SetFloat("TransitionSpeed", 1/transitionSpeed); //set the speed of the animation
        transition.SetBool("Start", true); //play animation - fade to black
        yield return new WaitForSeconds(transitionPause); //pauses the co-routine for x amount of seconds
        SceneManager.LoadScene(levelName); //load the scene
        transition.SetBool("Start", false); //play animation - fade from black
    }

    IEnumerator InitialiseTitle()
    {
        yield return new WaitForSeconds(2.0f); //waits for the time it will take the initial fade to end
        uiManager.TitleAnimation();
    }
}
