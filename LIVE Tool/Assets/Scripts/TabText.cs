using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script disables the input field of the tab unless it is the active tab.
/// </summary>
public class TabText : MonoBehaviour {
    public GameObject parent;
    private Transform canvas;
    public InputField ipf;
    public string oldName;
    public string newName;
    public ModelController model;

    void Start()
    {
        canvas = parent.transform.parent;
        model = GameObject.FindGameObjectWithTag("Tool").GetComponent<ModelController>();
        ipf.onEndEdit.AddListener(delegate { UpdateDefinition(); });
    }

    /// <summary>
    /// Updates the definition name in the model
    /// </summary>
    public void UpdateDefinition()
    {
        newName = this.GetComponent<Text>().text.ToString();
        if(newName.Contains("   ") || newName.Contains(" "))
        {
            Debug.Log("Error: Definition name cannot contain whitespace.");
        }
        model.UpdateDefinition(oldName, newName);
        oldName = newName;
    }

    private void FixedUpdate()
    {
        if (parent.transform.GetSiblingIndex() == canvas.transform.childCount - 1 && this.GetComponent<Text>().text != "Global")
        {
            this.GetComponent<InputField>().enabled = true;
        }
        else
        {
            this.GetComponent<InputField>().enabled = false;
        }
    }
}
