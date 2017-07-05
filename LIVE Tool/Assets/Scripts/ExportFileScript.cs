using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;

public class ExportFileScript : MonoBehaviour {
    public UnityEngine.UI.Button yourButton;
    public string path = null;
    public ModelController mc;

    /// <summary>
    /// Subscribes the TaskOnClick function to the buttons onClick event
    /// </summary>
    void Start()
    {
        UnityEngine.UI.Button btn = yourButton.GetComponent<UnityEngine.UI.Button>();
        btn.onClick.AddListener(TaskOnClick);
    }
    /// <summary>
    /// When clicked opens the file browser
    /// </summary>
    void TaskOnClick()
    {
        SaveFileDialog save = new SaveFileDialog();
        save.ShowDialog();
        path = save.FileName.ToString();
        if(path != null)
        {
            mc.ExportToFile(path);
        }
    }
}
