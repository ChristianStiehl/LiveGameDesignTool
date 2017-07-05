using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script manages the tool enabling and disabling (toggling)
/// </summary>
public class ToggleTool : MonoBehaviour
{
    public bool toolEnabled = true;

    /// <summary>
    /// Important: This function sets the resolution of the tool to 640x326! Remove once other resolutions are supported
    /// </summary>
    private void Start()
    {
        //sets resolution to 640x326. Currently this is the only supported resolution.
        Screen.SetResolution(640, 326, false);
    }
    private void OnEnable()
    {
        ExitButton.OnExit += HandleOnExit;
    }
    /// <summary>
    /// Checks for keyboard command CTRL + M which toggles the tool on/off (only useful when intigrating the tool into a game).
    /// </summary>
	void Update () {
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.M))
        {
            HandleOnExit();
        }
    }

    public void HandleOnExit()
    {
        for (int i = 0; i < this.transform.childCount; ++i)
        {
            this.transform.GetChild(i).gameObject.SetActive(!toolEnabled);
        }
        toolEnabled = !toolEnabled;
    }
}
