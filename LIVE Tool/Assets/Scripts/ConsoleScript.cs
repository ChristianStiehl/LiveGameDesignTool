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
/// <summary>
/// Enum for different types of messages
/// </summary>
public enum ErrorType
{
    error,
    warning,
    info,
    message
}

/// <summary>
/// WIP
/// Debugging console of the tool
/// </summary>
public class ConsoleScript : MonoBehaviour
{
    public Text consoleText;
    public Button consoleButton;
    public GameObject errorTab;

    private void Start()
    {
        consoleButton.onClick.AddListener(GoToError);
    }

    /// <summary>
    /// Displays the error message in the console
    /// </summary>
    /// <param name="type">Type of the error</param>
    /// <param name="text">The error message</param>
    /// <param name="tab">Source of the error (definition)</param>
    /// <param name="objects"></param>
    public void DisplayError(ErrorType type, string text, GameObject tab, List<GameObject> objects)
    {
        consoleText.text = "Console: " +type.ToString() +": " +text;
        errorTab = tab;
        switch (type)
        {
            case ErrorType.error:
                consoleText.color = Color.red;
                break;
            case ErrorType.warning:
                consoleText.color = Color.yellow;
                break;
            case ErrorType.info:
                consoleText.color = Color.blue;
                break;
            case ErrorType.message:
                consoleText.color = Color.black;
                break;
        }
    }

    public void GoToError()
    {
        if (errorTab)
        {
            errorTab.transform.SetAsLastSibling();
        }
    }
}
