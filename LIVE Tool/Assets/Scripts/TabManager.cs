using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Script that manages the tabs and adds new tabs
/// </summary>
public class TabManager : MonoBehaviour {
    public Button yourButton;
    public GameObject graphTab;
    public Transform canvas;
    public int tabCount = 0;
    public GameObject tempObj;
    public float scale = 1.0f;
    public int nameCounter = 1;
    public ModelController model;
    /// <summary>
    /// start listening to the "tabRemoved" event
    /// </summary>
    void OnEnable()
    {
        EventManager.StartListening("tabRemoved", TabRemoved);
    }

    /// <summary>
    /// stop listening to the "tabRemoved" event
    /// </summary>
    void OnDisable()
    {
        EventManager.StopListening("tabRemoved", TabRemoved);
    }

    /// <summary>
    /// Adds the TaskOnClick to the button onclick
    /// </summary>
    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
        model = GameObject.FindGameObjectWithTag("Tool").GetComponent<ModelController>();
    }

    /// <summary>
    /// Checks for Ctrl + A to create a new tab
    /// Checks for Tab to cycle tabs
    /// </summary>
    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A))
        {
            TaskOnClick();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameObject[] tabs = GameObject.FindGameObjectsWithTag("GridTab");
            int index = 0;
            if(tabs.Length > 0)
            {
                if (canvas.GetChild(canvas.childCount - 1).GetChild(1).tag == "GridTab")
                {
                    index = canvas.GetChild(canvas.childCount - 1).GetChild(1).GetComponent<DynamicTab>().tabIndex;
                }
                if(index == tabs.Length-1)
                {
                    for (int i = 0; i < tabs.Length; i++)
                    {
                        if (tabs[i].GetComponent<DynamicTab>().tabIndex == 0)
                        {
                            tabs[i].GetComponent<DynamicTab>().OnClick();
                            tabs[i].GetComponent<DynamicTab>().gridTab.SetAsLastSibling();
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < tabs.Length; i++)
                    {
                        if (tabs[i].GetComponent<DynamicTab>().tabIndex == (index + 1))
                        {
                            tabs[i].GetComponent<DynamicTab>().OnClick();
                            tabs[i].GetComponent<DynamicTab>().gridTab.SetAsLastSibling();
                            break;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Function that is called on click, spawns a new tab and moves up the new tab accordingly
    /// </summary>
   void TaskOnClick()
    {
        if(tabCount < 10)
        {
            if(tabCount == 0)
            {
                AddTab("Global");
                model.AddProgram();
            }
            else
            {
                AddTab("Tab_" + nameCounter);
                model.AddDefinition("Tab_" + nameCounter);
                nameCounter++;
            }
        }
    }
    /// <summary>
    /// Adds a tab to the diagram
    /// </summary>
    /// <param name="name">name of the new tab</param>
    public void AddTab(string name)
    {
        EventManager.TriggerEvent("tabAdded");
        if (tabCount > 4)
        {
            EventManager.TriggerEvent("scaleDown");
        }
        tabCount++;
        tempObj = Instantiate(graphTab, new Vector3(250, 176, 0), canvas.rotation, canvas) as GameObject;
        tempObj.GetComponentInChildren<InputField>().text = name;
        tempObj.GetComponentInChildren<TabText>().oldName = name;

        for (int i = 0; i < tabCount - 1; i++)
        {
            if (i > 3)
            {
                tempObj.GetComponentInChildren<DynamicTab>().ScaleDown();
            }
            tempObj.GetComponentInChildren<DynamicTab>().MoveUp();
        }

        if (tabCount <= 5)
        {
            GetComponent<RectTransform>().localPosition += new Vector3(94, 0, 0);
        }
    }
    /// <summary>
    /// When this script receives a "tabRemoved" event this function scales the remaining tabs up
    /// </summary>
    public void TabRemoved()
    {
        tabCount--;
        EventManager.TriggerEvent("scaleUp");
        if(tabCount < 5)
        {
            GetComponent<RectTransform>().position -= new Vector3(94, 0, 0);
        }
    }

    /// <summary>
    /// returns the active tab
    /// </summary>
    /// <returns>Returns the active tab</returns>
    public GameObject GetLastTab()
    {
        return tempObj;
    }
}
