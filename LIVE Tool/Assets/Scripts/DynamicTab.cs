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
