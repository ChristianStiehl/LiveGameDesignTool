using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script enables the user to use the scroll wheel to zoom out on diagrams
/// </summary>
public class ZoomContentScript : MonoBehaviour {
    private float currentScale = 1;
    private float maxScale = 1;
    private float minScale = 0.5f;
    public RectTransform content;
    public Transform canvas;
    public Transform grid;
    private Vector3 anchor;

    private void Awake()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas").transform;
        anchor = content.position;
    }

    void FixedUpdate()
    {
        if (canvas.GetChild(canvas.childCount - 1) == grid)
        {
            if(Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                Zoom(Input.GetAxis("Mouse ScrollWheel"));
            }
        }
    }

    void Zoom(float increment)
    {
        currentScale += increment;
        if (currentScale >= maxScale)
        {
            currentScale = maxScale;
        }
        else if (currentScale <= minScale)
        {
            currentScale = minScale;
        }
        content.position = anchor;
        content.localScale = new Vector3(currentScale, currentScale, currentScale);
    }
}
