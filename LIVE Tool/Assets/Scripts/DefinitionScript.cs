using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefinitionScript : MonoBehaviour {
    public GameObject referencePoint;
    public List<GameObject> references;
    public List<GameObject> referencePoints;
    private GameObject tempref;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

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

    public void ClearReferencePoints()
    {
        for (int i = 0; i < references.Count; i++)
        {
            Destroy(referencePoints[i]);
        }
        references.Clear();
    }
}
