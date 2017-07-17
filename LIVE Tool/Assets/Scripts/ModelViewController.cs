using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
/// <summary>
/// Enum for the behavior types of a Node (0 is an empty behavior used for the mouse pointer option).
/// For more information about Behaviors, When, Act, How and IO types please refer to research by Riemer van Rozen
/// </summary>
public enum Behavior
{
    Mouse,
    Pool,
    Gate,
    Source,
    Drain,
    Converter,
    Reference,
    Definition,
    Flow,
    State
}
/// <summary>
/// Enum for the When types of a Node.
/// </summary>
public enum When
{
    Passive,
    User,
    Auto,
    Start
}

/// <summary>
/// Enum for the Act types of a Node.
/// </summary>
public enum Act
{
    Pull,
    Push
}

/// <summary>
/// Enum for the How types of a Node.
/// </summary>
public enum How
{
    Any,
    All
}

/// <summary>
/// Enum for the In Out types of a Node.
/// </summary>
public enum IO
{
    Internal,
    In,
    Out,
    InOut

}

/// <summary>
/// Model View Controller for the tool, links the visual (view) with the functional (model) elements of the diagrams
/// </summary>
public class ModelViewController : MonoBehaviour
{
    // A basic node to be instantiated
    public GameObject nodePrefab;
    // A basic edge to be instantiated
    public GameObject edgePrefab;
    // A basic state to be instantiated
    public GameObject statePrefab;
    public Transform canvas;
    public Transform view;
    public Transform grid;
    public RectTransform gridTransform;
    // The selected type to be instantiated. Default is mouse (to navigate the diagram and selected nodes)
    public Behavior selectedType = Behavior.Mouse;
    public ScrollRect scrollRect;
    public ModelController model;
    // boolean used for the creation of multi corner edges
    private bool addingToEdge = false;
    // boolean used for the creation of multi corner states
    private bool addingToState = false;
    public GameObject tempEdge;
    public GameObject tempState;
    public bool somethingSelected = false;
    public GameObject selectedObject;
    public NodeInspector nodeInspector;
    public EdgeInspector edgeInspector;
    // temporary GameObject used for copy/pasting
    public GameObject copiedNode;
    public GameObject tempNode;
    public RectTransform tab;
    public bool editingEnabled = true;
    /// <summary>
    /// Finds some UI elements of the tool (canvas and node inspector) and starts listening to the "tabSelected" event
    /// </summary>
    private void Awake()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas").transform;
        scrollRect = gameObject.GetComponent<ScrollRect>();
        nodeInspector = GameObject.FindGameObjectWithTag("Inspector").GetComponent<NodeInspector>();
        edgeInspector = GameObject.FindGameObjectWithTag("EdgeInspector").GetComponent<EdgeInspector>();
        EventManager.StartListening("tabSelected", HideInspector);
        model = GameObject.FindGameObjectWithTag("Tool").GetComponent<ModelController>();
    }

    /// <summary>
    /// stops listening to the "tabSelected" event
    /// </summary>
    private void OnDestroy()
    {
        EventManager.StopListening("tabSelected", HideInspector);
    }

    /// <summary>
    /// Checks if this grid is the active grid
    /// Checks for keyboard shortcuts for cut/copy/paste actions
    /// Checks for mouse inputs to select or instantiate objects
    /// </summary>
    void Update ()
    {
        if (editingEnabled)
        {
            if (canvas.GetChild(canvas.childCount - 1) == grid)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.X))
                {
                    if (copiedNode)
                    {
                        Destroy(copiedNode);
                    }
                    if (selectedObject)
                    {
                        copiedNode = Instantiate(selectedObject);
                        copiedNode.GetComponent<SelectNodeButton>().isSelected = false;
                        copiedNode.tag = "Copy";
                        Destroy(selectedObject);
                    }
                }
                else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
                {
                    if (copiedNode)
                    {
                        Destroy(copiedNode);
                    }
                    if (selectedObject)
                    {
                        copiedNode = Instantiate(selectedObject);
                        copiedNode.GetComponent<SelectNodeButton>().isSelected = false;
                        copiedNode.tag = "Copy";
                    }
                }
                else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
                {
                    if (copiedNode)
                    {
                        if (isMouseOverGrid())
                        {
                            if (selectedObject)
                            {
                                selectedObject.GetComponent<SelectNodeButton>().Deselect();
                                nodeInspector.Deselect();
                            }
                            GameObject pastedObj = Instantiate(copiedNode, Input.mousePosition, grid.transform.rotation, view) as GameObject;
                            pastedObj.GetComponent<SelectNodeButton>().isSelected = true;
                            nodeInspector.Select(pastedObj);
                            pastedObj.tag = "Node";
                            pastedObj.GetComponent<SelectNodeButton>().label.text += "(copy)";
                            pastedObj.GetComponent<RectTransform>().SetAsLastSibling();
                        }
                    }
                    else if (GameObject.FindGameObjectWithTag("Copy"))
                    {
                        copiedNode = GameObject.FindGameObjectWithTag("Copy");
                        if (isMouseOverGrid())
                        {
                            if (selectedObject)
                            {
                                selectedObject.GetComponent<SelectNodeButton>().Deselect();
                                nodeInspector.Deselect();
                            }
                            GameObject pastedObj = Instantiate(copiedNode, Input.mousePosition, grid.transform.rotation, view) as GameObject;
                            pastedObj.GetComponent<SelectNodeButton>().isSelected = true;
                            nodeInspector.Select(pastedObj);
                            pastedObj.tag = "Node";
                            pastedObj.GetComponent<SelectNodeButton>().label.text += "(copy)";
                            pastedObj.GetComponent<RectTransform>().SetAsLastSibling();
                        }
                    }
                }
                if (Input.GetMouseButtonDown(0) && isMouseOverGrid())
                {
                    EventManager.TriggerEvent("gridSelected");
                    somethingSelected = false;
                    GetComponent<ScrollRect>().enabled = true;
                    nodeInspector.Deselect();
                    edgeInspector.Deselect();
                }
                if (Input.GetMouseButtonDown(0) && !isMouseNotOverChildren(true))
                {
                    if (selectedType != Behavior.Flow && selectedType != Behavior.State)
                    {
                        if (selectedObject.GetComponent<SelectNodeButton>())
                        {
                            selectedObject.GetComponent<SelectNodeButton>().isSelected = true;
                            nodeInspector.Select(selectedObject);
                            edgeInspector.Deselect();
                            selectedObject.GetComponent<RectTransform>().SetAsLastSibling();
                            GetComponent<ScrollRect>().enabled = false;
                        }
                    }
                }
                if (selectedType != Behavior.Mouse)
                {
                    if (Input.GetMouseButtonDown(1) && isMouseOverGrid())
                    {
                        nodeInspector.Deselect();
                        ChangeType(Behavior.Mouse);
                    }
                    Vector3 mousePos = Input.mousePosition;
                    if (Input.GetMouseButtonDown(0) && isMouseOverGrid())
                    {
                        if (selectedType == Behavior.Flow)
                        {
                            if (!addingToEdge)
                            {
                                if (!isMouseNotOverChildren(true))
                                {
                                    AddEdge(mousePos);
                                    tempEdge.GetComponent<SelectEdgeButton>().AddEdgePoint(mousePos, selectedObject);
                                    tempEdge.transform.SetAsFirstSibling();
                                    addingToEdge = true;
                                }
                                else
                                {
                                    AddEdge(mousePos);
                                    tempEdge.GetComponent<SelectEdgeButton>().AddEdgePoint(mousePos, null);
                                    tempEdge.transform.SetAsFirstSibling();
                                    addingToEdge = true;
                                }

                            }
                            else
                            {
                                if (!isMouseNotOverChildren(true))
                                {
                                    tempEdge.GetComponent<SelectEdgeButton>().AddEdgePoint(mousePos, selectedObject);
                                    ChangeType(Behavior.Mouse);
                                }
                                else
                                {
                                    tempEdge.GetComponent<SelectEdgeButton>().AddEdgePoint(mousePos, null);
                                }
                            }
                        }
                        else if (selectedType == Behavior.State)
                        {
                            if (!addingToState)
                            {
                                if (!isMouseNotOverChildren(true))
                                {
                                    AddState(mousePos);
                                    tempState.GetComponent<SelectEdgeButton>().AddEdgePoint(mousePos, selectedObject);
                                    tempState.transform.SetAsFirstSibling();
                                    selectedObject = tempState;
                                    addingToState = true;
                                }
                                else
                                {
                                    AddState(mousePos);
                                    tempState.GetComponent<SelectEdgeButton>().AddEdgePoint(mousePos, null);
                                    tempState.transform.SetAsFirstSibling();
                                    addingToState = true;
                                }
                            }
                            else
                            {
                                if (!isMouseNotOverChildren(true))
                                {
                                    tempState.GetComponent<SelectEdgeButton>().AddEdgePoint(mousePos, selectedObject);
                                    ChangeType(Behavior.Mouse);
                                }
                                else
                                {
                                    tempState.GetComponent<SelectEdgeButton>().AddEdgePoint(mousePos, null);
                                }
                            }
                        }
                        else if (isMouseNotOverChildren(false))
                        {
                            AddNode(selectedType, mousePos);
                            model.AddNode(selectedType, mousePos, tab.GetComponentInChildren<Text>().text, tempNode);
                            nodeInspector.Select(tempNode);
                            selectedObject = tempNode;
                            tempNode.GetComponent<SelectNodeButton>().isSelected = true;
                        }
                    }
                }
            }
        }
	}//end Update

    /// <summary>
    /// Changes the selected type of the MVC
    /// </summary>
    /// <param name="type"> The new type </param>
    public void ChangeType(Behavior type)
    {
        selectedType = type;
        EventManager.TriggerEvent("gridSelected");
        somethingSelected = false;
        if (addingToEdge || addingToState)
        {
            if (addingToEdge)
            {
                addingToEdge = false;
                tempEdge.GetComponent<SelectEdgeButton>().StopCurrentEdge();
                model.AddEdge(Behavior.Flow, tab.GetComponentInChildren<Text>().text, tempEdge);
                edgeInspector.Select(tempEdge);
                tempEdge = null;
            }
            if (addingToState)
            {
                addingToState = false;
                tempState.GetComponent<SelectEdgeButton>().StopCurrentEdge();
                model.AddEdge(Behavior.State, tab.GetComponentInChildren<Text>().text, tempState);
                edgeInspector.Select(tempState);
                tempState = null;
            }
        }
        if(type == Behavior.Mouse)
        {
            GetComponent<ScrollRect>().enabled = true;
        }
        else
        {
            GetComponent<ScrollRect>().enabled = false;
        }
    }//end ChangeType

    /// <summary>
    /// Checks if the mouse is over the transform of the grid
    /// </summary>
    /// <returns> returns true if the mouse is over the grid and returns false if it is not</returns>
    public bool isMouseOverGrid()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector3[] worldCorners = new Vector3[4];
        gridTransform.GetWorldCorners(worldCorners);

        if (mousePosition.x >= worldCorners[0].x && mousePosition.x < worldCorners[2].x
           && mousePosition.y >= worldCorners[0].y && mousePosition.y < worldCorners[2].y)
        {
            return true;
        }  
        else
        {
            return false;
        }
    }//end isMouseOverGrid

    /// <summary>
    /// Checks if the mouse is not over one of the Nodes
    /// </summary>
    /// <param name="select"> If the mouse is over a node, should that node be selected? </param>
    /// <returns> True if mouse is not over any children, False if the mouse is over a child </returns>
    public bool isMouseNotOverChildren(bool select)
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        Vector3 mousePosition = Input.mousePosition;
        float imageBuffer = 25 * view.localScale.x;
        for (int i = 0; i < nodes.Length; i++)
        {
            if(nodes[i].transform.parent == view)
            {
                RectTransform temp = nodes[i].GetComponent<RectTransform>();
                if (mousePosition.x >= temp.position.x - imageBuffer && mousePosition.x <= temp.position.x + imageBuffer && mousePosition.y >= temp.position.y - imageBuffer && mousePosition.y <= temp.position.y + imageBuffer)
                {
                    if (select)
                    {
                        selectedObject = nodes[i];
                    }
                    return false;
                }
            }
        }
        
        GameObject[] interfaces = GameObject.FindGameObjectsWithTag("ReferenceInterface");
        for (int i = 0; i < interfaces.Length; i++)
        {
            if (interfaces[i].transform.parent.parent.parent == view)
            {
                RectTransform temp = interfaces[i].GetComponent<RectTransform>();
                //TODO this check does not wrok properly
                if (mousePosition.x >= temp.position.x - imageBuffer && mousePosition.x <= temp.position.x + imageBuffer && mousePosition.y >= temp.position.y - imageBuffer && mousePosition.y <= temp.position.y + imageBuffer)
                {
                    if (select)
                    {
                        selectedObject = interfaces[i];
                    }
                    return false;
                }
            }
        }
        return true;
    }//end isMouseNotOverChildren

    /// <summary>
    /// Function to hide the node and endge inspectors
    /// </summary>
    public void HideInspector()
    {
        if (selectedObject)
        {
            if (selectedObject.GetComponent<SelectNodeButton>())
            {
                selectedObject.GetComponent<SelectNodeButton>().isSelected = false;
            }
            else
            {
                selectedObject.GetComponent<SelectEdgeButton>().isSelected = false;
            }
            selectedObject = null;
        }
        nodeInspector.Deselect();
        edgeInspector.Deselect();
    }
    /// <summary>
    /// Tells the edge inspector to select the given object and tells the node inspector to deselect.
    /// </summary>
    /// <param name="selectedEdge">The selected edge</param>
    public void SelectEdge(GameObject selectedEdge)
    {
        selectedObject = selectedEdge;
        edgeInspector.Select(selectedObject);
        nodeInspector.Deselect();
    }
    /// <summary>
    /// Adds a visual node to the diagram 
    /// </summary>
    /// <param name="newbehavior">The behavior of the new node</param>
    /// <param name="pos">The position of the new node</param>
    public void AddNode(Behavior newbehavior, Vector2 pos)
    {
        //add the visual node to the view
        tempNode = Instantiate(nodePrefab, pos, grid.transform.rotation, view) as GameObject;
        //set the visual and type of the node to the selected node
        tempNode.GetComponent<SelectNodeButton>().ChangeBehavior(newbehavior);
    }
    /// <summary>
    /// Adds a visual flow edge to the diagram
    /// </summary>
    /// <param name="pos">Starting position of the new flow edge</param>
    public void AddEdge(Vector2 pos)
    {
        tempEdge = Instantiate(edgePrefab, pos, grid.transform.rotation, view) as GameObject;
    }
    /// <summary>
    /// Adds a visual state edge to the diagram
    /// </summary>
    /// <param name="pos">Starting position of the new state edge</param>
    public void AddState(Vector2 pos)
    {
        tempState = Instantiate(statePrefab, pos, grid.transform.rotation, view) as GameObject;
    }

    public void DisableEditing()
    {
        editingEnabled = false;
        scrollRect.enabled = false;
        view.GetComponent<ZoomContentScript>().enabled = false;
        HideInspector();
        DestroyUnfinishedEdges();
    }

    public void EnableEditing()
    {
        editingEnabled = true;
        scrollRect.enabled = true;
        view.GetComponent<ZoomContentScript>().enabled = true;
        selectedType = Behavior.Mouse;
    }

    public void DestroyUnfinishedEdges()
    {
        if (tempEdge)
        {
            Destroy(tempEdge);
        }
        if (tempState)
        {
            Destroy(tempState);
        }
        addingToEdge = false;
        addingToState = false;
    }
}