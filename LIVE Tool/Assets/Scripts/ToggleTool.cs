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


    private void Start()
    {
        Screen.SetResolution(640, 326, false);
    }
    private void OnEnable()
    {
        ExitButton.OnExit += HandleOnExit;
    }
    
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
