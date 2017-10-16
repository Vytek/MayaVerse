using UnityEditor;
using UnityEngine;

namespace UnityNotes
{
	[CustomEditor(typeof(Note))]
	public class NoteEditor : Editor
	{
		bool editable = false;
		SerializedProperty noteContent;
		SerializedProperty noteTitle;
		GUIStyle textAreaStyle;
		GUIStyle labelStyle;
		GUIStyle labelBoldStyle;
		GUILayoutOption[] options = { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false) };

		void OnEnable()
		{
			editable = false;
			noteContent = serializedObject.FindProperty("note");
			noteTitle = serializedObject.FindProperty("title");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			if (textAreaStyle == null)
			{
				textAreaStyle = new GUIStyle(GUI.skin.GetStyle("textArea"));
				textAreaStyle.wordWrap = true;
				textAreaStyle.stretchWidth = true;
			}

			if (labelStyle == null)
			{
				labelStyle = new GUIStyle(GUI.skin.GetStyle("label"));
				labelStyle.wordWrap = true;
				labelStyle.stretchWidth = true;
				labelStyle.fontStyle = FontStyle.Normal;
			}

			if (labelBoldStyle == null)
			{
				labelBoldStyle = new GUIStyle(GUI.skin.GetStyle("label"));
				labelBoldStyle.fontStyle = FontStyle.Bold;
			}

			if (GUILayout.Button(editable ? "Save" : "Edit"))
			{
				editable = !editable;
			}

			if (editable)
			{
				noteTitle.stringValue = EditorGUILayout.TextField(noteTitle.stringValue);
				noteContent.stringValue = EditorGUILayout.TextArea(noteContent.stringValue, textAreaStyle, options);
			}
			else
			{
				EditorGUILayout.LabelField(noteTitle.stringValue, labelBoldStyle);
				EditorGUILayout.LabelField(noteContent.stringValue, labelStyle, options);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}