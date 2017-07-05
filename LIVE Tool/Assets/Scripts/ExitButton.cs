using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Button that closes the tool
/// </summary>
public class ExitButton : MonoBehaviour {
    public Button yourButton;
    // delegate of the OnExit event from the ToggleTool script
    public delegate void exit();
    public static event exit OnExit;

    /// <summary>
    /// Subscribes the TaskOnClick function to the buttons onClick event
    /// </summary>
    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }
    /// <summary>
    /// When clicked activates the OnExit event (more information in ToggleTool script)
    /// </summary>
    void TaskOnClick()
    {
        if(OnExit != null)
        {
            OnExit();
        }
    }
}
