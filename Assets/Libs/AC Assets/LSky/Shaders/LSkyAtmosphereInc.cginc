
#ifndef LSKY_ATMOSPHERE_INCLUDED
#define LSKY_ATMOSPHERE_INCLUDED

//////////////////////////////
/// Atmospheric Scattering ///
//////////////////////////////

#include "LSkyVariablesInc.cginc"  
#include "LSkyInc.cginc"

float RayleighPhase(float cosTheta)
{
	return 0.75 + 0.75 * (1.0 + cosTheta * cosTheta);
}

float Scale(float Cos)
{
	float x = 1.0 - Cos;
	return 0.25 * exp( -0.00287 + x * (0.459 + x * (3.83 + x * (-6.80 + x * 5.25))) );
}
//----------------------------------------------------------------------------------------------


inline void AtmosphericScattering(float3 ray, out half3 inScatter, out half4 outScatter, bool clampScatter)
{

	ray.y = max(0.0, ray.y); // fix downside.

	float3 cameraPos = float3(0.0, LSky_kInnerRadius + LSky_kCameraHeight, 0.0); 
	float  far       = sqrt(LSky_kOuterRadius2 + LSky_kInnerRadius2 * ray.y * ray.y - LSky_kInnerRadius2) - LSky_kInnerRadius * ray.y;
	float3 pos       = cameraPos + far * ray;
	//-------------------------------------------------------------------------------------------------------------------------------------

//	float startDepth  = exp(kScaleOverScaleDepth * (kInnerRadius - kCameraHeight));
	float startDepth  = exp(LSky_kScaleOverScaleDepth * (-LSky_kCameraHeight));  
	float startHeight = LSky_kInnerRadius + LSky_kCameraHeight;
	float startAngle  = dot(ray, cameraPos) / startHeight;
	float startOffset = startDepth * Scale(startAngle);
	//-------------------------------------------------------------------------------------------------------------------------------------

	const float kSamples = 2;

	float  sampleLength = far / kSamples;
	float  scaledLength = sampleLength * LSky_kScale;
	float3 sampleRay    = ray * sampleLength;
	float3 samplePoint  = cameraPos + sampleRay * 0.5;
	//-------------------------------------------------------------------------------------------------------------------------------------


	float3 frontColor = 0.0; float4 outColor = 0.0; 

	for(int i = 0; i < int(kSamples); i++)
	{

		float height    = length(samplePoint);
		float invHeight = 1.0 / height; // reciprocal.
		//---------------------------------------------------------------------------------------------------------------------------------

		float  depth       = exp(LSky_kScaleOverScaleDepth * (LSky_kInnerRadius - height));
		float  lightAngle  = dot(LSky_SunDir.xyz, samplePoint) * invHeight; 
		float  cameraAngle = dot(ray, samplePoint) * invHeight;
		float3 betaAtten   = (LSky_InvWavelength * LSky_kKr4PI) + LSky_kKm4PI;
		//---------------------------------------------------------------------------------------------------------------------------------

		float  scatter     = startOffset + depth * ( Scale(lightAngle) - Scale(cameraAngle) );

		float3 attenuate   = clampScatter ? exp(-clamp(scatter,0.0, 50) * betaAtten) : exp(-scatter * betaAtten);

		float3 dayColor    = attenuate * (depth * scaledLength) * LSky_DayAtmosphereTint;
		//---------------------------------------------------------------------------------------------------------------------------------


		float3 nightColor = 0;

		#ifdef LSKY_NIGHT_COLOR_ATMOSPHERIC

		float nightLightAngle = 0.0;

		#ifdef LSKY_MOON_INFLUENCE
		nightLightAngle =  dot(LSky_MoonDir.xyz, samplePoint) * invHeight; 
		#else
		nightLightAngle = -lightAngle; 
		#endif
		//---------------------------------------------------------------------------------------------------------------------------------
		float  nightScatter   = startOffset + depth * (Scale(nightLightAngle) - Scale(cameraAngle));
		float3 nightAttenuate = clampScatter ? exp(-clamp(nightScatter,0.0, 50) * betaAtten ) : exp(-nightScatter * betaAtten);
		//---------------------------------------------------------------------------------------------------------------------------------

		nightColor  = (nightAttenuate * (depth * scaledLength)) * LSky_NightAtmosphereTint;
		frontColor += nightColor;

		#endif
		//---------------------------------------------------------------------------------------------------------------------------------

		frontColor   += dayColor; 
		outColor.rgb += dayColor; 
		outColor.a   += exp(-(startOffset + depth * (Scale(half3(0,1,0)) - Scale(cameraAngle))) * betaAtten);
		//---------------------------------------------------------------------------------------------------------------------------------

		samplePoint  += sampleRay;
		//---------------------------------------------------------------------------------------------------------------------------------

	}

	float cosTheta  = dot(ray, LSky_SunDir.xyz);
	inScatter       =((frontColor * (LSky_InvWavelength * LSky_kKrESun))) * RayleighPhase(cosTheta);
	//-------------------------------------------------------------------------------------------------------------------------------------

	#ifndef LSKY_NIGHT_COLOR_ATMOSPHERIC
	inScatter +=(saturate(1-dot(ray.y, 0.5)) )* LSky_NightAtmosphereTint;
	#endif
	//-------------------------------------------------------------------------------------------------------------------------------------

	outScatter.rgb  = outColor * LSky_kKmESun;
	outScatter.a    = (Desaturate(outColor.a));
	//-------------------------------------------------------------------------------------------------------------------------------------
}


//---------------------------------------------------------------------------------------------------------------------------------------------------

// Original Henyey Greenstein phase function with small changes.
/*inline half3 MiePhase(float cosTheta, float3 betaMiePhase, half scattering, half3 color)
{
	return (LSky_PI14 * (betaMiePhase.x * pow(betaMiePhase.y - (betaMiePhase.z * cosTheta), -1.5))) * scattering * color;
}*/

// Simplified Henyey Greenstein phase function for moon.
inline half3 MiePhaseSimplified(float cosTheta, float3 betaMiePhase, half scattering, half3 color)
{
	return (LSky_PI14 * (betaMiePhase.x / (betaMiePhase.y - (betaMiePhase.z * cosTheta)))) * scattering *  color;
}

// Cornette Sharks Henyey Greenstein phase function with small changes.
inline half3 MiePhase(float cosTheta, float3 betaMiePhase, half scattering,  half3 color)
{
	return (1.5 * betaMiePhase.x * ((1.0 + cosTheta*cosTheta) * pow(betaMiePhase.y - (betaMiePhase.z * cosTheta), -1.5))) * scattering * color;
}
//---------------------------------------------------------------------------------------------------------------------------------------------------
#endif