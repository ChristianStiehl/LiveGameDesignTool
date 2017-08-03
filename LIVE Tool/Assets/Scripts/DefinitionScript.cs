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
/// This script controls the locations of interface nodes on a definition
/// TODO: Rethink the placement of interface nodes/make them interactable
/// </summary>
public class DefinitionScript : MonoBehaviour {
    public GameObject referencePoint;
    public List<GameObject> references;
    public List<GameObject> referencePoints;
    private GameObject tempref;

    /// <summary>
    /// Adds a reference point to the definition
    /// </summary>
    /// <param name="newref">The reference point to be added</param>
    public void AddReferencePoint(GameObject newref)
    {
        references.Add(newref);
        tempref = Instantiate(referencePoint, transform.position, transform.rotation, transform) as GameObject;
        tempref.GetComponentInChildren<Text>().text = transform.parent.GetComponent<SelectNodeButton>().label.text +"." +newref.GetComponent<SelectNodeButton>().label.text;
        Vector3 direction = newref.transform.position - transform.position;
        direction.Normalize();
        direction *= 40;

        if(direction.x > 26)
        {
            direction.x = 26;
        }
        if(direction.x < -26)
        {
            direction.x = -26;
        }
        if(direction.y > 26)
        {
            direction.y = 26;
        }
        if(direction.y < -26)
        {
            direction.y = -26;
        }
        tempref.GetComponent<RectTransform>().position += direction;
        referencePoints.Add(tempref);
    }

    /// <summary>
    /// Removes a reference point from the definition
    /// </summary>
    /// <param name="removedRef">The reference point to be removed</param>
    public void RemoveReferencePoint(GameObject removedRef)
    {
        for(int i = 0; i < references.Count; i++)
        {
            if(references[i] == removedRef)
            {
                Destroy(referencePoints[i]);
                referencePoints.RemoveAt(i);
                references.Remove(removedRef);
            }
        }
    }

    /// <summary>
    /// Removes all reference points
    /// </summary>
    public void ClearReferencePoints()
    {
        for (int i = 0; i < references.Count; i++)
        {
            Destroy(referencePoints[i]);
        }
        references.Clear();
    }
}
