
////////////
/// LSky ///
////////////


using System;
using UnityEngine;


namespace AC.LSky
{
	[AddComponentMenu("AC/LSky/LSky Manager")]
	[ExecuteInEditMode] public partial class LSky : MonoBehaviour 
	{

	

		#region Unity

		void Awake()
		{

			if(!IsReady && Application.isPlaying) 
			{
				CacheComponents();
			}
		}

		void Update()
		{

			if(!IsReady) 
			{
				if(!Application.isPlaying)
					CacheComponents();
				
				return;
			}

			SetEvaluateTime();
			Sun();
			Moon();
			OuterSpace();
			Atmosphere();
			ColorCorrection();
			Lighting();
		}


		#endregion

		#region Celestials

		void Sun()
		{

			Shader.SetGlobalMatrix("LSky_SunMatrix", m_SunLightTransform.worldToLocalMatrix); 
			Shader.SetGlobalVector("LSky_SunDir", SunDirection);

			if (!enableSunDisc)
			{
				skyboxMaterial.DisableKeyword("LSKY_ENABLE_SUN_DISC");
				return;
			}

			skyboxMaterial.EnableKeyword("LSKY_ENABLE_SUN_DISC");
			skyboxMaterial.SetColor("_SunDiscColor", sunDiscColor.OutputColor);
			skyboxMaterial.SetFloat("_SunDiscSize", sunDiscSize.OutputValue);
		}
		//-----------------------------------------------------------------------------------------------------------------------------

		void Moon()
		{

			Shader.SetGlobalVector("LSky_MoonDir", MoonDirection);
			Shader.SetGlobalMatrix("LSky_MoonMatrix", m_MoonLightTransform.worldToLocalMatrix);

			if(!enableMoon) 
			{
				skyboxMaterial.DisableKeyword("LSKY_ENABLE_MOON");
				return;
			}

			skyboxMaterial.EnableKeyword("LSKY_ENABLE_MOON");
			skyboxMaterial.SetTexture("_MoonTexture", moonTexture);
			skyboxMaterial.SetFloat("_MoonSize", moonSize.OutputValue);
			skyboxMaterial.SetColor("_MoonColor", moonColor.OutputColor);
			skyboxMaterial.SetFloat("_MoonIntensity", moonIntensity.OutputValue * moonMultiplier.OutputValue);

		}
		//-----------------------------------------------------------------------------------------------------------------------------

		public Matrix4x4 outerSpaceMatrix{ get{ return Matrix4x4.TRS (Vector3.zero, Quaternion.Euler(outerSpaceOffset), Vector3.one); } }

		float SNSC;
		void OuterSpace()
		{

			if(enableStars || enableNebula)
			{
				skyboxMaterial.SetTexture("_OuterSpaceCube", outerSpaceCube);
				skyboxMaterial.SetMatrix("_OuterSpaceMatrix", outerSpaceMatrix);
			}

			if(!enableStars) 
			{
				skyboxMaterial.DisableKeyword ("LSKY_ENABLE_STARS");
			} 
			else 
			{
				
				// Get stars twinkle speed.
				SNSC += Time.deltaTime * starsScintillationSpeed.OutputValue; 

				// Get noise matrix.
				Matrix4x4 starsNoiseMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler (SNSC, 0, 0), Vector3.one); 	
				//--------------------------------------------------------------------------------------------------------

				skyboxMaterial.EnableKeyword("LSKY_ENABLE_STARS");
				skyboxMaterial.SetTexture("_StarsNoiseCube", starsNoiseCube);
				skyboxMaterial.SetMatrix("_StarsNoiseMatrix", starsNoiseMatrix);
				skyboxMaterial.SetColor("_StarsColor", starsColor.OutputColor);
				skyboxMaterial.SetFloat("_StarsIntensity", starsIntensity.OutputValue);
				skyboxMaterial.SetFloat("_StarsScintillation", starsScintillation.OutputValue);
			}

			if(!enableNebula) 
			{
				skyboxMaterial.DisableKeyword("LSKY_ENABLE_NEBULA");
				return;
			}

			skyboxMaterial.EnableKeyword("LSKY_ENABLE_NEBULA");
			skyboxMaterial.SetColor("_NebulaColor", nebulaColor.OutputColor);
			skyboxMaterial.SetFloat("_NebulaIntensity", nebulaIntensity.OutputValue);

		}
		//-----------------------------------------------------------------------------------------------------------------------------

		#endregion

		#region Atmosphere


