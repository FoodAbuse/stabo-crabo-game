using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{

    AudioSource audioSource;

    private bool audioActive;
    // Start is called before the first frame update
    void Start()
    {
        // Fetching AudioSource from GameObject
        audioActive = false;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            audioActive = true;
            Debug.Log("Audio Source Activated");
            PlaySound();
        }
    }

    void PlaySound()
    {
        if (audioActive)
        {
            audioSource.Play();
            audioActive = false;
        }
    }
}
