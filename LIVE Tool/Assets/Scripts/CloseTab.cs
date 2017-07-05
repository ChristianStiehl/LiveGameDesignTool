using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Button that closes its tab when clicked
/// </summary>
public class CloseTab : MonoBehaviour {
    public Button yourButton;
    public GameObject parent;
    public ModelController mc;

    /// <summary>
    /// Subscribes the OnClick function to the button on click.
    /// </summary>
    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
        mc = GameObject.FindGameObjectWithTag("Tool").GetComponent<ModelController>();
    }

    /// <summary>
    /// When this object is destroyed it should notify other tabs that a tab has been removed.
    /// </summary>
    private void OnDestroy()
    {
        if(EventManager.eventManager != null)
        {
            EventManager.TriggerEvent("tabRemoved");
        }
    }
    /// <summary>
    /// When clicked this button destroys the tab (its parent).
    /// </summary>
    void OnClick()
    {
        mc.DeleteDefinition(parent.GetComponentInChildren<Text>().text);
        Destroy(parent);
    }
}