		void Atmosphere()
		{

			// Atmosphere based in GPUGems2(Sean Oneil).

			// Inverse wave legths (reciprocal).
			Vector3 InvWavelength = new Vector3 () 
			{

				x = 1.0f / Mathf.Pow(wavelengthR.OutputValue * 1e-3f, 4.0f),
				y = 1.0f / Mathf.Pow(wavelengthG.OutputValue * 1e-3f, 4.0f),
				z = 1.0f / Mathf.Pow(wavelengthB.OutputValue * 1e-3f, 4.0f)
			};
			//-----------------------------------------------------------------------------------------------------------------------------

			float kCameraHeight = 0.0001f;
			float kInnerRadius  = 1.0f;
			float kInnerRadius2 = 1.0f;
			float kOuterRadius  = 1.025f;
			float kOuterRadius2 = kOuterRadius * kOuterRadius;
			//-----------------------------------------------------------------------------------------------------------------------------

			float kScale               = (1.0f / (kOuterRadius - 1.0f));
			float kScaleDepth          = 0.25f;
			float kScaleOverScaleDepth = kScale / kScaleDepth;
			//-----------------------------------------------------------------------------------------------------------------------------

			float kSunBrightness = sunBrightness.OutputValue;
			float kMie           = mie.OutputValue;
			float kKmESun        = kMie * kSunBrightness;
			float kKm4PI         = kMie * 4.0f * Mathf.PI;
			//-----------------------------------------------------------------------------------------------------------------------------

			float kRayleigh      = 0.0025f * atmosphereThickness.OutputValue;
			float kKrESun        = kRayleigh * kSunBrightness;
			float kKr4PI         = kRayleigh * 4.0f * Mathf.PI;
			//-----------------------------------------------------------------------------------------------------------------------------


			Shader.SetGlobalFloat("LSky_kCameraHeight", kCameraHeight);
			Shader.SetGlobalFloat("LSky_kInnerRadius",  kInnerRadius);
			Shader.SetGlobalFloat("LSky_kInnerRadius2", kInnerRadius2);
			Shader.SetGlobalFloat("LSky_kOuterRadius",  kOuterRadius);
			Shader.SetGlobalFloat("LSky_kOuterRadius2", kOuterRadius2);
			//-----------------------------------------------------------------------------------------------------------------------------

			Shader.SetGlobalFloat("LSky_kScale",               kScale);
			Shader.SetGlobalFloat("LSky_kScaleDepth",          kScaleDepth);
			Shader.SetGlobalFloat("LSky_kScaleOverScaleDepth", kScaleOverScaleDepth);
			//-----------------------------------------------------------------------------------------------------------------------------

			Shader.SetGlobalFloat("LSky_kKm4PI",  kKm4PI);
			Shader.SetGlobalFloat("LSky_kKmESun", kKmESun);
			Shader.SetGlobalFloat("LSky_kKrESun", kKrESun);
			Shader.SetGlobalFloat("LSky_kKr4PI",  kKr4PI);
			//-----------------------------------------------------------------------------------------------------------------------------

			Shader.SetGlobalVector("LSky_InvWavelength", InvWavelength);
			//-----------------------------------------------------------------------------------------------------------------------------


			Shader.SetGlobalColor("LSky_DayAtmosphereTint", dayAtmosphereTint.OutputColor);
			Shader.SetGlobalVector("LSky_SunBetaMiePhase", SunBetaMiePhase(sunMieAnisotropy.OutputValue));
			Shader.SetGlobalFloat("LSky_SunMieScattering", sunMieScattering.OutputValue);
			Shader.SetGlobalColor("LSky_SunMieColor", sunMieColor.OutputColor);
			//-----------------------------------------------------------------------------------------------------------------------------

			if(nightColorType == LSkyNightColorType.Atmospheric)
			{
				Shader.EnableKeyword("LSKY_NIGHT_COLOR_ATMOSPHERIC"); 
				Shader.DisableKeyword("LSKY_NIGHT_COLOR_SIMPLE"); 
			}
			else
			{
				Shader.EnableKeyword("LSKY_NIGHT_COLOR_SIMPLE"); 
				Shader.DisableKeyword("LSKY_NIGHT_COLOR_ATMOSPHERIC"); 
			}
		
			if(moonInfluence)
				Shader.EnableKeyword("LSKY_MOON_INFLUENCE"); 
			else 
				Shader.DisableKeyword("LSKY_MOON_INFLUENCE"); 
			
			Shader.SetGlobalColor("LSky_NightAtmosphereTint", nightAtmosphereTint.OutputColor);
			//-----------------------------------------------------------------------------------------------------------------------------

			Shader.SetGlobalVector("LSky_MoonBetaMiePhase", MoonBetaMiePhase(moonMieAnisotropy.OutputValue));
			Shader.SetGlobalFloat("LSky_MoonMieScattering", moonMieScattering.OutputValue * moonMieMultiplier.OutputValue);
			Shader.SetGlobalColor("LSky_MoonMieColor", moonMieColor.OutputColor);

		}
		//-----------------------------------------------------------------------------------------------------------------------------

