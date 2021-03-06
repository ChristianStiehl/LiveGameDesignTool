﻿/******************************************************************************
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
