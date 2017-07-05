using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MM.Model;

/// <summary>
/// Stores and updates the visual elements of a node
/// </summary>
public class SelectNodeButton : MonoBehaviour {
    //The Behavior of this Node
    public Behavior type = Behavior.Pool;
    // Array of sprites containing the node visuals
    public Sprite[] sprites;
    // Array of sprites containing the user activated node visuals.
    public Sprite[] interactibleSprites;
    // Array of sprites containing the in/out arrow visuals.
    public Sprite[] inoutArrows;
    public Image nodeImage;
    public Image inoutImage;
    public bool isSelected = false;
    public Text label;
    public Text activationText;
    public Text pullModeText;
    public Text numberText;
    public Text maxText;
    public int number;
    public int capacity = 0;
    public string add = "";
    public string ofType = "";
    public How how = How.Any;
    public Act act = Act.Pull;
    public When when = When.Passive;
    public IO inout = IO.Internal;
    public GameObject definition = null;
    public GameObject definitionPrefab;
    public bool drag = false;
    public MM.Model.Node idNode = null;

    /// <summary>
    /// Starts listening to events
    /// </summary>
    private void Awake()
    {
        EventManager.StartListening("newNodeSelected", Deselect);
        EventManager.StartListening("gridSelected", Deselect);
    }

    /// <summary>
    /// Stops listening to events
    /// </summary>
    private void OnDestroy()
    {
        EventManager.StopListening("newNodeSelected", Deselect);
        EventManager.StopListening("gridSelected", Deselect);
    }

    /// <summary>
    /// Updates the position of this node with the mouse position and checks for Delete input.
    /// </summary>
    private void Update()
    {
        if(isSelected)
        {
            if (Input.GetMouseButtonUp(0))
            {
                drag = true;
            }
            if(Input.GetMouseButton(0) && GetComponentInParent<ModelViewController>().isMouseOverGrid() && drag)
            {
                GetComponent<RectTransform>().position = Input.mousePosition;
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Destroy(gameObject);
            }
        }
        else
        {
            drag = false;
        }
    }

    /// <summary>
    /// This node is no longer the selected node.
    /// </summary>
    public void Deselect()
    {
        isSelected = false;
       // drag = false;
    }

    /// <summary>
    /// Update the behavior of this node
    /// </summary>
    /// <param name="newType"> the new behavior </param>
    public void ChangeBehavior(Behavior newType)
    {
        if(type != newType)
        {
            type = newType;
            UpdateVisuals();
        }
    }

    /// <summary>
    /// Updates the visuals of this node
    /// </summary>
    public void UpdateVisuals()
    {
        nodeImage.GetComponent<Image>().sprite = sprites[(int)type-1];
        if(when == When.User && (int)type - 1 <= 4)
        {
            nodeImage.GetComponent<Image>().sprite = interactibleSprites[(int)type - 1];
        }
        if (definition)
        {
            Destroy(definition);
            definition = null;
        }
        numberText.text = "";
        number = 0;
        capacity = 0;
    }

    /// <summary>
    /// Updates the When component of this node.
    /// </summary>
    /// <param name="newActivation"> the new When </param>
    public void UpdateWhen(When newActivation)
    {
        when = newActivation;
        switch (when)
        {
            case When.Passive:
                nodeImage.GetComponent<Image>().sprite = sprites[(int)type - 1];
                activationText.text = "";
                break;
            case When.User:
                if((int)type-1 <= 4)
                {
                    nodeImage.GetComponent<Image>().sprite = interactibleSprites[(int)type - 1];
                }
                else
                {
                    nodeImage.GetComponent<Image>().sprite = sprites[(int)type - 1];
                }
                activationText.text = "";
                break;
            case When.Auto:
                nodeImage.GetComponent<Image>().sprite = sprites[(int)type - 1];
                activationText.text = "*";
                break;
            case When.Start:
                nodeImage.GetComponent<Image>().sprite = sprites[(int)type - 1];
                activationText.text = "s";
                break;
        }
    }

