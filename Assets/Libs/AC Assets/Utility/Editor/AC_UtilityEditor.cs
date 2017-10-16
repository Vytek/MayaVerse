
///////////////////////////////////
/// Utility methods for editor. ///
///////////////////////////////////


#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace AC.EditorUtility
{

	public static class AC_EditorUtility 
	{


		#region Color

		/// <summary>
		/// Custom Color property field.
		/// </summary>
		/// <param name="color">Color.</param>
		/// <param name="name">Name.</param>
		/// <param name="width">Width.</param>
		public static void ColorField(SerializedProperty color, string name, int width)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(name, GUILayout.MinWidth(20));
			EditorGUILayout.PropertyField(color, new GUIContent(""), GUILayout.MaxWidth(width),GUILayout.MinWidth(width * 0.5f));
			EditorGUILayout.EndHorizontal();
		}

		#endregion

		#region Curves

		/// <summary>
		/// Custom Curve Field.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="curve">Curve.</param>
		/// <param name="color">Color.</param>
		/// <param name="rect">Rect.</param>
		/// <param name="width">Width.</param>
		public static void CurveField(string name,  SerializedProperty curve, Color color, Rect rect, int width)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(name,GUILayout.MinWidth(20));
			curve.animationCurveValue = EditorGUILayout.CurveField("", curve.animationCurveValue, color , rect, GUILayout.MaxWidth(width), GUILayout.MinWidth(width*0.5f));
			EditorGUILayout.EndHorizontal();
		}

		#endregion

		#region Separator

		/// <summary>
		/// Horizontal line separator.
		/// </summary>
		/// <param name="height">Height.</param>
		public static void Separator(int height)
		{
			GUILayout.Box("", new GUILayoutOption[] {GUILayout.ExpandWidth(true), GUILayout.Height(height)});
		}

		/// <summary>
		/// Horizontal line separator.
		/// </summary>
		/// <param name="height">Height.</param>
		/// <param name="color">Color.</param>
		public static void Separator(int height, Color color)
		{
			GUI.color = color;
			GUILayout.Box("", new GUILayoutOption[] {GUILayout.ExpandWidth(true), GUILayout.Height(height)});
			GUI.color = Color.white;
		}
		#endregion

		#region Label

		/// <summary>
		/// Custom Label for text.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="textStyle">Text style.</param>
		/// <param name="center">If set to <c>true</c> center.</param>
		public static void Label(string text, GUIStyle textStyle, bool center)
		{

			if(center)
			{


				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label(text, textStyle);
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();

			} 
			else
			{
				GUILayout.Label(text, textStyle);
			}
				
		}

		/// <summary>
		/// Custom Label for text.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="textStyle">Text style.</param>
		/// <param name="center">If set to <c>true</c> center.</param>
		/// <param name="width">Width.</param>
		public static void Label(string text, GUIStyle textStyle, bool center, int width)
		{

			if(center)
			{


				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label(text, textStyle, GUILayout.Width(width));
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
			} 
			else
			{
				
				GUILayout.Label(text, textStyle, GUILayout.Width(width));
			}
		}
			
		#endregion

		#region Foldout Header

		/// <summary>
		/// Foldout header with suriken style.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="texStyle">Tex style.</param>
		/// <param name="foldout">Foldout.</param>
		public static void ShurikenFoldoutHeader(string text, GUIStyle texStyle, SerializedProperty foldout)
		{


			GUIStyle h = new GUIStyle("ShurikenModuleTitle")
			{
				font          = new GUIStyle("Label").font,
				border        = new RectOffset(15, 7, 4, 4),
				fixedHeight   = 22,
				contentOffset = new Vector2(20f, -2f)
			}; 

			EditorGUILayout.BeginHorizontal(h, GUILayout.Height(25));
			{

				foldout.boolValue = GUILayout.Toggle(foldout.boolValue, new GUIContent(text), EditorStyles.foldout, GUILayout.Width(25));
				//Label (tex, texStyle, true);
			}
			EditorGUILayout.EndHorizontal();
		}

		/// <summary>
		/// Foldout header with suriken style.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="texStyle">Tex style.</param>
		/// <param name="col">Col.</param>
		/// <param name="foldout">Foldout.</param>
		public static void ShurikenFoldoutHeader(string text, GUIStyle texStyle, Color col, SerializedProperty foldout)
		{


			GUIStyle h = new GUIStyle("ShurikenModuleTitle")
			{
				font          = new GUIStyle("Label").font,
				border        = new RectOffset(15, 7, 4, 4),
				fixedHeight   = 22,
				contentOffset = new Vector2(20f, -2f)
			}; 

			GUI.backgroundColor = col;
			EditorGUILayout.BeginHorizontal(h, GUILayout.Height(25));
			GUI.backgroundColor = Color.white;
			{
				foldout.boolValue = GUILayout.Toggle(foldout.boolValue, new GUIContent(text), EditorStyles.foldout, GUILayout.Width(25));
				//Label (tex, texStyle, true);
			}
			EditorGUILayout.EndHorizontal();
		}

		#endregion 

		#region Header

		/// <summary>
		/// Header with suriken style.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="texStyle">Tex style.</param>
		public static void ShurikenHeader(string text, GUIStyle texStyle)
		{


			GUIStyle h = new GUIStyle("ShurikenModuleTitle")
			{
				font          = new GUIStyle("Label").font,
				border        = new RectOffset(15, 7, 4, 4),
				fixedHeight   = 30,
				contentOffset = new Vector2(20f, -2f)
			}; 

			EditorGUILayout.BeginHorizontal(h, GUILayout.Height(25));
			{
				Label(text, texStyle, true);
			}
			EditorGUILayout.EndHorizontal();
		}

		/// <summary>
		/// Header with suriken style.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="texStyle">Tex style.</param>
		/// <param name="height">Height.</param>
		public static void ShurikenHeader(string text, GUIStyle texStyle, int height)
		{


			GUIStyle h = new GUIStyle("ShurikenModuleTitle")
			{
				font          = new GUIStyle("Label").font,
				border        = new RectOffset(15, 7, 4, 4),
				fixedHeight   = height,
				contentOffset = new Vector2(20f, -2f)
			}; 

			EditorGUILayout.BeginHorizontal(h, GUILayout.Height(25)); 
			{
				Label(text, texStyle, true);
			}
			EditorGUILayout.EndHorizontal();
		} 

		#endregion

		#region Progress Bar

		/// <summary>
		/// 100% progress bar.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="name">Name.</param>
		public static void ProgressBar(float value, string name)
		{

			EditorGUI.ProgressBar(GUILayoutUtility.GetRect(0,20),  value/100f, name);
		}

		/// <summary>
		/// 100% progress bar
		/// </summary>
		/// <param name="rect">Rect.</param>
		/// <param name="value">Value.</param>
		/// <param name="name">Name.</param>
		public static void ProgressBar(Rect rect, float value, string name)
		{

			EditorGUI.ProgressBar(rect, value/100f, name);
		}

		#endregion
	}
}
#endif