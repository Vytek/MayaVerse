using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform), true)]
public class ToolManager : TransformInspector {

    private Transform[] objectSelection = new Transform[]{};
    private Tool savedTool = Tool.None;

    private void OnSceneGUI() {
        // Check if the selection of objects in the hierarchy has changed
        if (HasSelectionChanged(objectSelection, Selection.transforms)) {
            RemoveToolhandle(objectSelection, savedTool);
            AddToolHandle(Selection.transforms, savedTool);
            objectSelection = Selection.transforms;
        }

        // Check if the user has switched to a different default tool
        if (Tools.current != savedTool) {
            RemoveToolhandle(objectSelection, savedTool);
            AddToolHandle(objectSelection, Tools.current);
            savedTool = Tools.current;
        }
    }

    // Compare 2 transform arrays 
    private bool HasSelectionChanged(Transform[] oldSelection, Transform[] newSelection) {
        if (oldSelection.Length == newSelection.Length) {
            for (int _i = 0; _i < oldSelection.Length; _i++) {
                if (oldSelection[_i] != newSelection[_i]) {
                    return true;
                }
            }
        } else {
            return true;
        }
        return false;
    }

    // Remove the Tool handle script for all previous selected GameObjects in the inspector
    private void RemoveToolhandle(Transform[] selection, Tool oldTool) {
        for (int _i = 0; _i < selection.Length; _i++) {
            if (oldTool == Tool.Move && selection[_i].gameObject.GetComponent<MoveHandle>() != null) {
                DestroyImmediate(selection[_i].gameObject.GetComponent<MoveHandle>());
            }

            if (oldTool == Tool.Rotate && selection[_i].gameObject.GetComponent<RotateHandle>() != null) {
                DestroyImmediate(selection[_i].gameObject.GetComponent<RotateHandle>());
            }

            if (oldTool == Tool.Scale && selection[_i].gameObject.GetComponent<ScaleHandle>() != null) {
                DestroyImmediate(selection[_i].gameObject.GetComponent<ScaleHandle>());
            }
        }
    }

    // Add a Tool handle script to every selected GameObject in the hierarchy.
    private void AddToolHandle(Transform[] selection, Tool newTool) {
        for (int _i = 0; _i < selection.Length; _i++) {
            if (newTool == Tool.Move && selection[_i].gameObject.GetComponent<MoveHandle>() == null) {
                selection[_i].gameObject.AddComponent<MoveHandle>();
            }

            if (newTool == Tool.Rotate && selection[_i].gameObject.GetComponent<RotateHandle>() == null) {
                selection[_i].gameObject.AddComponent<RotateHandle>();
            }

            if (newTool == Tool.Scale && selection[_i].gameObject.GetComponent<ScaleHandle>() == null) {
                selection[_i].gameObject.AddComponent<ScaleHandle>();
            }
        }
    }
}
