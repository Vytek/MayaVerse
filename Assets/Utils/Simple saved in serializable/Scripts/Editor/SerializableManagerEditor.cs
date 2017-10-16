using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SerializableManager))]
public class SerializableManagerEditor : Editor {
    private String lastpassword = "";
    private float passwordWarning;

    public override void OnInspectorGUI() {
        if (SerializableManager.current != target) {
            GUI.color = Color.red;
            EditorGUILayout.HelpBox("You can only have one instance of Serializable Manager at the same time.", MessageType.Error, true);
            if (SerializableManager.current == null && SerializableManager.current != (SerializableManager)target) {
                SerializableManager.current = ((SerializableManager)target);
            }
            return;
        }

        DrawDefaultInspector();

        serializedObject.Update();

        if (serializedObject.FindProperty("Encrypt").boolValue) {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("password"), true);
            if (passwordWarning > Time.realtimeSinceStartup || (lastpassword != "" && lastpassword != serializedObject.FindProperty("password").stringValue)) {
                EditorGUILayout.HelpBox("Change the password will cause can not be loaded old games", MessageType.Warning, true);
                if (passwordWarning < Time.realtimeSinceStartup)
                    passwordWarning = Time.realtimeSinceStartup + 20;
            }
            lastpassword = serializedObject.FindProperty("password").stringValue;
            
        }

        if (serializedObject.FindProperty("AutoSave").boolValue) {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TimeBetweenSavedSeconds"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CurrentTimeToSave"), true);
        }

        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button("Save All")) {
            SerializableManager.SaveAll();
        }
        if (GUILayout.Button("Load All")) {
            SerializableManager.LoadAll();
        }
        if (!Application.isPlaying) {
            GUI.enabled = true;
            GUI.color = Color.yellow;
            EditorGUILayout.HelpBox("You can only save and load in play mode", MessageType.Warning, true);
            GUI.color = Color.white;
        }


        serializedObject.ApplyModifiedProperties();
    }
}