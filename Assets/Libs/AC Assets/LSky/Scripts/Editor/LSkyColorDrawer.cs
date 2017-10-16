
/////////////////////////////////////////////
/// Custom property drawer for LSkyColor. ///
/////////////////////////////////////////////


using UnityEngine;
using UnityEditor;


namespace AC.LSky
{

	[CustomPropertyDrawer(typeof(LSkyColor))]
	public class LSkyColorDrawer : PropertyDrawer
	{


		string displayName;
		//----------------------------------

		SerializedProperty colorType;
		SerializedProperty inputColor;
		SerializedProperty gradient;
		//----------------------------------

		bool isCached = false;
		//----------------------------------

		string[] options = new string[]
		{
			"C", "G"
		};


		public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			
			if (!isCached) 
			{

				displayName = property.displayName; 
				property.Next (true);

				colorType = property.Copy ();
				property.NextVisible (true);

				inputColor = property.Copy ();
				property.NextVisible (true);

				gradient = property.Copy ();
				property.NextVisible (true);

				isCached = true;
			}
			//-----------------------------------------------------------------------------------

			rect.height = 20f; rect.width *= 0.90f; 
			EditorGUI.indentLevel = 0;
			//-----------------------------------------------------------------------------------

			if(colorType.enumValueIndex == 0) // Input color.
			{
				EditorGUI.PropertyField(rect, inputColor, new GUIContent(displayName));
			} 
			else
			{
				EditorGUI.PropertyField(rect, gradient, new GUIContent(displayName));
			}
			//-----------------------------------------------------------------------------------

	

			Rect switchRect     = rect; 
			switchRect.x       += rect.width; //buttonRect.y     += 2.5f; 
			switchRect.height   = 20; switchRect.width *= 0.1f;
			//-----------------------------------------------------------------------------------

			colorType.enumValueIndex = EditorGUI.Popup(switchRect, "", colorType.enumValueIndex, options,  EditorStyles.miniLabel); 

		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
		{
			return base.GetPropertyHeight(property, label) + 5;
		}

	}
}