		// Henyey Greenstein beta phase(Cornette Sharks).
		public Vector3 SunBetaMiePhase(float g) 
		{

			float g2 = g * g;
			return new Vector3() 
			{
				x = (1.0f - g2) / (2.0f + g2),
				y = (1.0f + g2),
				z = (2.0f * g) 
			};
		}

		// Henyey Greenstein beta mie phase.
		public Vector3 MoonBetaMiePhase(float g) 
		{

			float g2 = g * g;
			return new Vector3() 
			{
				x = (1.0f - g2),
				y = (1.0f + g2),
				z = (2.0f * g) 
			};
		}
		//-----------------------------------------------------------------------------------------------------------------------------

		#endregion

		#region Color Correction

		void ColorCorrection()
		{

			Shader.SetGlobalFloat("LSky_Exposure", exposure.OutputValue);

			if(!HDR) Shader.DisableKeyword("LSKY_HDR"); 
			else     Shader.EnableKeyword("LSKY_HDR");
			//------------------------------------------------------------------

			switch(m_ColorSpace)
			{

				case cSkyLiteColorSpace.Gamma:
				Shader.EnableKeyword("LSKY_GAMMA_COLOR_SPACE");
				break;

				case cSkyLiteColorSpace.Linear:
				Shader.DisableKeyword("LSKY_GAMMA_COLOR_SPACE");
				break;

				case cSkyLiteColorSpace.Automatic:
				if(QualitySettings.activeColorSpace == ColorSpace.Gamma)
					Shader.EnableKeyword("LSKY_GAMMA_COLOR_SPACE");
				else
					Shader.DisableKeyword("LSKY_GAMMA_COLOR_SPACE");
				break;
			}
			//------------------------------------------------------------------
		}

		#endregion

		#region Lighting


		private bool m_SunLightEnable;
		void Lighting()
		{

			m_SunLightEnable = !CheckDirLightEnable(Mathf.Max(0.0f, Mathf.Min(1.0f, SunDirection.y + 0.30f)), sunLightThreshold);

			m_SunLight.color     = sunLightColor.OutputColor;
			m_SunLight.intensity = sunLightIntensity.OutputValue;
			m_SunLight.enabled   = m_SunLightEnable;
			//---------------------------------------------------------------------------------------------------------------------------

			m_MoonLight.color     = moonLightColor.OutputColor;
			m_MoonLight.intensity = moonLightIntensity.OutputValue * moonLightMultiplier.OutputValue;
			m_MoonLight.enabled   = !m_SunLight.enabled;
			//---------------------------------------------------------------------------------------------------------------------------

			if(applySkybox)
				RenderSettings.skybox = skyboxMaterial;


			RenderSettings.ambientMode = ambientMode;
			switch (ambientMode) 
			{

				case UnityEngine.Rendering.AmbientMode.Skybox:
				RenderSettings.ambientIntensity = ambientIntensity.OutputValue;
				break;

				case UnityEngine.Rendering.AmbientMode.Trilight: 
				RenderSettings.ambientSkyColor     = ambientSkyColor.OutputColor;
				RenderSettings.ambientEquatorColor = ambientEquatorColor.OutputColor;
				break;

				case UnityEngine.Rendering.AmbientMode.Flat :
				RenderSettings.ambientSkyColor     = ambientSkyColor.OutputColor;
				break;

			}

			RenderSettings.ambientGroundColor   = ambientGroundColor.OutputColor;
			Shader.SetGlobalColor("LSky_GroundColor", ambientGroundColor.OutputColor);
			//---------------------------------------------------------------------------------------------------------------------------

			RenderSettings.fog = enableUnityFog;
			if(enableUnityFog)
			{
				RenderSettings.fogMode  = unityFogMode;
				RenderSettings.fogColor = unityFogColor.OutputColor;

				if (unityFogMode == FogMode.Linear)
				{
					RenderSettings.fogStartDistance = unityFogStartDistance.OutputValue;
					RenderSettings.fogEndDistance   = unityFogEndDistance.OutputValue;
				}
				else
					RenderSettings.fogDensity = unityFogDensity.OutputValue;

			}
		}

		bool CheckDirLightEnable(float theta, float threshold = 0.20f)
		{
			return(Mathf.Abs(theta) < threshold) ? true : false;
		}

		#endregion



		#region Day States

		public bool IsDay{ get{ return m_SunLightEnable; } }
		public bool IsNight{ get{ return !IsDay; }}

		#endregion


	}
}
