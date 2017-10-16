



using UnityEngine;
using UnityEditor;
using AC.EditorUtility;

namespace AC.LSky
{

	[CustomEditor(typeof(LSky))] 
	public class LSkyEditor : Editor
	{


		SerializedObject  serObj;
//		LSky             _target;
		//----------------------------------------------

		SerializedProperty applySkybox;
		SerializedProperty skyboxMaterial;
		SerializedProperty moonTexture;
		SerializedProperty outerSpaceCube;
		SerializedProperty starsNoiseCube;
		//----------------------------------------------

		SerializedProperty sunLight;
		SerializedProperty moonLight;
		//----------------------------------------------

		SerializedProperty wavelengthR;
		SerializedProperty wavelengthG;
		SerializedProperty wavelengthB;
		SerializedProperty atmosphereThickness;

		SerializedProperty dayAtmosphereTint;
		SerializedProperty nightColorType;
		SerializedProperty moonInfluence;
		SerializedProperty nightAtmosphereTint;

		SerializedProperty sunBrightness;
		SerializedProperty mie;

		SerializedProperty sunMieColor;
		SerializedProperty sunMieAnisotropy;
		SerializedProperty sunMieScattering;

		SerializedProperty moonMieColor;
		SerializedProperty moonMieAnisotropy;
		SerializedProperty moonMieScattering;
		SerializedProperty moonMieMultiplier;
		//----------------------------------------------

		SerializedProperty enableSunDisc;
		SerializedProperty sunDiscSize;
		SerializedProperty sunDiscColor;

		SerializedProperty enableMoon;
		SerializedProperty moonSize;
		SerializedProperty moonColor;
		SerializedProperty moonIntensity;
		SerializedProperty moonMultiplier;
		//----------------------------------------------

		SerializedProperty enableStars;
		SerializedProperty outerSpaceOffset;
		SerializedProperty starsColor;
		SerializedProperty starsIntensity;
		SerializedProperty starsScintillation;
		SerializedProperty starsScintillationSpeed;

		SerializedProperty enableNebula;
		SerializedProperty nebulaColor;
		SerializedProperty nebulaIntensity;

		//----------------------------------------------
		SerializedProperty HDR;
		SerializedProperty exposure;
		SerializedProperty colorSpace;
		//----------------------------------------------

		SerializedProperty sunLightColor;
		SerializedProperty sunLightIntensity;
		SerializedProperty sunLightThreshold;
		//----------------------------------------------

		SerializedProperty moonLightColor;
		SerializedProperty moonLightIntensity;
		SerializedProperty moonLightMultiplier;
		//----------------------------------------------

		SerializedProperty ambientMode;
		SerializedProperty ambientSkyColor;
		SerializedProperty ambientEquatorColor;
		SerializedProperty ambientGroundColor;
		SerializedProperty ambientIntensity;
		//----------------------------------------------

		SerializedProperty enableUnityFog;
		SerializedProperty unityFogMode;
		SerializedProperty unityFogColor;
		SerializedProperty unityFogDensity;
		SerializedProperty unityFogStartDistance;
		SerializedProperty unityFogEndDistance;
		//----------------------------------------------

		SerializedProperty CRFoldout;
		SerializedProperty atmosphereFoldout;
		SerializedProperty celestialsFoldout;
		SerializedProperty colorCorrectionFoldout;
		SerializedProperty lightingFoldout;
		//----------------------------------------------

