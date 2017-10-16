

////////////////////////////////////////////////
/// Custom property drawer for LSkyFloat ///
////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;

namespace AC.LSky
{

	//[CustomPropertyDrawer(typeof(LSkyFloat))]
	public class LSkyFloatDrawer : PropertyDrawer
	{


		string displayName;
		//----------------------------------

		SerializedProperty valueType;
		SerializedProperty inputValue;
		SerializedProperty curve;
		//----------------------------------

		bool isCached = false;
		//----------------------------------

		string[] options = new string[]
		{
			"V", "C",
		};



		public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{

			if(!isCached) 
			{

				displayName = property.displayName;
				property.Next(true);

				valueType = property.Copy();
				property.NextVisible(true);

				inputValue = property.Copy();
				property.NextVisible(true);

				curve = property.Copy();
				property.NextVisible(true);

				isCached = true;
			}
			//----------------------------------------------------------------------------


			rect.height = 20f; rect.width *= 0.90f; 
			EditorGUI.indentLevel = 0;
			//----------------------------------------------------------------------------

			if(valueType.enumValueIndex == 0) // Input value.
			{
				
				EditorGUI.PropertyField(rect, inputValue,  new GUIContent(displayName));

			}
			else // Input curve.
			{
				EditorGUI.PropertyField(rect, curve, new GUIContent(displayName));
			}
			//----------------------------------------------------------------------------

			Rect switchRect     = rect; 
			switchRect.x       += rect.width; //buttonRect.y += 2.5f; 
			switchRect.height   = 20; switchRect.width *= 0.1f;
			//----------------------------------------------------------------------------

			valueType.enumValueIndex = EditorGUI.Popup(switchRect, "", valueType.enumValueIndex, options, EditorStyles.label); 

		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
		{
			return base.GetPropertyHeight(property, label) + 5;
		}

	}
}