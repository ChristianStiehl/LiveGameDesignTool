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
using UnityEngine.UI;

public class RunBehavior : MonoBehaviour {
    public Button yourButton;
    private GameObject[] tabs = new GameObject[0];
    public GameObject addTabButton;
    public EditBehavior eb;
    public DisableEditingScript fb, gb;
    public GameObject diagramNameBar;
    /// <summary>
    /// Subscribes the OnClick function to the button on click.
    /// </summary>
    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
        eb = GameObject.FindGameObjectWithTag("EditTab").GetComponent<EditBehavior>();
        fb = GameObject.FindGameObjectWithTag("FileTab").GetComponent<DisableEditingScript>();
        gb = GameObject.FindGameObjectWithTag("GraphTab").GetComponent<DisableEditingScript>();
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    void OnClick()
    {
        if(diagramNameBar.activeSelf == false)
        {
            tabs = GameObject.FindGameObjectsWithTag("GridTab");
            eb.SetTabArray(tabs);
            fb.SetTabArray(tabs);
            gb.SetTabArray(tabs);
            foreach (GameObject tab in tabs)
            {
                tab.SetActive(false);
                tab.transform.parent.gameObject.GetComponentInChildren<ModelViewController>().DisableEditing();
            }
            addTabButton.SetActive(false);
            diagramNameBar.SetActive(true);
        }
    }
}
