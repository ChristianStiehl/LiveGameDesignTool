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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
/// <summary>
/// This script moves and scales the tabs when needed
/// </summary>
public class DynamicTab : MonoBehaviour
{
    private UnityAction dynamicTabListener;
    //the index of this tab
    public int tabIndex;
    //array to store all tabs in
    public GameObject[] tabs;
    //scale variable to scale the width
    public float scale = 1.0f;
    public Button yourButton;
    public RectTransform gridTab;
    /// <summary>
    /// on Awake this tab subscribes to events it needs to properly scale and move.
    /// </summary>
    void Awake()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
        dynamicTabListener = new UnityAction(MoveDown);
        EventManager.StartListening("tabRemoved", dynamicTabListener);
        EventManager.StartListening("moveDown", MoveDown);
        EventManager.StartListening("scaleDown", ScaleDown);
        EventManager.StartListening("scaleUp", ScaleUp);
    }
    /// <summary>
    /// on Destroy this tab unsubscribes from events it needed to properly scale and move.
    /// </summary>
    void OnDestroy()
    {
        if (EventManager.eventManager != null)
        {
            EventManager.StopListening("tabRemoved", dynamicTabListener);
            EventManager.StopListening("moveDown", MoveDown);
            EventManager.StopListening("scaleDown", ScaleUp);
            EventManager.StopListening("scaleUp", ScaleUp);
        }
    }
    /// <summary>
    /// Moves the tab up (scales with scale variable)
    /// </summary>
    public void MoveUp()
    {
        tabIndex++;
        this.transform.position += new Vector3(94*(scale), 0, 0);
    }
    /// <summary>
    /// Moves the tab down if it's not the leftmost tab (scales with scale variable)
    /// </summary>
    public void MoveDown()
    {
        if(tabIndex != 0)
        {
            tabs = GameObject.FindGameObjectsWithTag("GridTab");
            for(int i = 0; i < tabs.Length; i++)
            {
                if(tabs[i].GetComponent<DynamicTab>().tabIndex == tabIndex-1)
                {
                    return;
                }
            }
            tabIndex--;
            this.transform.position -= new Vector3(94*(scale), 0, 0);
            EventManager.TriggerEvent("moveDown");
        }
    }
    /// <summary>
    /// Decreases the scale and corrects position according to new width.
    /// </summary>
    public void ScaleDown()
    {
        if(scale > 0.5f)
        {
            if(tabIndex == 0)
            {
                this.transform.position -= new Vector3(((8 * scale)), 0, 0);
            }
            else
            {
                this.transform.position -= new Vector3(((8 * scale) + 12 * tabIndex), 0, 0);
            }
            scale *= 0.8f;
            if(scale < 0.5f)
            {
                scale = 0.5f;
            }
            gameObject.GetComponent<RectTransform>().localScale = new Vector3(scale, 1, 1);
        }
    }
    /// <summary>
    /// Increases scale and corrects position according to new width.
    /// </summary>
    public void ScaleUp()
    {
        if(scale < 1)
        {
            scale *= 1.2f;
            if (scale > 1)
            {
                scale = 1;
            }
            gameObject.GetComponent<RectTransform>().localScale = new Vector3(scale, 1, 1);
            if (tabIndex == 0)
            {
                this.transform.position += new Vector3(((8 * scale)), 0, 0);
            }
            else
            {
                this.transform.position += new Vector3(((8 * scale) + 12 * tabIndex), 0, 0);
            }
        }
    }
    /// <summary>
    /// When clicked the tab notifies other objects that a tab has been selected
    /// </summary>
    public void OnClick()
    {
        EventManager.TriggerEvent("tabSelected");
    }
}