		void OnEnable()
		{

			serObj  = new SerializedObject(target);
		//	_target = (LSky)target;
			//----------------------------------------------------------------

			applySkybox    = serObj.FindProperty("applySkybox");
			skyboxMaterial = serObj.FindProperty("skyboxMaterial");
			moonTexture    = serObj.FindProperty("moonTexture");
			outerSpaceCube = serObj.FindProperty("outerSpaceCube");
			starsNoiseCube = serObj.FindProperty("starsNoiseCube");
			//----------------------------------------------------------------

			sunLight  = serObj.FindProperty("m_SunLight");
			moonLight = serObj.FindProperty("m_MoonLight");
			//----------------------------------------------------------------

			wavelengthR = serObj.FindProperty("wavelengthR");
			wavelengthG = serObj.FindProperty("wavelengthG");
			wavelengthB = serObj.FindProperty("wavelengthB");
			atmosphereThickness = serObj.FindProperty("atmosphereThickness");

			dayAtmosphereTint   = serObj.FindProperty("dayAtmosphereTint");
			nightColorType 	    = serObj.FindProperty("nightColorType");
			moonInfluence 	    = serObj.FindProperty("moonInfluence");
			nightAtmosphereTint = serObj.FindProperty("nightAtmosphereTint");

			sunBrightness = serObj.FindProperty("sunBrightness");
			mie			  = serObj.FindProperty("mie");

			sunMieColor      = serObj.FindProperty("sunMieColor");
			sunMieAnisotropy = serObj.FindProperty("sunMieAnisotropy");
			sunMieScattering = serObj.FindProperty("sunMieScattering");

			moonMieColor      = serObj.FindProperty("moonMieColor");
			moonMieAnisotropy = serObj.FindProperty("moonMieAnisotropy");
			moonMieScattering = serObj.FindProperty("moonMieScattering");
			moonMieMultiplier = serObj.FindProperty("moonMieMultiplier");
			//----------------------------------------------------------------

			enableSunDisc = serObj.FindProperty("enableSunDisc");
			sunDiscSize   = serObj.FindProperty("sunDiscSize");
			sunDiscColor  = serObj.FindProperty("sunDiscColor");

			enableMoon      = serObj.FindProperty("enableMoon");
			moonSize        = serObj.FindProperty("moonSize");
			moonColor       = serObj.FindProperty("moonColor");
			moonIntensity   = serObj.FindProperty("moonIntensity");
			moonMultiplier  = serObj.FindProperty("moonMultiplier");
			//----------------------------------------------------------------

			outerSpaceOffset         = serObj.FindProperty("outerSpaceOffset");
			enableStars 			 = serObj.FindProperty("enableStars");
			starsColor               = serObj.FindProperty("starsColor");
			starsIntensity           = serObj.FindProperty("starsIntensity");
			starsScintillation       = serObj.FindProperty("starsScintillation");
			starsScintillationSpeed  = serObj.FindProperty("starsScintillationSpeed");
			//----------------------------------------------------------------

			enableNebula 	= serObj.FindProperty("enableNebula");
			nebulaColor     = serObj.FindProperty("nebulaColor");
			nebulaIntensity = serObj.FindProperty("nebulaIntensity");
			//----------------------------------------------------------------

			HDR         = serObj.FindProperty("HDR");
			exposure	= serObj.FindProperty("exposure");
			colorSpace 	= serObj.FindProperty("m_ColorSpace");
			//----------------------------------------------------------------

			sunLightColor	    = serObj.FindProperty("sunLightColor");
			sunLightIntensity	= serObj.FindProperty("sunLightIntensity");
			sunLightThreshold   = serObj.FindProperty("sunLightThreshold");
			//----------------------------------------------------------------


			moonLightColor	    = serObj.FindProperty("moonLightColor");
			moonLightIntensity	= serObj.FindProperty("moonLightIntensity");
			moonLightMultiplier = serObj.FindProperty("moonLightMultiplier");
			//----------------------------------------------------------------

			ambientMode	        = serObj.FindProperty("ambientMode");
			ambientSkyColor     = serObj.FindProperty("ambientSkyColor");
			ambientEquatorColor = serObj.FindProperty("ambientEquatorColor");
			ambientGroundColor  = serObj.FindProperty("ambientGroundColor");
			ambientIntensity    = serObj.FindProperty("ambientIntensity");
			//----------------------------------------------------------------

			enableUnityFog	       = serObj.FindProperty("enableUnityFog");
			unityFogMode	       = serObj.FindProperty("unityFogMode");
			unityFogColor          = serObj.FindProperty("unityFogColor");
			unityFogDensity        = serObj.FindProperty("unityFogDensity");
			unityFogStartDistance  = serObj.FindProperty("unityFogStartDistance");
			unityFogEndDistance    = serObj.FindProperty("unityFogEndDistance");
			//----------------------------------------------------------------

			CRFoldout         = serObj.FindProperty("CRFoldout");
			atmosphereFoldout = serObj.FindProperty("atmosphereFoldout");
			celestialsFoldout = serObj.FindProperty("celestialsFoldout");
			colorCorrectionFoldout = serObj.FindProperty ("colorCorrectionFoldout");
			lightingFoldout   = serObj.FindProperty ("lightingFoldout");
			//----------------------------------------------------------------
		}

