using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BaseTool : Editor, IBaseTool {

    public SettingsSerializer settings = new SettingsSerializer();

    public virtual void OnEnable() {
        settings = SettingsSerializer.Load();
    }

    public virtual void OnSceneGUI() {
        // You can keep this empty, this is a building block for dirived Tools
    }

    public GUIStyle SetLabelStyle() {
        GUIStyle _style         = new GUIStyle();
        _style.normal.textColor = Color.white;
        _style.fontStyle        = FontStyle.Bold;
        _style.fontSize         = 14;

        return _style;
    }
}
