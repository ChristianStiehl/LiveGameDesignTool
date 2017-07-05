using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Button script for the tool selection buttons in the editor
/// </summary>
public class EditorButtonScript : MonoBehaviour {
    public Button yourButton;
    /// <summary>
    /// This enum is used as the type of tool the ModelViewController is using
    /// </summary>
    public Behavior type;
    public Transform canvas;
    public Transform currentGrid;

    /// <summary>
    /// Subscribes the OnClick function to the buttons onClick event
    /// </summary>
    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    /// <summary>
    /// Makes sure the currently active grid is up to date
    /// </summary>
    private void Update()
    {
        currentGrid = canvas.GetChild(canvas.childCount-1);
    }
    /// <summary>
    /// When clicked this button tells the ModelViewController what tool (type) is selected.
    /// </summary>
    void OnClick()
    {
        if (currentGrid.GetComponentInChildren<ModelViewController>())
        {
            currentGrid.GetComponentInChildren<ModelViewController>().ChangeType(type);
        }
    }
}
