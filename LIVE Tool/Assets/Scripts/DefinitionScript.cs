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
