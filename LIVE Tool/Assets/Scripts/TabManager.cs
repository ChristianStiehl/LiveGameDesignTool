/******************************************************************************
 * Copyright (c) 2013-2017, Amsterdam University of Applied Sciences (HvA), 
 * Firebrush Studios and Centrum Wiskunde & Informatica (CWI)
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice,
 *    this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 * 3. Neither the name of the copyright holder nor the names of its contributors
 *    may be used to endorse or promote products derived from this software
 *    without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 * PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
 * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
 * OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * Contributors:
 *   * Christian Stiehl - christian.stiehl@hva.nl - HvA / Firebrush Studios
 *   * Riemer van Rozen - rozen@cwi.nl - HvA / CWI
 ******************************************************************************/

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
