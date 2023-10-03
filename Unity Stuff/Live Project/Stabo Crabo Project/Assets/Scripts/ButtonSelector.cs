using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelector : MonoBehaviour
{
    Button button;
    void OnEnable()
    {
        button = GetComponent<Button>();
        button.Select();
    }
}
