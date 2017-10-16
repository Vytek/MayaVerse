using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MoveHandle))]
public class MoveTool : BaseTool {
 
    private const float MARGIN = 0.6f;

    private Vector3 startPosition;
    private bool isMouseDown = false;
    private float axisInfo = 0.0f;
	
    public override void OnEnable() {
        base.OnEnable();
    }

    public override void OnSceneGUI() {
        MoveHandle _handle = target as MoveHandle;

        if (_handle != null) {
            // On MouseDown event, set some variables
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                isMouseDown = true;
                startPosition = _handle.transform.position;
            } else if (Event.current.type == EventType.MouseUp && Event.current.button == 0) {
                isMouseDown = false;
            }

            if (isMouseDown) {
                Vector3 _direction = _handle.transform.position - startPosition;
                Vector3 _localDirection = _handle.transform.InverseTransformDirection(_direction);

                if (_localDirection.x < -MARGIN || _localDirection.x > MARGIN) {
                    Handles.color = settings.moveTool.x;
                    axisInfo = _localDirection.x;
                }

                if (_localDirection.y < -MARGIN || _localDirection.y > MARGIN) {
                    Handles.color = settings.moveTool.y;
                    axisInfo = _localDirection.y;
                }

                if (_localDirection.z < -MARGIN || _localDirection.z > MARGIN) {
                    Handles.color = settings.moveTool.z;
                    axisInfo = _localDirection.z;
                }

                if (settings.moveTool.ruler) {
                    // Draw a ruler and a label with extra information
                    Handles.DrawDottedLine(startPosition, _handle.transform.position, 10);
                }
                
                if (settings.moveTool.label) {
                    Handles.Label(
                        _handle.transform.position,
                        axisInfo.ToString("F1") + " units",
                        SetLabelStyle()
                    );
                }
            }
        }
    }
}
