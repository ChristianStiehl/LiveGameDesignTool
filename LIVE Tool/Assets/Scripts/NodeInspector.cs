using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// The node inspector
/// </summary>
public class NodeInspector : MonoBehaviour
{
    public Transform canvas;
    public GameObject content;
    public ModelController mc;

    //The selected node GameObject
    public GameObject selectedNode;

    //GameObjects of inspector elements that need to be disabled (only Pool Behavior related)
    public GameObject atField;
    public GameObject maxField;
    public GameObject addField;
    public GameObject ofTypeField;

    //InputField inspector elements
    public InputField label;
    public InputField at;
    public InputField max;
    public InputField add;
    public InputField positionx;
    public InputField positiony;

    // Dropdown inspector elements
    public Dropdown whendd;
    public Dropdown actdd;
    public Dropdown howdd;
    public Dropdown behaviordd;
    public Dropdown inoutdd;
    public Dropdown ofTypedd;

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
        label.onEndEdit.AddListener(delegate { UpdateText(); });
        behaviordd.onValueChanged.AddListener(delegate { UpdateType(); });
        at.onEndEdit.AddListener(delegate { UpdateNumber(); });
        max.onEndEdit.AddListener(delegate { UpdateCapacity(); });
        whendd.onValueChanged.AddListener(delegate { UpdateActivation(); });
        actdd.onValueChanged.AddListener(delegate { UpdatePullMode(); });
        howdd.onValueChanged.AddListener(delegate { UpdatePullMode(); });
        inoutdd.onValueChanged.AddListener(delegate { UpdateIO(); });
        add.onEndEdit.AddListener(delegate { UpdateAdd(); });
        ofTypedd.onValueChanged.AddListener(delegate { UpdateOfType(); });
        positionx.onEndEdit.AddListener(delegate { UpdatePosition(); });
        positiony.onEndEdit.AddListener(delegate { UpdatePosition(); });
    }
    /// <summary>
    /// Detects if the selected Node is null (destroyed or deselected) and disables the content if it is. Also updates the position label in real time.
    /// </summary>
    private void Update()
    {
        if (selectedNode == null)
        {
            content.SetActive(false);
        }
        else
        {
            activeTabName = canvas.GetChild(canvas.childCount - 1).GetChild(1).GetComponentInChildren<Text>().text;
            if ((positionx.text != (Mathf.RoundToInt(selectedNode.transform.position.x)).ToString() || positiony.text != (Mathf.RoundToInt(selectedNode.transform.position.y)).ToString()) && Input.GetMouseButton(0))
            {
                int tempx, tempy;
                int.TryParse(positionx.text, out tempx);
                int.TryParse(positiony.text, out tempy);
                Vector2 old = new Vector2(tempx, tempy);
                mc.UpdateNodePosition(selectedNode.transform.position, activeTabName, selectedNode.GetComponent<SelectNodeButton>().idNode);
                positionx.text = ((int)selectedNode.transform.position.x).ToString();
                positiony.text = ((int)selectedNode.transform.position.y).ToString();
            }

        }
    }
    /// <summary> 
    /// Selects given object in the node inspector 
    /// </summary>
    /// <param name="selectedObject"> the object that needs to be inspected </param>
    public void Select(GameObject selectedObject)
    {
        content.SetActive(true);
        SelectNodeButton snb = selectedObject.GetComponent<SelectNodeButton>();
        switch (snb.type)
        {
            case Behavior.Pool:
                UpdateContent(selectedObject, false, true, true, true, 0, (int)snb.how, (int)snb.act, (int)snb.when);
                break;
            case Behavior.Gate:
                UpdateContent(selectedObject, false, true, false, true, 0, (int)snb.how, 0, (int)snb.when);
                break;
            case Behavior.Source:
                UpdateContent(selectedObject, false, true, false, true, 0, 0, 1, (int)snb.when);
                break;
            case Behavior.Drain:
                UpdateContent(selectedObject, false, true, false, true, 0, (int)snb.how, 0, (int)snb.when);
                break;
            case Behavior.Converter:
                UpdateContent(selectedObject, false, false, false, true, 0, 1, 0, (int)snb.when);
                break;
            case Behavior.Reference:
                UpdateContent(selectedObject, true, false, false, false, (int)snb.inout, 0, 0, 0);
                break;
            case Behavior.Definition:
                //currently definitions dont have any stuff to change in the inspector, so disable it.
                content.SetActive(false);
                break;
        }
    }
    /// <summary>
    /// When the behavior of a Node changes while selected the inspector needs to update the content.
    /// </summary>
    /// <param name="selectedObject"> The selected object. </param>
    public void ChangeBehavior(GameObject selectedObject)
    {
        SelectNodeButton snb = selectedObject.GetComponent<SelectNodeButton>();
        switch (snb.type)
        {
            case Behavior.Pool:
                UpdateContent(selectedObject, false, true, true, true, 0, 0, 0, 0);
                break;
            case Behavior.Gate:
                UpdateContent(selectedObject, false, true, false, true, 0, 0, 0, 0);
                break;
            case Behavior.Source:
                UpdateContent(selectedObject, false, true, false, true, 0, 0, 1, 0);
                break;
            case Behavior.Drain:
                UpdateContent(selectedObject, false, true, false, true, 0, 0, 0, 0);
                break;
            case Behavior.Converter:
                UpdateContent(selectedObject, false, false, false, true, 0, 1, 0, 0);
                break;
            case Behavior.Reference:
                UpdateContent(selectedObject, true, false, false, false, 0, 0, 0, 0);
                break;
        }

    }
    /// <summary>
    /// Clears selection of the Node Inspector.
    /// </summary>
    public void Deselect()
    {
        selectedNode = null;
        content.SetActive(false);
        label.text = "";
    }
    /// <summary>
    /// Updates the options of the TypeOf drop down. These options are the names of all other tabs in the editor.
    /// </summary>
    /// <param name="selectedObj"> The current selected object used to set the default value of the drop down to whatever the selected object already has </param>
    public void UpdateDefinitions(GameObject selectedObj)
    {
        GameObject[] temparr = GameObject.FindGameObjectsWithTag("GridTab");
        ofTypedd.options.Clear();
        ofTypedd.options.Add(new Dropdown.OptionData() { text = "" });
        for (int i = 0; i < temparr.Length; i++)
        {
           if (canvas.GetChild(canvas.childCount - 1).GetChild(1) != temparr[i].GetComponent<RectTransform>())
           {
                if(temparr[i].GetComponentInChildren<Text>().text != "Global")
                {
                    ofTypedd.options.Add(new Dropdown.OptionData() { text = temparr[i].GetComponentInChildren<Text>().text });
                }
           }
        }
        if (selectedObj)
        {
            GameObject def = selectedObj.GetComponent<SelectNodeButton>().definition;
            if (def == null)
            {
                ofTypedd.value = 0;
            }
            else
            {
                ofTypedd.RefreshShownValue();
                for(int j = 0; j < ofTypedd.options.Count; j++)
                {
                    if (ofTypedd.options[j].text == selectedObj.GetComponent<SelectNodeButton>().ofType)
                    {
                        ofTypedd.value = j;
                        break;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Updates the Name of the currently selected node.
    /// </summary>
    public void UpdateText()
    {
        if (selectedNode)
        {
            selectedNode.GetComponent<SelectNodeButton>().label.text = label.text;
            if (label.text.Contains("   ") || label.text.Contains(" "))
            {
                Debug.Log("Error: Node name cannot contain whitespace.");
            }
            mc.UpdateNodeName(label.text, selectedNode.GetComponent<SelectNodeButton>().idNode, activeTabName);
        }
    }
    /// <summary>
    /// Updates the Behavior component of the currently selected node.
    /// </summary>
    public void UpdateType()
    {
        if (selectedNode)
        {
            selectedNode.GetComponent<SelectNodeButton>().ChangeBehavior((Behavior)behaviordd.value + 1);
            mc.UpdateNodeBehavior(selectedNode.GetComponent<SelectNodeButton>().type, selectedNode.GetComponent<SelectNodeButton>().idNode, activeTabName);
            ChangeBehavior(selectedNode);
        }
    }
    /// <summary>
    /// Updates the At component of the currently selected node.
    /// </summary>
    public void UpdateNumber()
    {
        if (selectedNode && at.text != "0")
        {
            selectedNode.GetComponent<SelectNodeButton>().UpdateAt(int.Parse(at.text));
            int newat;
            int.TryParse(at.text, out newat);
            mc.UpdateNodeAt(newat, selectedNode.GetComponent<SelectNodeButton>().idNode, activeTabName);
        }
    }
    /// <summary>
    /// Updates the Max component of the currently selected node.
    /// </summary>
    public void UpdateCapacity()
    {
        if (selectedNode)
        {
            selectedNode.GetComponent<SelectNodeButton>().UpdateMax(int.Parse(max.text));
            if (max.text.StartsWith("-"))
            {
                max.text = "0";
            }
            mc.UpdateNodeMax(int.Parse(max.text), activeTabName, selectedNode.GetComponent<SelectNodeButton>().idNode);
        }
    }
    /// <summary>
    /// Updates the Act and How components of the currently selected node.
    /// </summary>
    public void UpdatePullMode()
    {
        if (selectedNode)
        {
            selectedNode.GetComponent<SelectNodeButton>().UpdateActHow((Act)actdd.value, (How)howdd.value);
            mc.UpdateNodeAct((Act)actdd.value, selectedNode.GetComponent<SelectNodeButton>().idNode, activeTabName);
            mc.UpdateNodeHow((How)howdd.value, selectedNode.GetComponent<SelectNodeButton>().idNode, activeTabName);
        }
    }
    /// <summary>
    /// Updates the Activation component of the currently selected node.
    /// </summary>
    public void UpdateActivation()
    {
        if (selectedNode)
        {
            selectedNode.GetComponent<SelectNodeButton>().UpdateWhen((When)whendd.value);
            mc.UpdateNodeWhen((When)whendd.value, selectedNode.GetComponent<SelectNodeButton>().idNode, activeTabName);
        }
    }
    /// <summary>
    /// Updates the In Out component of the currently selected node.
    /// </summary>
    public void UpdateIO()
    {
        if (selectedNode)
        {
            selectedNode.GetComponent<SelectNodeButton>().UpdateIO((IO)inoutdd.value);
            mc.UpdateNodeIO((IO)inoutdd.value, selectedNode.GetComponent<SelectNodeButton>().idNode, activeTabName);
        }
    }
    /// <summary>
    /// Update the values and states of the elements of the inspector. For example if a pool node is selected, the In Out dropdown should not be interactable.
    /// </summary>
    /// <param name="selectedObject"> The selected Game Object, needed to get some values like the At or Name of a node </param>
    /// <param name="iointer"> boolean, should the In Out dropdown be interactable? </param>
    /// <param name="howinter"> boolean, should the How dropdown be interactable? </param>
    /// <param name="actinter"> boolean, should the Act dropdown be interactable? </param>
    /// <param name="wheninter"> boolean, should the When dropdown be interactable? </param>
    /// <param name="ioval"> int, what should the value of the In Out dropdown be. </param>
    /// <param name="howval"> int, what should the value of the How dropdown be. </param>
    /// <param name="actval"> int, what should the value of the Act dropdown be. </param>
    /// <param name="whenval"> int, what should the value of the When dropdown be. </param>
    private void UpdateContent(GameObject selectedObject, bool iointer, bool howinter, bool actinter, bool wheninter, int ioval, int howval, int actval, int whenval)
    {
        SelectNodeButton snb = selectedObject.GetComponent<SelectNodeButton>();
        if (snb.type == Behavior.Pool)
        {
            atField.SetActive(true);
            maxField.SetActive(true);
            addField.SetActive(true);
            ofTypeField.SetActive(true);
            at.text = snb.number.ToString();
            max.text = snb.capacity.ToString();
            add.text = snb.add;
            UpdateNumber();
            UpdateCapacity();
            UpdateDefinitions(selectedObject);
        }
        else
        {
            add.text = "";
            atField.SetActive(false);
            maxField.SetActive(false);
            addField.SetActive(false);
            ofTypeField.SetActive(false);
        }

        inoutdd.value = ioval;
        inoutdd.interactable = iointer;
        howdd.interactable = howinter;
        actdd.interactable = actinter;
        whendd.interactable = wheninter;
        whendd.value = whenval;
        actdd.value = actval;
        howdd.value = howval;
        behaviordd.value = (int)snb.type - 1;
        label.text = snb.label.text;
        positionx.text = selectedObject.transform.position.x.ToString();
        positiony.text = selectedObject.transform.position.y.ToString();

        selectedNode = selectedObject;
        UpdateText();
        UpdateActivation();
        UpdateIO();
        UpdatePullMode();
        UpdateAdd();
    }
    /// <summary>
    /// Updates the Add component of the currently selected node.
    /// </summary>
    public void UpdateAdd()
    {
        if (selectedNode)
        {
            if(add.text != "")
            {
                selectedNode.GetComponent<SelectNodeButton>().UpdateAdd(add.text);
                mc.UpdateNodeAdd(add.text, selectedNode.GetComponent<SelectNodeButton>().idNode, activeTabName);
            }
        }
    }
    /// <summary>
    /// Updates the OfType component of the currently selected node.
    /// </summary>
    public void UpdateOfType()
    {
        if (selectedNode)
        {
            if(ofTypedd.options[ofTypedd.value].text != "")
            {
                selectedNode.GetComponent<SelectNodeButton>().UpdateOfType(ofTypedd.options[ofTypedd.value].text);
                mc.UpdateNodeDefinition(ofTypedd.options[ofTypedd.value].text, selectedNode.GetComponent<SelectNodeButton>().idNode, activeTabName);
            }
            else
            {
                selectedNode.GetComponent<SelectNodeButton>().UpdateOfType(ofTypedd.options[ofTypedd.value].text);
                mc.UpdateNodeDefinition(null, selectedNode.GetComponent<SelectNodeButton>().idNode, activeTabName);
            }
        }
    }

    public void UpdatePosition()
    {
        if (selectedNode)
        {
            int tempx, tempy;
            int.TryParse(positionx.text, out tempx);
            int.TryParse(positiony.text, out tempy);
            Vector2 tempPos = new Vector2(tempx, tempy);
            selectedNode.transform.position = tempPos;
            mc.UpdateNodePosition(selectedNode.transform.position, activeTabName, selectedNode.GetComponent<SelectNodeButton>().idNode);
        }
    }
}