    /// <summary>
    /// Updates the value of the At component of this node.
    /// </summary>
    /// <param name="newNumber"> the new number </param>
    public void UpdateAt(int newNumber)
    {
        if(capacity == 0)
        {
            number = newNumber;
        }
        else if (newNumber <= capacity)
        {
            number = newNumber;
        }  
        else
        {
            number = capacity;
        }
        numberText.text = number.ToString();
    }

    /// <summary>
    /// Updates the max value of this node
    /// </summary>
    /// <param name="newCapacity"> the new max value, a max of 0 means there is no max </param>
    public void UpdateMax(int newCapacity)
    {
        if(newCapacity < 0)
        {
            newCapacity = 0;
            maxText.text = "";
        }
        capacity = newCapacity;
        maxText.text = "/" + capacity;
    }

    /// <summary>
    /// Updates the Act and How values of this node
    /// </summary>
    /// <param name="newAct"> new Act of this node </param>
    /// <param name="newHow"> new How of this node </param>
    public void UpdateActHow(Act newAct, How newHow)
    {
        act = newAct;
        how = newHow;

        pullModeText.text = "";

        if (act == Act.Push)
        {
            pullModeText.text = "p";
        }
        if (how == How.All)
        {
            pullModeText.text += "&";
        }
    }

    /// <summary>
    /// Updates the In Out component of this node.
    /// </summary>
    /// <param name="newIO"> new In Out of this node </param>
    public void UpdateIO(IO newIO)
    {
        inout = newIO;
        switch (inout)
        {
            case IO.Internal:
                inoutImage.enabled = false;
                break;
            case IO.In:
                inoutImage.enabled = true;
                inoutImage.GetComponent<Image>().sprite = inoutArrows[(int)inout - 1];
                break;
            case IO.Out:
                inoutImage.enabled = true;
                inoutImage.GetComponent<Image>().sprite = inoutArrows[(int)inout - 1];
                break;
            case IO.InOut:
                inoutImage.enabled = true;
                inoutImage.GetComponent<Image>().sprite = inoutArrows[(int)inout - 1];
                break;
        }
    }

    /// <summary>
    /// Update the add of this node
    /// </summary>
    /// <param name="newAdd"> new add value of this node </param>
    public void UpdateAdd(string newAdd)
    {
        add = newAdd;
    }

    /// <summary>
    /// Updates the OfType of this node (adds a definition)
    /// </summary>
    /// <param name="newOfType"> new OfType </param>
    public void UpdateOfType(string newOfType)
    {
        //TODO update definition text on tab selected?
        if(newOfType == "")
        {
            Destroy(definition);
            definition = null;
        }
        else if(definition)
        {
            ofType = newOfType;
            definition.GetComponentInChildren<Text>().text = newOfType;
            UpdateDefinitionReferences();
        }
        else
        {
            ofType = newOfType;
            definition = Instantiate(definitionPrefab, transform.position + new Vector3(0, 25, 0), definitionPrefab.transform.rotation, transform) as GameObject;
            definition.transform.SetAsFirstSibling();
            definition.GetComponentInChildren<Text>().text = newOfType;
            UpdateDefinitionReferences();
        }
    }
    public void UpdateDefinitionReferences()
    {
        definition.GetComponent<DefinitionScript>().ClearReferencePoints();
        GameObject[] tempArr = GameObject.FindGameObjectsWithTag("GridTab");
        Transform gridtab;
        Transform content = null;
        for(int i = 0; i < tempArr.Length; i++)
        {
            if(tempArr[i].GetComponentInChildren<Text>().text == definition.GetComponentInChildren<Text>().text)
            {
                gridtab = tempArr[i].transform.parent;
                content = gridtab.GetChild(0).GetChild(0).GetChild(0);
                break;
            }
        }
        tempArr = GameObject.FindGameObjectsWithTag("Node");
        for(int i = 0; i < tempArr.Length; i++)
        {
            if(content && tempArr[i].transform.parent == content)
            {
                if (tempArr[i].GetComponent<SelectNodeButton>().type == Behavior.Reference)
                {
                    definition.GetComponent<DefinitionScript>().AddReferencePoint(tempArr[i]);
                }
            }
        }
    }
}
