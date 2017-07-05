using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Enum for different types of messages
/// </summary>
public enum ErrorType
{
    error,
    warning,
    info,
    message
}

/// <summary>
/// WIP
/// Debugging console of the tool
/// </summary>
public class ConsoleScript : MonoBehaviour
{
    public Text consoleText;
    public Button consoleButton;
    public GameObject errorTab;

    private void Start()
    {
        consoleButton.onClick.AddListener(GoToError);
    }

    /// <summary>
    /// Displays the error message in the console
    /// </summary>
    /// <param name="type">Type of the error</param>
    /// <param name="text">The error message</param>
    /// <param name="tab">Source of the error (definition)</param>
    /// <param name="objects"></param>
    public void DisplayError(ErrorType type, string text, GameObject tab, List<GameObject> objects)
    {
        consoleText.text = "Console: " +type.ToString() +": " +text;
        errorTab = tab;
        switch (type)
        {
            case ErrorType.error:
                consoleText.color = Color.red;
                break;
            case ErrorType.warning:
                consoleText.color = Color.yellow;
                break;
            case ErrorType.info:
                consoleText.color = Color.blue;
                break;
            case ErrorType.message:
                consoleText.color = Color.black;
                break;
        }
    }

    public void GoToError()
    {
        if (errorTab)
        {
            errorTab.transform.SetAsLastSibling();
        }
    }
}