		public override void OnInspectorGUI()
		{
			serObj.Update ();

			EditorGUILayout.Separator();
			AC_EditorUtility.ShurikenHeader("LSky", TextTitleStyle, 30);

			AC_EditorUtility.ShurikenFoldoutHeader("Components And Resources", TextTitleStyle, CRFoldout);
			if(CRFoldout.boolValue) 
				ComponentsAndResources();
		
			AC_EditorUtility.ShurikenFoldoutHeader("Atmosphere", TextTitleStyle, atmosphereFoldout);
			if(atmosphereFoldout.boolValue) 
				Atmosphere();

			AC_EditorUtility.ShurikenFoldoutHeader("Celestials", TextTitleStyle, celestialsFoldout);
			if(celestialsFoldout.boolValue) 
				Celestials();

			AC_EditorUtility.ShurikenFoldoutHeader("Color Correction", TextTitleStyle, colorCorrectionFoldout);
			if(colorCorrectionFoldout.boolValue) 
				ColorCorrection();


			AC_EditorUtility.ShurikenFoldoutHeader("Lighting", TextTitleStyle, lightingFoldout);
			if(lightingFoldout.boolValue) 
				Lighting();

			serObj.ApplyModifiedProperties();
		}

		void ComponentsAndResources()
		{

			AC_EditorUtility.Separator(SeparatorHeight);

			EditorGUILayout.PropertyField(applySkybox, new GUIContent("Apply Skybox")); 
			EditorGUILayout.PropertyField(skyboxMaterial, new GUIContent("Skybox Material")); 
			if(skyboxMaterial.objectReferenceValue == null) 
				EditorGUILayout.HelpBox("Skybox material no assigned", MessageType.Warning);

			EditorGUILayout.PropertyField(moonTexture, new GUIContent("Moon Texture")); 
			if(moonTexture.objectReferenceValue == null) 
				EditorGUILayout.HelpBox("Moon texture no assigned", MessageType.Warning);

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			{
				EditorGUILayout.PropertyField (outerSpaceCube, new GUIContent ("Outer Space Cube")); 

				EditorGUILayout.HelpBox ("RGB = Nebula, A = StarField", MessageType.Info);

				if (outerSpaceCube.objectReferenceValue == null)
					EditorGUILayout.HelpBox ("Outer space cubemap no assigned", MessageType.Warning);
			}
			EditorGUILayout.EndVertical ();


			EditorGUILayout.PropertyField(starsNoiseCube, new GUIContent("Stars Noise Cube")); 
			if(starsNoiseCube.objectReferenceValue == null) 
				EditorGUILayout.HelpBox("Noise cubemap no assigned", MessageType.Warning);
			
			AC_EditorUtility.Separator(SeparatorHeight);
			EditorGUILayout.Separator();

			EditorGUILayout.PropertyField(sunLight, new GUIContent("Sun Light")); 
			if(sunLight.objectReferenceValue == null) 
				EditorGUILayout.HelpBox("Sun light no assigned", MessageType.Warning);

			EditorGUILayout.PropertyField(moonLight, new GUIContent("Moon Light")); 
			if(moonLight.objectReferenceValue == null) 
				EditorGUILayout.HelpBox("Moon light no assigned", MessageType.Warning);

			AC_EditorUtility.Separator(SeparatorHeight);
			EditorGUILayout.Separator();
		}

