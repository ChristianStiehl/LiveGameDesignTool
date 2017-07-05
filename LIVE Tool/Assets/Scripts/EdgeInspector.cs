using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// The edge inspector
/// </summary>
public class EdgeInspector : MonoBehaviour
{
    public Transform canvas;
    public GameObject content;
    public ModelController mc;

    //The selected edge GameObject
    public GameObject selectedEdge;

    //InputField inspector elements
    public InputField label;
    public InputField source;
    public InputField exp;
    public InputField target;
    public Dropdown typedd;

    public string activeTabName;

    /// <summary>
    /// The inspector subscribes to the "tabAdded" event when created.
    /// </summary>
    private void Awake()
    {
        EventManager.StartListening("tabAdded", Deselect);
    }
    /// <summary>
    /// The inspector unsubscribes from the "tabAdded" event when destroyed.
    /// </summary>
    private void OnDestroy()
    {
        EventManager.StopListening("tabAdded", Deselect);
    }
    /// <summary>
    /// Subscribes the inspector to value changes of the inspector elements
    /// </summary>
    void Start()
    {
        label.onEndEdit.AddListener(delegate { UpdateName(); });
        source.onEndEdit.AddListener(delegate { UpdateSource(); });
        exp.onEndEdit.AddListener(delegate { UpdateExp(); });
        target.onEndEdit.AddListener(delegate { UpdateTarget(); });
    }
    /// <summary>
    /// Detects if the selected Edge is null (destroyed or deselected) and disables the content if it is.
    /// </summary>
    private void Update()
    {
        if (selectedEdge == null)
        {
            content.SetActive(false);
        }
        else
        {
            activeTabName = canvas.GetChild(canvas.childCount - 1).GetChild(1).GetComponentInChildren<Text>().text;
        }
    }
    /// <summary> 
    /// Selects given object in the edge inspector 
    /// </summary>
    /// <param name="selectedObject"> the object that needs to be inspected </param>
    public void Select(GameObject selectedObject)
    {
        content.SetActive(true);
        UpdateContent(selectedObject);
    }
    /// <summary>
    /// Clears selection of the Edge Inspector.
    /// </summary>
    public void Deselect()
    {
        selectedEdge = null;
        content.SetActive(false);
        label.text = "";
    }
    /// <summary>
    /// Updates the Name of the currently selected node.
    /// </summary>
    public void UpdateName()
    {
        if (selectedEdge)
        {
            if(label.text != "")
            {
                selectedEdge.GetComponent<SelectEdgeButton>().label.text = label.text;
                if (label.text.Contains("   ") || label.text.Contains(" "))
                {
                    Debug.Log("Error: Edge name cannot contain whitespace.");
                }
                mc.UpdateEdgeName(label.text, selectedEdge.GetComponent<SelectEdgeButton>().idedge, activeTabName);
            }
        }
    }
    /// <summary>
    /// Updates the Behavior component of the currently selected node.
    /// </summary>
    public void UpdateSource()
    {
        if (selectedEdge)
        {
            if(source.text != "")
            {
                if (!selectedEdge.GetComponent<SelectEdgeButton>().UpdateSource(source.text))
                {
                    source.text = selectedEdge.GetComponent<SelectEdgeButton>().source;
                    mc.UpdateEdgeSource(source.text, selectedEdge.GetComponent<SelectEdgeButton>().idedge, activeTabName);
                }
            }
        }
    }
    /// <summary>
    /// Updates the At component of the currently selected node.
    /// </summary>
    public void UpdateExp()
    {
        if (selectedEdge)
        {
            selectedEdge.GetComponent<SelectEdgeButton>().exp.text = exp.text;
            mc.UpdateEdgeExpression(exp.text, selectedEdge.GetComponent<SelectEdgeButton>().idedge, activeTabName);
        }
    }
    /// <summary>
    /// Updates the Max component of the currently selected node.
    /// </summary>
    public void UpdateTarget()
    {
        if (selectedEdge)
        {
            if(target.text != "")
            {
                if (!selectedEdge.GetComponent<SelectEdgeButton>().UpdateTarget(target.text))
                {
                    target.text = selectedEdge.GetComponent<SelectEdgeButton>().target;
                    mc.UpdateEdgeTarget(target.text, selectedEdge.GetComponent<SelectEdgeButton>().idedge, activeTabName);
                }
            }
        }
    }
    /// <summary>
    /// Update the values and states of the elements of the inspector. For example if a pool node is selected, the In Out dropdown should not be interactable.
    /// </summary>
    /// <param name="selectedObject"> The selected Game Object, needed to get some values like the source and target of the edge </param>
    private void UpdateContent(GameObject selectedObject)
    {
        if (selectedObject)
        {
            SelectEdgeButton seb = selectedObject.GetComponent<SelectEdgeButton>();

            if(seb.sourceObj != null)
            {
                if (seb.sourceObj.GetComponent<SelectNodeButton>())
                {
                    source.text = seb.sourceObj.GetComponent<SelectNodeButton>().label.text;
                }
                else
                {
                    source.text = seb.sourceObj.GetComponentInChildren<Text>().text;
                }
            }
            if(seb.targetObj != null)
            {
                if (seb.targetObj.GetComponent<SelectNodeButton>())
                {
                    target.text = seb.targetObj.GetComponent<SelectNodeButton>().label.text;
                }
                else
                {
                    target.text = seb.targetObj.GetComponentInChildren<Text>().text;
                }
            }
            exp.text = seb.exp.text;
            label.text = seb.label.text;
            typedd.value = (int)seb.type - 8; //type = enum, 8 & 10 are flow and state, edge type - 8 returns 0 for flow, edge type - 8 returns 1 for state

            selectedEdge = selectedObject;
            UpdateName();
            UpdateExp();
            UpdateSource();
            UpdateTarget();
        }
    }
}
