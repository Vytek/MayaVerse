
///////////////////
/// Parameters. ///
///////////////////

using System;
using UnityEngine;


namespace AC.LSky
{

	public partial class LSky : MonoBehaviour 
	{


		#region Atmosphere

		// Wavelenghts.
		[LSkyFloatAttribute(0.0f, 1000f, 0.0f, 0.0f, 1.0f, 1000f, DefautlColors.red)]
		public LSkyFloat wavelengthR = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 650f,
			curve        = AnimationCurve.Linear(0.0f, 650f, 1.0f, 650f),
			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 1000f, 0.0f, 0.0f, 1.0f, 1000f, DefautlColors.green)]
		public LSkyFloat wavelengthG = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 570f,
			curve        = AnimationCurve.Linear(0.0f, 570f, 1.0f, 570f),
			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 1000f, 0.0f, 0.0f, 1.0f, 1000f, DefautlColors.blue)]
		public LSkyFloat wavelengthB = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 475f,
			curve        = AnimationCurve.Linear(0.0f, 475f, 1.0f, 475f),
			evaluateTime = 0.0f

		};
		//----------------------------------------------------------------------------------------

		[LSkyFloatAttribute(0.0f, 50f, 0.0f, 0.0f, 1.0f, 50f, DefautlColors.white)]
		public LSkyFloat atmosphereThickness = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 1.0f,
			curve        = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f),
			evaluateTime = 0.0f

		};
		//----------------------------------------------------------------------------------------

		// Tint
		public LSkyColor dayAtmosphereTint = new LSkyColor()
		{

			colorType    = LSkyColorType.Color,
			inputColor   = Color.white,  

			gradient     = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.0f),
					new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.5f),
					new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 1.0f)
				},

				alphaKeys = new GradientAlphaKey[] 
				{
					new GradientAlphaKey(1.0f, 0.0f),
					new GradientAlphaKey(1.0f, 1.0f)
				}
			},

			evaluateTime = 0.0f

		};

		public LSkyNightColorType nightColorType = LSkyNightColorType.Atmospheric;
		public bool moonInfluence = true; // Moon affected in atmosphere color.

		public LSkyColor nightAtmosphereTint = new LSkyColor()
		{

			colorType    = LSkyColorType.Gradient,
			inputColor   = new Color(0.039f, 0.079f, 0.111f, 1.0f),  

			gradient     = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(new Color(0.039f, 0.079f, 0.111f, 1.0f), 0.0f),
					new GradientColorKey(new Color(0.039f, 0.079f, 0.111f, 1.0f), 0.5f),
					new GradientColorKey(new Color(0.039f, 0.079f, 0.111f, 1.0f), 1.0f)
				},

				alphaKeys = new GradientAlphaKey[] 
				{
					new GradientAlphaKey(1.0f, 0.0f),
					new GradientAlphaKey(1.0f, 1.0f)
				}
			},

			evaluateTime = 0.0f

		};
		//----------------------------------------------------------------------------------------

		[LSkyFloatAttribute(0.0f, 100f, 0.0f, 0.0f, 1.0f, 100f, DefautlColors.yellow)]
		public LSkyFloat sunBrightness = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 30f,
			curve        = AnimationCurve.Linear(0.0f, 30f, 1.0f, 30f),
			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 0.5f, 0.0f, 0.0f, 1.0f, 0.5f, DefautlColors.yellow)]
		public LSkyFloat mie = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 0.010f,
			curve        = AnimationCurve.Linear(0.0f, 0.010f, 1.0f, 0.010f),
			evaluateTime = 0.0f

		};
		//----------------------------------------------------------------------------------------


		public LSkyColor sunMieColor = new LSkyColor()
		{

			colorType    = LSkyColorType.Color,
			inputColor   = new Color(1.0f, 0.95f, 0.83f, 1.0f),  

			gradient     = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(new Color(1.0f, 0.95f, 0.83f, 1.0f), 0.0f),
					new GradientColorKey(new Color(1.0f, 0.95f, 0.83f, 1.0f), 0.5f),
					new GradientColorKey(new Color(1.0f, 0.95f, 0.83f, 1.0f), 1.0f)
				},

				alphaKeys = new GradientAlphaKey[] 
				{
					new GradientAlphaKey(1.0f, 0.0f),
					new GradientAlphaKey(1.0f, 1.0f)
				}
			},

			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 0.999f, 0.0f, 0.0f, 1.0f, 0.999f, DefautlColors.yellow)]
		public LSkyFloat sunMieAnisotropy = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 0.75f,
			curve        = AnimationCurve.Linear(0.0f, 0.75f, 1.0f, 0.75f),
			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 5.0f, 0.0f, 0.0f, 1.0f, 5.0f, DefautlColors.yellow)]
		public LSkyFloat sunMieScattering = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 0.5f,
			curve        = AnimationCurve.Linear(0.0f, 0.5f, 1.0f, 0.5f),
			evaluateTime = 0.0f

		};

		//----------------------------------------------------------------------------------------

		public LSkyColor moonMieColor = new LSkyColor()
		{

			colorType    = LSkyColorType.Color,
			inputColor   = new Color(0.507f, 0.695f, 1.0f, 1.0f),  

			gradient     = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(new Color(0.507f, 0.695f,  1.0f, 1.0f), 0.0f),
					new GradientColorKey(new Color(0.507f, 0.6951f, 1.0f, 1.0f), 0.5f),
					new GradientColorKey(new Color(0.507f, 0.695f,  1.0f, 1.0f), 1.0f)
				},

				alphaKeys = new GradientAlphaKey[] 
				{
					new GradientAlphaKey(1.0f, 0.0f),
					new GradientAlphaKey(1.0f, 1.0f)
				}
			},

			evaluateTime = 0.0f

		};


		[LSkyFloatAttribute(0.0f, 0.999f, 0.0f, 0.0f, 1.0f, 0.999f, DefautlColors.cyan)]
		public LSkyFloat moonMieAnisotropy = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 0.93f,
			curve        = AnimationCurve.Linear(0.0f, 0.93f, 1.0f, 0.93f),
			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 5.0f, 0.0f, 0.0f, 1.0f, 5.0f, DefautlColors.cyan)]
		public LSkyFloat moonMieScattering = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 0.5f,
			curve        = AnimationCurve.Linear(0.0f, 0.5f, 1.0f, 0.5f),
			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 5.0f, 0.0f, 0.0f, 1.0f, 5.0f, DefautlColors.cyan)]
		public LSkyFloat moonMieMultiplier = new LSkyFloat()
		{

			valueType    = LSkyValueType.Curve,
			inputValue   = 1.0f,
			curve        = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f),
			evaluateTime = 0.0f

		};

		#endregion

		#region Celestials

		public bool enableSunDisc = true;

		[LSkyFloatAttribute(0.0f, 0.5f, 0.0f, 0.0f, 1.0f, 0.5f, DefautlColors.yellow)]
		public LSkyFloat sunDiscSize = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 0.05f,
			curve        = AnimationCurve.Linear(0.0f, 0.05f, 1.0f, 0.05f),
			evaluateTime = 0.0f

		};

		public LSkyColor sunDiscColor = new LSkyColor()
		{

			colorType    = LSkyColorType.Color,
			inputColor   = Color.white,  

			gradient     = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.0f),
					new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.5f),
					new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 1.0f)
				},

				alphaKeys = new GradientAlphaKey[] 
				{
					new GradientAlphaKey(1.0f, 0.0f),
					new GradientAlphaKey(1.0f, 1.0f)
				}
			},

			evaluateTime = 0.0f

		};
		//----------------------------------------------------------------------------------------

		public bool enableMoon = true;

		[LSkyFloatAttribute(0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, DefautlColors.white)]
		public LSkyFloat moonSize = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 0.3f,
			curve        = AnimationCurve.Linear(0.0f, 0.3f, 1.0f, 0.3f),
			evaluateTime = 0.0f

		};

		public LSkyColor moonColor = new LSkyColor()
		{

			colorType    = LSkyColorType.Color,
			inputColor   = new Color(1.0f, 1.0f, 1.0f, 1.0f),  

			gradient     = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.0f),
					new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.5f),
					new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 1.0f)
				},

				alphaKeys = new GradientAlphaKey[] 
				{
					new GradientAlphaKey(1.0f, 0.0f),
					new GradientAlphaKey(1.0f, 1.0f)
				}
			},

			evaluateTime = 0.0f

		};


		[LSkyFloatAttribute(0.0f, 5.0f, 0.0f, 0.0f, 1.0f, 5.0f, DefautlColors.white)]
		public LSkyFloat moonIntensity = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 1.0f,
			curve        = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f),
			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 5.0f, 0.0f, 0.0f, 1.0f, 5.0f, DefautlColors.white)]
		public LSkyFloat moonMultiplier = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 1.0f,
			curve        = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f),
			evaluateTime = 0.0f

		};
		//----------------------------------------------------------------------------------------

		public bool enableStars = true;

		public LSkyColor starsColor = new LSkyColor()
		{

			colorType    = LSkyColorType.Gradient,
			inputColor   = new Color(1.0f, 1.0f, 1.0f, 1.0f),  

			gradient     = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.0f),
					new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.5f),
					new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 1.0f)
				},

				alphaKeys = new GradientAlphaKey[] 
				{
					new GradientAlphaKey(1.0f, 0.0f),
					new GradientAlphaKey(1.0f, 1.0f)
				}
			},

			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 10f, 0.0f, 0.0f, 1.0f, 10f, DefautlColors.white)]
		public LSkyFloat starsIntensity = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 1.0f,
			curve        = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f),
			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, DefautlColors.white)]
		public LSkyFloat starsScintillation = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 0.7f,
			curve        = AnimationCurve.Linear(0.0f, 0.7f, 1.0f, 0.7f),
			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 50f, 0.0f, 0.0f, 1.0f, 50f, DefautlColors.white)]
		public LSkyFloat starsScintillationSpeed = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 10f,
			curve        = AnimationCurve.Linear(0.0f, 10f, 1.0f, 10f),
			evaluateTime = 0.0f

		};
		//----------------------------------------------------------------------------------------

		public bool enableNebula = true;

		public LSkyColor nebulaColor = new LSkyColor()
		{

			colorType    = LSkyColorType.Gradient,
			inputColor   = new Color(1.0f, 1.0f, 1.0f, 1.0f),  

			gradient     = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.0f),
					new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.5f),
					new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 1.0f)
				},

				alphaKeys = new GradientAlphaKey[] 
				{
					new GradientAlphaKey(1.0f, 0.0f),
					new GradientAlphaKey(1.0f, 1.0f)
				}
			},

			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 10f, 0.0f, 0.0f, 1.0f, 10f, DefautlColors.mangeta)]
		public LSkyFloat nebulaIntensity = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 1.0f,
			curve        = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f),
			evaluateTime = 0.0f

		};

		public Vector3 outerSpaceOffset = Vector3.zero;

		#endregion

		#region Color Correction

		public bool HDR = false;

		[LSkyFloatAttribute(0.0f, 10f, 0.0f, 0.0f, 1.0f, 10f, DefautlColors.white)]
		public LSkyFloat exposure = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 1.3f,
			curve        = AnimationCurve.Linear(0.0f, 1.3f, 1.0f, 1.3f),
			evaluateTime = 0.0f

		};
		//--------------------------------------------------------------------------------------

		enum cSkyLiteColorSpace{ Gamma, Linear, Automatic }
		[SerializeField] private cSkyLiteColorSpace m_ColorSpace = cSkyLiteColorSpace.Automatic;

		#endregion

		#region Lighting

		public LSkyColor sunLightColor = new LSkyColor()
		{

			colorType    = LSkyColorType.Gradient,
			inputColor   = new Color(1.0f, 1.0f, 1.0f, 1.0f),  

			gradient     = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(new Color(1.0f, 0.956f, 0.839f, 1.0f), 0.0f),
					new GradientColorKey(new Color(1.0f, 0.956f, 0.839f, 1.0f), 0.25f),
					new GradientColorKey(new Color(1.0f, 0.523f, 0.264f, 1.0f), 0.50f),
					new GradientColorKey(new Color(0.0f, 0.0f, 0.0f, 0.0f), 0.55f),
					new GradientColorKey(new Color(0.0f, 0.0f, 0.0f, 0.0f), 1.0f)
				},

				alphaKeys = new GradientAlphaKey[] 
				{
					new GradientAlphaKey(1.0f, 0.0f),
					new GradientAlphaKey(1.0f, 1.0f)
				}
			},

			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 10f, 0.0f, 0.0f, 1.0f, 10f, DefautlColors.yellow)]
		public LSkyFloat sunLightIntensity = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 1.0f,
			curve        = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f),
			evaluateTime = 0.0f

		};

		public float sunLightThreshold = 0.20f;

		public LSkyColor moonLightColor = new LSkyColor()
		{

			colorType    = LSkyColorType.Color,
			inputColor   = new Color(0.632f, 0.794f, 1.0f, 1.0f),  

			gradient     = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(new Color(0.632f, 0.794f, 1.0f, 1.0f), 0.0f),
					new GradientColorKey(new Color(0.632f, 0.794f, 1.0f, 1.0f), 0.5f),
					new GradientColorKey(new Color(0.632f, 0.794f, 1.0f, 1.0f), 1.0f)
				},

				alphaKeys = new GradientAlphaKey[] 
				{
					new GradientAlphaKey(1.0f, 0.0f),
					new GradientAlphaKey(1.0f, 1.0f)
				}
			},

			evaluateTime = 0.0f

		};
		//--------------------------------------------------------------------------------------

		[LSkyFloatAttribute(0.0f, 10f, 0.0f, 0.0f, 1.0f, 10f, DefautlColors.white)]
		public LSkyFloat moonLightIntensity = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 0.3f,
			curve        = AnimationCurve.Linear(0.0f, 0.3f, 1.0f, 0.3f),
			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 5.0f, 0.0f, 0.0f, 1.0f, 5.0f, DefautlColors.white)]
		public LSkyFloat moonLightMultiplier = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 1.0f,
			curve        = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f),
			evaluateTime = 0.0f

		};
		//--------------------------------------------------------------------------------------

		public UnityEngine.Rendering.AmbientMode ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;

		public LSkyColor ambientSkyColor = new LSkyColor()
		{

			colorType    = LSkyColorType.Gradient,
			inputColor   = new Color(0.443f, 0.552f, 0.737f, 1.0f),  

			gradient     = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(new Color(0.443f, 0.552f, 0.737f, 1.0f), 0.0f),
					new GradientColorKey(new Color(0.443f, 0.552f, 0.737f, 1.0f), 0.45f),
					new GradientColorKey(new Color(0.231f, 0.290f, 0.352f, 1.0f), 0.50f),
					new GradientColorKey(new Color(0.047f, 0.094f, 0.180f, 1.0f), 0.55f),
					new GradientColorKey(new Color(0.047f, 0.094f, 0.180f, 1.0f), 1.0f)
				},

				alphaKeys = new GradientAlphaKey[] 
				{
					new GradientAlphaKey(1.0f, 0.0f),
					new GradientAlphaKey(1.0f, 1.0f)
				}
			},

			evaluateTime = 0.0f

		};

		public LSkyColor ambientEquatorColor = new LSkyColor()
		{

			colorType    = LSkyColorType.Gradient,
			inputColor   = new Color(0.901f, 0.956f, 0.968f, 1.0f),  

			gradient     = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(new Color(0.901f, 0.956f, 0.968f, 1.0f), 0.0f),
					new GradientColorKey(new Color(0.901f, 0.956f, 0.968f, 1.0f), 0.45f),
					new GradientColorKey(new Color(0.650f, 0.607f, 0.349f, 1.0f), 0.50f),
					new GradientColorKey(new Color(0.121f, 0.239f, 0.337f, 1.0f), 0.55f),
					new GradientColorKey(new Color(0.121f, 0.239f, 0.337f, 1.0f), 1.0f)
				},

				alphaKeys = new GradientAlphaKey[] 
				{
					new GradientAlphaKey(1.0f, 0.0f),
					new GradientAlphaKey(1.0f, 1.0f)
				}
			},

			evaluateTime = 0.0f

		};

		public LSkyColor ambientGroundColor = new LSkyColor()
		{

			colorType    = LSkyColorType.Gradient,
			inputColor   = new Color(0.466f, 0.435f, 0.415f, 1.0f),  

			gradient     = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(new Color(0.466f, 0.435f, 0.415f, 1.0f), 0.0f),
					new GradientColorKey(new Color(0.355f, 0.305f, 0.269f, 1.0f), 0.45f),
					new GradientColorKey(new Color(0.227f, 0.156f, 0.101f, 1.0f), 0.50f),
					new GradientColorKey(new Color(0.0f, 0.0f, 0.0f, 1.0f), 0.55f),
					new GradientColorKey(new Color(0.0f, 0.0f, 0.0f, 1.0f), 1.0f)
				},

				alphaKeys = new GradientAlphaKey[] 
				{
					new GradientAlphaKey(1.0f, 0.0f),
					new GradientAlphaKey(1.0f, 1.0f)
				}
			},

			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 8.0f, 0.0f, 0.0f, 1.0f, 8.0f, DefautlColors.white)]
		public LSkyFloat ambientIntensity = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 1.0f,
			curve        = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f),
			evaluateTime = 0.0f

		};
		//--------------------------------------------------------------------------------------

		public bool enableUnityFog = false;

		public FogMode unityFogMode = FogMode.ExponentialSquared;

		public LSkyColor unityFogColor = new LSkyColor()
		{

			colorType    = LSkyColorType.Gradient,
			inputColor   = new Color(0.901f, 0.956f, 0.968f, 1.0f),  

			gradient     = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(new Color(0.901f, 0.956f, 0.968f, 1.0f), 0.0f),
					new GradientColorKey(new Color(0.901f, 0.956f, 0.968f, 1.0f), 0.45f),
					new GradientColorKey(new Color(0.650f, 0.607f, 0.349f, 1.0f), 0.50f),
					new GradientColorKey(new Color(0.121f, 0.239f, 0.337f, 1.0f), 0.55f),
					new GradientColorKey(new Color(0.121f, 0.239f, 0.337f, 1.0f), 1.0f)
				},

				alphaKeys = new GradientAlphaKey[] 
				{
					new GradientAlphaKey(1.0f, 0.0f),
					new GradientAlphaKey(1.0f, 1.0f)
				}
			},

			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, DefautlColors.white)]
		public LSkyFloat unityFogDensity = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 0.01f,
			curve        = AnimationCurve.Linear(0.0f, 0.01f, 1.0f, 0.01f),
			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 1000f, 0.0f, 0.0f, 1.0f, 1000f, DefautlColors.white)]
		public LSkyFloat unityFogStartDistance = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 0.0f,
			curve        = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 0.0f),
			evaluateTime = 0.0f

		};

		[LSkyFloatAttribute(0.0f, 1000f, 0.0f, 0.0f, 1.0f, 1000f, DefautlColors.white)]
		public LSkyFloat unityFogEndDistance = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 300f,
			curve        = AnimationCurve.Linear(0.0f, 300f, 1.0f, 300f),
			evaluateTime = 0.0f

		};


		#endregion

		#region Get Celestials direction

		public Vector3 SunDirection { get { return -m_SunLightTransform.forward;  } }
		public Vector3 MoonDirection{ get { return -m_MoonLightTransform.forward; } }

		#endregion

		#region Curves and gradients

		/// <summary>
		/// Evaluate time by sun direction.
		/// </summary>
		/// <value>The SU n DI r HAL f EVALUAT e TIM.</value>
		public float SUN_DIR_EVALUATE_TIME{ get{ return GetEvaluateTime(SunDirection.y, true); } }  

		/// <summary>
		/// Evaluate time by sun direction only above the horizon.
		/// </summary>
		/// <value>The SU n DI r HAL f EVALUAT e TIM.</value>
		public float SUN_DIR_HALF_EVALUATE_TIME{ get{ return GetEvaluateTime(SunDirection.y, false); } } 
		public float N_SUN_DIR_HALF_EVALUATE_TIME{ get{ return GetEvaluateTime(-SunDirection.y, false); } } 
		//----------------------------------------------------------------------------------------------------

		/// <summary>
		/// Evaluate time by moon direction.
		/// </summary>
		/// <value>The MOO n DI r EVALUAT e TIM.</value>
		public float MOON_DIR_EVALUATE_TIME{ get{ return GetEvaluateTime(MoonDirection.y, true); } }  

		/// <summary>
		/// Evaluate time by moon direction only above the horizon.
		/// </summary>
		/// <value>The MOO n DI r HAL f EVALUAT e TIM.</value>
		public float MOON_DIR_HALF_EVALUATE_TIME{ get{ return GetEvaluateTime(MoonDirection.y, false); } }  
		//----------------------------------------------------------------------------------------------------

		/// <summary>
		/// Evaluate time by "Y" direction.
		/// </summary>
		/// <returns>The evaluate time.</returns>
		/// <param name="sourceDir">Source dir.</param>
		/// <param name="doubleSide">If set to <c>true</c> double side.</param>
		public float GetEvaluateTime(float sourceDir, bool fullCycle)
		{
			float val = 1.0f - sourceDir;
			return fullCycle ?  val * 0.5f : val; 
		}
			
		void SetEvaluateTime()
		{

			wavelengthR.evaluateTime         = SUN_DIR_EVALUATE_TIME;
			wavelengthG.evaluateTime         = SUN_DIR_EVALUATE_TIME;
			wavelengthB.evaluateTime         = SUN_DIR_EVALUATE_TIME;
			atmosphereThickness.evaluateTime = SUN_DIR_EVALUATE_TIME;
			//-------------------------------------------------------------------------------------------------------------

			dayAtmosphereTint.evaluateTime   = SUN_DIR_HALF_EVALUATE_TIME;
			nightAtmosphereTint.evaluateTime = moonInfluence ? MOON_DIR_HALF_EVALUATE_TIME : N_SUN_DIR_HALF_EVALUATE_TIME;
			//-------------------------------------------------------------------------------------------------------------

			sunBrightness.evaluateTime = SUN_DIR_EVALUATE_TIME;
			mie.evaluateTime           = SUN_DIR_EVALUATE_TIME;
			//-------------------------------------------------------------------------------------------------------------

			sunMieColor.evaluateTime       = SUN_DIR_HALF_EVALUATE_TIME;
			sunMieAnisotropy.evaluateTime  = SUN_DIR_HALF_EVALUATE_TIME;
			sunMieScattering.evaluateTime  = SUN_DIR_HALF_EVALUATE_TIME;
			//-------------------------------------------------------------------------------------------------------------

			moonMieColor.evaluateTime       =  MOON_DIR_HALF_EVALUATE_TIME;
			moonMieAnisotropy.evaluateTime  =  MOON_DIR_HALF_EVALUATE_TIME;
			moonMieScattering.evaluateTime  =  MOON_DIR_HALF_EVALUATE_TIME;
			moonMieMultiplier.evaluateTime  =  SUN_DIR_EVALUATE_TIME;
			//-------------------------------------------------------------------------------------------------------------

			if (enableSunDisc)
			{
				sunDiscSize.evaluateTime  = SUN_DIR_HALF_EVALUATE_TIME;
				sunDiscColor.evaluateTime = SUN_DIR_HALF_EVALUATE_TIME;
			}
			//-------------------------------------------------------------------------------------------------------------


			if(enableMoon)
			{

				moonSize.evaluateTime       = MOON_DIR_HALF_EVALUATE_TIME;
				moonColor.evaluateTime      = MOON_DIR_HALF_EVALUATE_TIME;
				moonIntensity.evaluateTime  = MOON_DIR_HALF_EVALUATE_TIME;
				moonMultiplier.evaluateTime = SUN_DIR_EVALUATE_TIME;
			}
			//-------------------------------------------------------------------------------------------------------------

			if(enableStars) 
			{
				starsColor.evaluateTime         = SUN_DIR_EVALUATE_TIME;
				starsIntensity.evaluateTime     = SUN_DIR_EVALUATE_TIME;
				starsScintillation.evaluateTime = SUN_DIR_EVALUATE_TIME;
			}


			if(enableNebula) 
			{
				nebulaColor.evaluateTime      = SUN_DIR_EVALUATE_TIME;
				nebulaIntensity.evaluateTime  = SUN_DIR_EVALUATE_TIME;
			}
			//-------------------------------------------------------------------------------------------------------------

			exposure.evaluateTime = SUN_DIR_EVALUATE_TIME;
			//-------------------------------------------------------------------------------------------------------------

			sunLightColor.evaluateTime     = SUN_DIR_EVALUATE_TIME;
			sunLightIntensity.evaluateTime = SUN_DIR_EVALUATE_TIME;
			//-------------------------------------------------------------------------------------------------------------

			moonLightColor.evaluateTime      = MOON_DIR_HALF_EVALUATE_TIME;
			moonLightIntensity.evaluateTime  = MOON_DIR_HALF_EVALUATE_TIME;
			moonLightMultiplier.evaluateTime = SUN_DIR_EVALUATE_TIME;
			//-------------------------------------------------------------------------------------------------------------

			// Ambient
			switch (ambientMode) 
			{
				case UnityEngine.Rendering.AmbientMode.Skybox:
				ambientIntensity.evaluateTime = SUN_DIR_EVALUATE_TIME;
				break;

				case UnityEngine.Rendering.AmbientMode.Trilight: 
				ambientSkyColor.evaluateTime     = SUN_DIR_EVALUATE_TIME;
				ambientEquatorColor.evaluateTime = SUN_DIR_EVALUATE_TIME;
				break;

				case UnityEngine.Rendering.AmbientMode.Flat :
				ambientSkyColor.evaluateTime   = SUN_DIR_EVALUATE_TIME;
				break;
			}

			ambientGroundColor.evaluateTime = SUN_DIR_EVALUATE_TIME;
			//-------------------------------------------------------------------------------------------------------------

			if (enableUnityFog)
			{
				
				unityFogColor.evaluateTime = SUN_DIR_EVALUATE_TIME;

				if (unityFogMode == FogMode.Linear)
				{
					unityFogStartDistance.evaluateTime = SUN_DIR_EVALUATE_TIME;
					unityFogEndDistance.evaluateTime = SUN_DIR_EVALUATE_TIME;
				} else {
					unityFogDensity.evaluateTime = SUN_DIR_EVALUATE_TIME;
				}

			}
			//-------------------------------------------------------------------------------------------------------------
		}

		#endregion


		// Editor foldouts.
		#if UNITY_EDITOR

		[HideInInspector]
		public bool CRFoldout  = true,
		atmosphereFoldout      = true,
		celestialsFoldout      = true,
		colorCorrectionFoldout = true,
		lightingFoldout        = true;

		#endif

	}
}
