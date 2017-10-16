

///////////////////////////////////////////
/// Small utility for get color values. ///
///////////////////////////////////////////

using UnityEngine;
using UnityEditor;

namespace AC.EditorUtility
{

	public class AC_ColorValueIdentifier : EditorWindow
	{


		[MenuItem("AC/Utility/Color Value Identifier")]
		static void Init()
		{
			AC_ColorValueIdentifier window = (AC_ColorValueIdentifier)EditorWindow.GetWindow<AC_ColorValueIdentifier>();
			window.Show();
		}
		//---------------------------------------------------------------------------------------------------------------------

		internal Color   inputColor = Color.white;
		internal Vector3 RGB        = Vector3.zero;
		internal float   alpha      = 1;

		void OnGUI()
		{
			AC_EditorUtility.ShurikenHeader("Color Value Identifier", TextTitleStyle);
			inputColor = EditorGUILayout.ColorField("Color", inputColor);
			RGB        = EditorGUILayout.Vector3Field("RGB", ColorToVector3(inputColor));
			alpha      = EditorGUILayout.FloatField("Alpha", inputColor.a);
		}
		//----------------------------------------------------------------------------------------------------------------------

		Vector3 ColorToVector3(Color color)
		{
			return new Vector3() 
			{

				x = color.r,
				y = color.g,
				z = color.b
			};
		}
		//----------------------------------------------------------------------------------------------------------------------

		internal GUIStyle TextTitleStyle
		{

			get 
			{

				GUIStyle style         = new GUIStyle(EditorStyles.label); 
				style.normal.textColor = new Color(inputColor.r, inputColor.g, inputColor.b, 1);
				style.fontStyle        = FontStyle.Bold;
				style.fontSize         = 15;

				return style;
			}
		}
		//----------------------------------------------------------------------------------------------------------------------
	}
}
