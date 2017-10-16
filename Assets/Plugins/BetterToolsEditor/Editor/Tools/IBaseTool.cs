using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseTool {
    void OnEnable();
    void OnSceneGUI();
    GUIStyle SetLabelStyle();
}
