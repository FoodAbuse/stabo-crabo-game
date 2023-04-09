using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    //purpose of this manager is to manage saving / loading, quiting, etc.
    private static ApplicationManager _instance; // the manager variable

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

    public void ExitApplication()
    {
        Application.Quit(); //close the app
    }


}
