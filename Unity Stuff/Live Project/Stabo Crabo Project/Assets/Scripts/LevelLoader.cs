using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private static LevelLoader _instance; //the static holder for this script

    public Animator transition; //which transition is being used
    public float transitionPause = 1.0f; //timing between the fade in, and fade out
    public float transitionSpeed = 1.0f; //timing for the actual fading

    void Awake()
	{
		if (_instance == null) //if no instance already exists
		{
			_instance = this; //store this leavel loader
			DontDestroyOnLoad(gameObject); //And protect it across scene loads
		}
		else
		{
			if (_instance != this) //Otherwise if there is a different level loader
				Destroy(gameObject); //Destroy this object, because it is a duplicate
		}
    }

    public void LoadNextLevel(string levelName) //function accessible to other scripts
    {
        StartCoroutine(LoadLevel(levelName));

    }

    IEnumerator LoadLevel(string levelName)
    {
        transition.speed = transitionSpeed; //set the speed of the animation
        transition.SetBool("Start", true); //play animation - fade to black
        yield return new WaitForSeconds(transitionPause); //pauses the co-routine for x amount of seconds
        SceneManager.LoadScene(levelName); //load the scene
        transition.SetBool("Start", false); //play animation - fade from black
    }
}
