using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ApplicationManager : MonoBehaviour
{
    //purpose of this manager is to manage saving / loading, quiting, etc.
    private static ApplicationManager _instance; // the manager variable
	public static bool isKeyboardAndMouse;

    void Awake()
	{
		Application.targetFrameRate = 60;
		QualitySettings.vSyncCount = 1;

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
		//InputSystem.onActionChange += InputChange; //called whenever a change in input is detected
	}

	//for some reason the controller constantly outputs changes if plugged in, prevent use of keybvoard hints. might be stick drift as cause
	/*private void InputChange(object obj, InputActionChange change)
	{
		if(change == InputActionChange.ActionPerformed)
		{
			InputAction receivedAction = (InputAction) obj;
			InputDevice lastDevice = receivedAction.activeControl.device;
			Debug.Log(lastDevice.name);
			isKeyboardAndMouse = lastDevice.name.Equals("Keyboard") || lastDevice.name.Equals("Mouse");
		}

	}*/

    public void ExitApplication()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }


}
