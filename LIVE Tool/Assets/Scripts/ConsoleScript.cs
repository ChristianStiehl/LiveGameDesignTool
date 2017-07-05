using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ErrorType
{
    error,
    warning,
    info,
    message
}

public class ConsoleScript : MonoBehaviour
{
    public Text consoleText;
    public Button consoleButton;
    public GameObject errorTab;

    private void Start()
    {
        consoleButton.onClick.AddListener(GoToError);
    }

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
