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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script disables the input field of the tab unless it is the active tab.
/// </summary>
public class TabText : MonoBehaviour {
    public GameObject parent;
    private Transform canvas;
    public InputField ipf;
    public string oldName;
    public string newName;
    public ModelController model;

    void Start()
    {
        canvas = parent.transform.parent;
        model = GameObject.FindGameObjectWithTag("Tool").GetComponent<ModelController>();
        ipf.onEndEdit.AddListener(delegate { UpdateDefinition(); });
    }

    /// <summary>
    /// Updates the definition name in the model
    /// </summary>
    public void UpdateDefinition()
    {
        newName = this.GetComponent<Text>().text.ToString();
        if(newName.Contains("   ") || newName.Contains(" "))
        {
            Debug.Log("Error: Definition name cannot contain whitespace.");
        }
        model.UpdateDefinition(oldName, newName);
        oldName = newName;
    }

    private void FixedUpdate()
    {
        if (parent.transform.GetSiblingIndex() == canvas.transform.childCount - 1 && this.GetComponent<Text>().text != "Global")
        {
            this.GetComponent<InputField>().enabled = true;
        }
        else
        {
            this.GetComponent<InputField>().enabled = false;
        }
    }
}
