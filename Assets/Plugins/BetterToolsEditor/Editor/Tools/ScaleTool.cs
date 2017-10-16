using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScaleHandle))]
public class ScaleTool : BaseTool {

    private Vector3 startScale;
    private bool isMouseDown = false;
    private float axisInfo = 0.0f;

    public override void OnEnable() {
        base.OnEnable();
    }

    public override void OnSceneGUI() {
        ScaleHandle _handle = target as ScaleHandle;

        if (_handle != null) {
            // On MouseDown event, set some variables
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                isMouseDown = true;
                startScale = _handle.transform.localScale;
            } else if (Event.current.type == EventType.MouseUp && Event.current.button == 0) {
                isMouseDown = false;
            }

            if (isMouseDown) {
                if (settings.scaleTool.label) {
                    // Display a label with scaling related information
                    Handles.Label(
                        _handle.transform.position,
                        "Scale " + _handle.transform.localScale.ToString("F1"),
                        SetLabelStyle()
                    );
                }
            }
        }
    }
}