		void Atmosphere()
		{


			AC_EditorUtility.Separator(SeparatorHeight);

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(wavelengthR, new GUIContent("")); 
				GUILayout.Button(new GUIContent("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(wavelengthG, new GUIContent("")); 
				GUILayout.Button(new GUIContent("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(wavelengthB, new GUIContent("")); 
				GUILayout.Button(new GUIContent("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(atmosphereThickness, new GUIContent("")); 
				GUILayout.Button(new GUIContent("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();

			AC_EditorUtility.Separator(SeparatorHeight);
			EditorGUILayout.Separator();
			//-------------------------------------------------------------------------------------------------------------------------------


			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField (dayAtmosphereTint, new GUIContent ("")); 
				GUILayout.Button (new GUIContent ("?", EvaluateInfoSunAbove), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.PropertyField(nightColorType, new GUIContent("Night Color Type")); 
			moonInfluence.boolValue = EditorGUILayout.Toggle(new GUIContent("Moon Influence", "The moon affects the color of the atmosphere"), moonInfluence.boolValue, EditorStyles.radioButton);


			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField (nightAtmosphereTint, new GUIContent ("")); 
				GUILayout.Button(new GUIContent ("?", moonInfluence.boolValue ? EvaluateInfoMoonAbove : EvaluateInfoSunBelow), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();

			AC_EditorUtility.Separator(SeparatorHeight);
			EditorGUILayout.Separator();
			//-------------------------------------------------------------------------------------------------------------------------------

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(sunBrightness, new GUIContent ("")); 
				GUILayout.Button(new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(mie, new GUIContent ("")); 
				GUILayout.Button(new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();

			AC_EditorUtility.Separator(SeparatorHeight);
			EditorGUILayout.Separator();
			//-------------------------------------------------------------------------------------------------------------------------------

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(sunMieColor, new GUIContent ("")); 
				GUILayout.Button(new GUIContent ("?", EvaluateInfoSunAbove), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(sunMieAnisotropy, new GUIContent ("")); 
				GUILayout.Button(new GUIContent ("?", EvaluateInfoSunAbove), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(sunMieScattering, new GUIContent ("")); 
				GUILayout.Button(new GUIContent ("?", EvaluateInfoSunAbove), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();

			AC_EditorUtility.Separator(SeparatorHeight);
			EditorGUILayout.Separator();
			//-------------------------------------------------------------------------------------------------------------------------------

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(moonMieColor, new GUIContent ("")); 
				GUILayout.Button(new GUIContent ("?", EvaluateInfoMoonAbove), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(moonMieAnisotropy, new GUIContent ("")); 
				GUILayout.Button(new GUIContent ("?", EvaluateInfoMoonAbove), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(moonMieScattering, new GUIContent ("")); 
				GUILayout.Button(new GUIContent ("?", EvaluateInfoMoonAbove), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(moonMieMultiplier, new GUIContent ("")); 
				GUILayout.Button(new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();

			AC_EditorUtility.Separator(SeparatorHeight);
			EditorGUILayout.Separator();
			//-------------------------------------------------------------------------------------------------------------------------------
		}
			
		void Celestials()
		{

			AC_EditorUtility.Separator(SeparatorHeight);

			EditorGUILayout.PropertyField(enableSunDisc, new GUIContent ("Sun Disc")); 
			if(enableSunDisc.boolValue)
			{

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PropertyField(sunDiscSize, new GUIContent("")); 
					GUILayout.Button(new GUIContent ("?", EvaluateInfoSunAbove), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PropertyField(sunDiscColor, new GUIContent("")); 
					GUILayout.Button(new GUIContent ("?", EvaluateInfoSunAbove), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
				}
				EditorGUILayout.EndHorizontal();
			}

			AC_EditorUtility.Separator(SeparatorHeight);
			EditorGUILayout.Separator();
			//-------------------------------------------------------------------------------------------------------------------------------

			EditorGUILayout.PropertyField(enableMoon, new GUIContent ("Moon")); 
			if(enableMoon.boolValue) 
			{

				EditorGUILayout.BeginHorizontal ();
				{
					EditorGUILayout.PropertyField (moonSize, new GUIContent ("")); 
					GUILayout.Button (new GUIContent ("?", EvaluateInfoMoonAbove), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
				}
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				{
					EditorGUILayout.PropertyField (moonColor, new GUIContent ("")); 
					GUILayout.Button (new GUIContent ("?", EvaluateInfoMoonAbove), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
				}
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				{
					EditorGUILayout.PropertyField (moonIntensity, new GUIContent ("")); 
					GUILayout.Button (new GUIContent ("?", EvaluateInfoMoonAbove), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
				}
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				{
					EditorGUILayout.PropertyField (moonMultiplier, new GUIContent ("")); 
					GUILayout.Button (new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
				}
				EditorGUILayout.EndHorizontal ();

			}

			AC_EditorUtility.Separator(SeparatorHeight);
			EditorGUILayout.Separator();
			//-------------------------------------------------------------------------------------------------------------------------------


			EditorGUILayout.PropertyField(enableStars, new GUIContent ("Stars")); 
			if(enableStars.boolValue)
			{

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PropertyField(starsColor, new GUIContent("")); 
					GUILayout.Button(new GUIContent("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PropertyField(starsIntensity, new GUIContent ("")); 
					GUILayout.Button(new GUIContent("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
				}
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				{
					EditorGUILayout.PropertyField (starsScintillation, new GUIContent ("")); 
					GUILayout.Button (new GUIContent("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PropertyField(starsScintillationSpeed, new GUIContent("")); 
					GUILayout.Button(new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
				}
				EditorGUILayout.EndHorizontal ();

			}

			AC_EditorUtility.Separator(SeparatorHeight);
			EditorGUILayout.Separator();
			//-------------------------------------------------------------------------------------------------------------------------------


			EditorGUILayout.PropertyField(enableNebula, new GUIContent ("Nebula")); 
			if(enableNebula.boolValue)
			{

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PropertyField (nebulaColor, new GUIContent ("")); 
					GUILayout.Button(new GUIContent("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PropertyField (nebulaIntensity, new GUIContent ("")); 
					GUILayout.Button(new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
				}
				EditorGUILayout.EndHorizontal();
			}

			if(enableStars.boolValue || enableNebula.boolValue)
				EditorGUILayout.PropertyField(outerSpaceOffset, new GUIContent("Offsets")); 
			

			AC_EditorUtility.Separator(SeparatorHeight);
			EditorGUILayout.Separator();
			//-------------------------------------------------------------------------------------------------------------------------------
		}

		void ColorCorrection()
		{

			AC_EditorUtility.Separator(SeparatorHeight);

			EditorGUILayout.PropertyField(HDR, new GUIContent ("HDR")); 

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(exposure, new GUIContent ("")); 
				GUILayout.Button(new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.PropertyField(colorSpace, new GUIContent ("Color Space")); 

			AC_EditorUtility.Separator(SeparatorHeight);
			EditorGUILayout.Separator();
			//-------------------------------------------------------------------------------------------------------------------------------

		}

		void Lighting()
		{
			
			AC_EditorUtility.Separator(SeparatorHeight);

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(sunLightColor, new GUIContent ("")); 
				GUILayout.Button(new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(sunLightIntensity, new GUIContent ("")); 
				GUILayout.Button(new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(sunLightThreshold, new GUIContent("Sun Light Threshold")); 
				GUILayout.Button(new GUIContent ("?", "Enable/Disable sun light threshold"), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();


			AC_EditorUtility.Separator(SeparatorHeight);
			EditorGUILayout.Separator();
			//-------------------------------------------------------------------------------------------------------------------------------

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(moonLightColor, new GUIContent ("")); 
				GUILayout.Button(new GUIContent ("?", EvaluateInfoMoonAbove), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(moonLightIntensity, new GUIContent ("")); 
				GUILayout.Button(new GUIContent ("?", EvaluateInfoMoonAbove), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PropertyField(moonLightMultiplier, new GUIContent("Sun Light Threshold")); 
				GUILayout.Button(new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal();

			AC_EditorUtility.Separator(SeparatorHeight);
			EditorGUILayout.Separator();
			//-------------------------------------------------------------------------------------------------------------------------------

			EditorGUILayout.PropertyField(ambientMode, new GUIContent("Ambient Mode")); 

			if(ambientMode.enumValueIndex != 3)
			{

				if(ambientMode.enumValueIndex == 0)
				{

					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.PropertyField(ambientIntensity, new GUIContent("Ambient Intensity")); 
						GUILayout.Button(new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width(InfoWidth)); 
					}
					EditorGUILayout.EndHorizontal();
				}

				if(ambientMode.enumValueIndex != 0) 
				{
					EditorGUILayout.BeginHorizontal ();
					{
						EditorGUILayout.PropertyField (ambientSkyColor, new GUIContent ("Ambient Sky Color")); 
						GUILayout.Button (new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
					}
					EditorGUILayout.EndHorizontal ();
				}

				if(ambientMode.enumValueIndex == 1) 
				{

					EditorGUILayout.BeginHorizontal ();
					{
						EditorGUILayout.PropertyField (ambientEquatorColor, new GUIContent ("Ambient Equator Color")); 
						GUILayout.Button (new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
					}
					EditorGUILayout.EndHorizontal ();
				}
			}

			EditorGUILayout.BeginHorizontal ();
			{
				EditorGUILayout.PropertyField (ambientGroundColor, new GUIContent ("Ambient Ground Color")); 
				GUILayout.Button (new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
			}
			EditorGUILayout.EndHorizontal ();

			AC_EditorUtility.Separator(SeparatorHeight);
			EditorGUILayout.Separator();
			//-------------------------------------------------------------------------------------------------------------------------------

			EditorGUILayout.PropertyField(enableUnityFog, new GUIContent ("Enable Unity Fog")); 

			if(enableUnityFog.boolValue)
			{


				EditorGUILayout.PropertyField(unityFogMode, new GUIContent ("Unity Fog Mode")); 

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PropertyField(unityFogColor, new GUIContent ("")); 
					GUILayout.Button (new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
				}
				EditorGUILayout.EndHorizontal();

				if (unityFogMode.enumValueIndex != 0)
				{

					EditorGUILayout.BeginHorizontal ();
					{
						EditorGUILayout.PropertyField (unityFogDensity, new GUIContent ("")); 
						GUILayout.Button (new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
					}
					EditorGUILayout.EndHorizontal ();

				} 
				else 
				{

					EditorGUILayout.BeginHorizontal ();
					{
						EditorGUILayout.PropertyField (unityFogStartDistance, new GUIContent ("")); 
						GUILayout.Button (new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
					}
					EditorGUILayout.EndHorizontal ();


					EditorGUILayout.BeginHorizontal ();
					{
						EditorGUILayout.PropertyField (unityFogEndDistance, new GUIContent ("")); 
						GUILayout.Button (new GUIContent ("?", EvaluateInfoSun), EditorStyles.miniLabel, GUILayout.Width (InfoWidth)); 
					}
					EditorGUILayout.EndHorizontal ();
				}
			}

			AC_EditorUtility.Separator(SeparatorHeight);
			EditorGUILayout.Separator();
			//-------------------------------------------------------------------------------------------------------------------------------
		}


		internal GUIStyle TextTitleStyle
		{

			get 
			{

				GUIStyle style = new GUIStyle(EditorStyles.label); 
				style.fontStyle = FontStyle.Bold;
				style.fontSize = 15;

				return style;
			}
		}
		//--------------------------------------------------------------------------------

		internal int SeparatorHeight{ get{ return 2; } }
		internal int InfoWidth{ get { return 12; } }
		//--------------------------------------------------------------------------------

		internal string EvaluateInfoSun
		{
			get { return "Evaluate time by sun direction in complete cycle"; }
		}

		internal string EvaluateInfoSunAbove
		{
			get { return "Evaluate time by sun direction only above the horizon"; }
		}

		internal string EvaluateInfoSunBelow
		{
			get { return "Evaluate time by sun direction only below the horizon"; }
		}

		internal string EvaluateInfoMoonAbove
		{
			get { return "Evaluate time by moon direction only above the horizon"; }
		}
		//--------------------------------------------------------------------------------

	}
}