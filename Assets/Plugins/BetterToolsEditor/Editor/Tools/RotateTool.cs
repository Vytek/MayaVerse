using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RotateHandle))]
public class RotateTool : BaseTool {

    private Vector3 startWorldDirection, direction, rootForward, rootRight, rootUp;
    private bool isMouseDown = false;
    private float axisInfo = 0.0f;

    public override void OnEnable() {
        base.OnEnable();
    }

    public override void OnSceneGUI() {
        RotateHandle _handle = target as RotateHandle;

        if (_handle != null) {
            // On MouseDown event, set some variables
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                isMouseDown = true;

                // Get starting values of the axis
                startWorldDirection = _handle.transform.up;
                rootForward = _handle.transform.forward;
                rootRight = _handle.transform.right;
                rootUp = _handle.transform.up;

            } else if (Event.current.type == EventType.MouseUp && Event.current.button == 0) {
                isMouseDown = false;
            }

            if (isMouseDown) {
                Vector3 _worldDirection = _handle.transform.up;

                // Get our rotation angle
                float _angle = Mathf.Acos(Vector3.Dot(startWorldDirection, _worldDirection)) * Mathf.Rad2Deg;

                // Get Cross product between 2 Vectors
                Vector3 _cross = Vector3.Cross(startWorldDirection, _worldDirection);

                // Get Dot product between 2 Vectors
                if (Vector3.Dot(_handle.transform.forward, _cross) < 0) {
                    _angle = -_angle;
                }

                Vector3 _newForward = _handle.transform.forward;
                Vector3 _newRight = _handle.transform.right;
                Vector3 _newUp = _handle.transform.up;

                bool _y, _x, _z;
                _y = rootUp == _newUp;
                _x = rootRight == _newRight;
                _z = rootForward == _newForward;

                //rotating over x
                if (_x && !(_y && _z)) {
                    Handles.color = settings.rotateTool.x;
                    direction = new Vector3(0, 1, 1);
                    axisInfo = CalculateDegrees(GetAngle(rootUp, _newUp, rootRight));
                }
                //rotating over y
                if (_y && !(_x && _z)) {
                    Handles.color = settings.rotateTool.y;
                    direction = Vector3.up;
                    axisInfo = CalculateDegrees(GetAngle(rootRight, _newRight, rootUp));
                }
                //rotating over z
                if (_z && !(_x && _y)) {
                    Handles.color = settings.rotateTool.z;
                    direction = Vector3.forward;
                    axisInfo = CalculateDegrees(GetAngle(rootUp, _newUp, rootForward));
                }

                if (settings.rotateTool.ruler) {
                    Handles.DrawSolidArc(
                        _handle.transform.position,
                        direction,
                        -Vector3.right,
                        axisInfo,
                        // make sure our disc stays the same size as our tool handle
                        HandleUtility.GetHandleSize(_handle.transform.position)
                    );
                }
                
                if (settings.rotateTool.label) {
                    // Show a label with useful rotation related information
                    Handles.Label(
                        _handle.transform.position,
                        axisInfo.ToString("F1") + " degrees",
                        SetLabelStyle()
                    );
                }
            }
        }
    }

    private float GetAngle(Vector3 a, Vector3 b, Vector3? customCross = null) {
        a.Normalize();
        b.Normalize();

        Vector3 _cross = customCross == null ? Vector3.Cross(a, b) : customCross.Value;

        Vector3 _aP = Quaternion.AngleAxis(90, _cross) * a;
        return (Mathf.Atan2(Vector3.Dot(_aP, b), Vector3.Dot(a, b)) / Mathf.PI) * 180;
    }

    // Dot product goes from -180 to 0 to 180
    // -180 now equals 360 degrees rotation
    private float CalculateDegrees(float value) {
        if (value < 0) {
            return 180 + 180 + value;
        }
        return value;
    }
}
