using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Visual Edge Object logic
/// </summary>
public class SelectEdgeButton : MonoBehaviour
{
    public List<RectTransform> edgeCorners;
    private List<RectTransform> edgeLines;
    public Button yourButton;
    public Behavior type;
    public RectTransform lineImage;
    public RectTransform cornerImage;
    private RectTransform tempCorner;
    public RectTransform tempEdge;
    public Sprite cornerSprite;
    public Transform content;
    public bool isSelected = false;
    private bool isCompleted = false;
    public MM.Model.Edge idedge;

    public Text label;
    public Text exp;
    public string source;
    public string target;

    public GameObject sourceObj = null;
    public GameObject targetObj = null;

    public RectTransform sourceCorner = null;
    public RectTransform targetCorner = null;

    public RectTransform edgeArrow;

    // Use this for initialization
    void Awake ()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
        edgeCorners = new List<RectTransform>();
        edgeLines = new List<RectTransform>();
        EventManager.StartListening("newNodeSelected", Deselect);
        EventManager.StartListening("gridSelected", Deselect);
        content = GetComponentInParent<ModelViewController>().view;
        isSelected = true;
        if(type == Behavior.Flow)
        {
            exp.text = "1";
        }
    }

    private void OnDestroy()
    {
        EventManager.StopListening("newNodeSelected", Deselect);
        EventManager.StopListening("gridSelected", Deselect);
        for(int i = 0; i < edgeCorners.Count; i++)
        {
            if (edgeCorners[i])
            {
                Destroy(edgeCorners[i].gameObject);
            }
        }
    }

    void OnClick()
    {
        if (isCompleted)
        {
            isSelected = true;
            GetComponentInParent<ModelViewController>().SelectEdge(this.gameObject);

        }
    }

    private void Update()
    {
        if(edgeCorners.Count > 0)
        {
            for (int i = 0; i < edgeLines.Count; i++)
            {
                if (i + 1 < edgeCorners.Count)
                {
                    Stretch(edgeLines[i], edgeCorners[i].position, edgeCorners[i + 1].position);
                    if(i+1 == edgeCorners.Count / 2) //checks for middle of the edge for label/expression text placement
                    {
                        Stretch(label.rectTransform, edgeCorners[i].position, edgeCorners[i + 1].position);
                        label.rectTransform.localScale = new Vector3(1, 1, 1);
                        label.rectTransform.position += new Vector3(0, -10, 0);
                        label.rectTransform.rotation = new Quaternion(0, 0, 0, 0);
                        Stretch(exp.rectTransform, edgeCorners[i].position, edgeCorners[i + 1].position);
                        exp.rectTransform.localScale = new Vector3(1, 1, 1);
                        exp.rectTransform.position += new Vector3(0, 10, 0);
                        exp.rectTransform.rotation = new Quaternion(0, 0, 0, 0);
                    }
                    //fix arrow position and rotation. offset by 25 from middle of nodes
                    edgeArrow.position = edgeCorners[i + 1].position;
                    Vector3 rot = edgeCorners[i + 1].position - edgeCorners[i].position;
                    rot.Normalize();
                    edgeArrow.transform.right = rot;
                    Vector3 scaler;
                    //smaller offset for interface nodes
                    //Potentially TODO: set custom offsets for all types of nodes.
                    if (targetObj != null && targetObj.tag == "ReferenceInterface")
                    {
                        scaler = new Vector3(10, 10, 10);
                    }
                    else
                    {
                        scaler = new Vector3(25, 25, 25);
                    }
                    scaler.Scale(rot);
                    edgeArrow.position -= scaler;
                }
                else
                {
                    Stretch(tempEdge, edgeCorners[i].position, Input.mousePosition);
                    edgeArrow.position = Input.mousePosition;
                    Vector3 rot = Input.mousePosition - edgeCorners[i].position;
                    rot.Normalize();
                    edgeArrow.transform.right = rot;
                    Vector3 scaler = new Vector3(5, 5, 5);
                    scaler.Scale(rot);
                    edgeArrow.position -= scaler;
                }
            }
        }
        if (isSelected)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Adds a new corner point to the edge
    /// TODO: Corner points should be moveable
    /// </summary>
    /// <param name="newPoint">Position of the new point</param>
    /// <param name="parent">Parent of the new point (important for target/source)</param>
    public void AddEdgePoint(Vector2 newPoint, GameObject parent)
    {
        if(parent == null)
        {
            parent = this.gameObject;
            tempCorner = Instantiate(cornerImage, newPoint, transform.rotation, parent.transform);
            tempCorner.GetComponent<Image>().sprite = cornerSprite;
            tempCorner.GetComponent<Image>().color = Color.white;
            edgeCorners.Add(tempCorner);
        }
        else
        {
            if(sourceObj == null)
            {
                sourceObj = parent;
                if (sourceObj.GetComponent<SelectNodeButton>())
                {
                    source = sourceObj.GetComponent<SelectNodeButton>().label.text;
                }
                else
                {
                    source = sourceObj.GetComponentInChildren<Text>().text;
                }
                //Add corner image
                tempCorner = Instantiate(cornerImage, newPoint, transform.rotation, parent.transform);
                tempCorner.GetComponent<Image>().sprite = cornerSprite;
                tempCorner.GetComponent<Image>().color = Color.white;
                tempCorner.SetAsFirstSibling();
                edgeCorners.Add(tempCorner);
                sourceCorner = tempCorner;
            }
            else
            {
                targetObj = parent;
                if (targetObj.GetComponent<SelectNodeButton>())
                {
                    target = targetObj.GetComponent<SelectNodeButton>().label.text;
                }
                else
                {
                    target = targetObj.GetComponentInChildren<Text>().text;
                }
                tempCorner = Instantiate(cornerImage, newPoint, transform.rotation, parent.transform);
                tempCorner.GetComponent<Image>().sprite = cornerSprite;
                tempCorner.GetComponent<Image>().color = Color.white;
                tempCorner.SetAsFirstSibling();
                edgeCorners.Add(tempCorner);
                targetCorner = tempCorner;
            }
        }

        tempEdge = Instantiate(lineImage, transform.position, transform.rotation, gameObject.GetComponent<RectTransform>());
        tempEdge.gameObject.SetActive(true);
        if(type == Behavior.State) {
            tempEdge.GetComponent<RawImage>().color = Color.black;
        }
        else
        {
            tempEdge.GetComponent<Image>().color = Color.black;
        }

        tempEdge.SetAsFirstSibling();
        edgeLines.Add(tempEdge);
    }

    /// <summary>
    /// Stretches the edge image between two points (Important: Edges use RawImages for UV management!)
    /// </summary>
    /// <param name="_sprite">The recttransform that needs to be stretched</param>
    /// <param name="_initialPosition">first point</param>
    /// <param name="_finalPosition">second point</param>
    public void Stretch(RectTransform _sprite, Vector2 _initialPosition, Vector2 _finalPosition)
    {
        Vector2 centerPos = (_initialPosition + _finalPosition) / 2f;
        _sprite.transform.position = centerPos;
        Vector2 direction = _finalPosition - _initialPosition;
        direction.Normalize();
        _sprite.transform.right = direction;
        Vector3 scale = new Vector3(1, 0.2f, 1);
        float scaleFactor = (1-content.localScale.x)*2;
        scale.x = (Vector2.Distance(_initialPosition, _finalPosition) * (1+scaleFactor)) * (0.1f);
        _sprite.transform.localScale = scale;
        if(this.type == Behavior.State && _sprite.GetComponent<RawImage>())
        {
            _sprite.GetComponent<RawImage>().uvRect = new Rect(0, 0, Vector2.Distance(_initialPosition, _finalPosition) / 20, 1);
        }
    }

    /// <summary>
    /// Completes the edge (stops stretching to mouse)
    /// </summary>
    public void StopCurrentEdge()
    {
        edgeLines.Remove(tempEdge);
        Destroy(tempEdge.gameObject);
        isCompleted = true;
    }

    /// <summary>
    /// deselects
    /// </summary>
    public void Deselect()
    {
        isSelected = false;
    }

    /// <summary>
    /// Updates the source of this edge
    /// </summary>
    /// <param name="newSourceName">Name of the new source</param>
    /// <returns>Returns true if a node with the given name was found and the source is updated</returns>
    public bool UpdateSource(string newSourceName)
    {
        GameObject[] tempArr = GameObject.FindGameObjectsWithTag("Node");
        for (int i = 0; i < tempArr.Length; i++)
        {
            if (tempArr[i].transform.parent == this.transform.parent && tempArr[i].GetComponent<SelectNodeButton>().label.text == newSourceName)
            {
                if (tempArr[i] != targetObj)
                {
                    sourceObj = tempArr[i];
                    source = newSourceName;
                    if (sourceCorner)
                    {
                        sourceCorner.SetParent(sourceObj.transform);
                        sourceCorner.SetAsFirstSibling();
                        sourceCorner.position = sourceObj.transform.position;
                    }
                    else
                    {
                        sourceCorner = edgeCorners[0];
                        sourceCorner.SetParent(sourceObj.transform);
                        sourceCorner.SetAsFirstSibling();
                        sourceCorner.position = sourceObj.transform.position;
                    }
                    return true;
                }
            }
        }
        GameObject[] interfaces = GameObject.FindGameObjectsWithTag("ReferenceInterface");
        for (int i = 0; i < interfaces.Length; i++)
        {
            if (interfaces[i].transform.parent.parent.parent == this.transform.parent && interfaces[i].GetComponentInChildren<Text>().text == newSourceName)
            {
                if (interfaces[i] != targetObj)
                {
                    sourceObj = interfaces[i];
                    source = newSourceName;
                    if (sourceCorner)
                    {
                        sourceCorner.SetParent(sourceObj.transform);
                        sourceCorner.SetAsFirstSibling();
                        sourceCorner.position = sourceObj.transform.position;
                    }
                    else
                    {
                        sourceCorner = edgeCorners[0];
                        sourceCorner.SetParent(sourceObj.transform);
                        sourceCorner.SetAsFirstSibling();
                        sourceCorner.position = sourceObj.transform.position;
                    }
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Updates the target node of this edge
    /// </summary>
    /// <param name="newTargetName">Name of the new target</param>
    /// <returns>Returns true if a node with the given name was found and the target is updated</returns>
    public bool UpdateTarget(string newTargetName)
    {
        GameObject[] tempArr = GameObject.FindGameObjectsWithTag("Node");
        for(int i = 0; i<tempArr.Length; i++)
        {
            if(tempArr[i].transform.parent == this.transform.parent && tempArr[i].GetComponent<SelectNodeButton>().label.text == newTargetName)
            {
                if(tempArr[i] != sourceObj)
                {
                    targetObj = tempArr[i];
                    target = newTargetName;
                    if (targetCorner)
                    {
                        targetCorner.SetParent(targetObj.transform);
                        targetCorner.SetAsFirstSibling();
                        targetCorner.position = targetObj.transform.position;
                    }
                    else
                    {
                        targetCorner = edgeCorners[edgeCorners.Count-1];
                        targetCorner.SetParent(targetObj.transform);
                        targetCorner.SetAsFirstSibling();
                        targetCorner.position = targetObj.transform.position;
                    }
                    return true;
                }
            }
        }
        GameObject[] interfaces = GameObject.FindGameObjectsWithTag("ReferenceInterface");
        for (int i = 0; i < interfaces.Length; i++)
        {
            if (interfaces[i].transform.parent == this.transform.parent && interfaces[i].GetComponentInChildren<Text>().text == newTargetName)
            {
                if (interfaces[i] != sourceObj)
                {
                    targetObj = interfaces[i];
                    target = newTargetName;
                    if (targetCorner)
                    {
                        targetCorner.SetParent(targetObj.transform);
                        targetCorner.SetAsFirstSibling();
                        targetCorner.position = targetObj.transform.position;
                    }
                    else
                    {
                        targetCorner = edgeCorners[edgeCorners.Count-1];
                        targetCorner.SetParent(targetObj.transform);
                        targetCorner.SetAsFirstSibling();
                        targetCorner.position = targetObj.transform.position;
                    }
                    return true;
                }
            }
        }
        return false;
    }
}
