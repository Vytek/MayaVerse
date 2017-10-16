
#ifndef LSKY_VARIABLES_INCLUDED
#define LSKY_VARIABLES_INCLUDED

/////////////////////////
/// Global variables. ///
/////////////////////////

// Celestials directions.
uniform float3 LSky_SunDir;
uniform float3 LSky_MoonDir;
//---------------------------------------

// Defautl atmosphere.
uniform float  LSky_kCameraHeight;
uniform float  LSky_kInnerRadius;
uniform float  LSky_kInnerRadius2;
uniform float  LSky_kOuterRadius;
uniform float  LSky_kOuterRadius2;
uniform float  LSky_kScale;
uniform float  LSky_kScaleOverScaleDepth;
uniform float  LSky_kKmESun;
uniform float  LSky_kKm4PI;
uniform float  LSky_kKrESun;
uniform float  LSky_kKr4PI;
uniform float3 LSky_InvWavelength;
//---------------------------------------

// Atmosphere tint.
uniform half3 LSky_DayAtmosphereTint;
uniform half3 LSky_NightAtmosphereTint;
//---------------------------------------

// Sun.
uniform float3 LSky_SunBetaMiePhase;
uniform half3  LSky_SunMieColor;
uniform half   LSky_SunMieScattering;
//---------------------------------------

// Moon.
uniform float3 LSky_MoonBetaMiePhase;
uniform half3  LSky_MoonMieColor;
uniform half   LSky_MoonMieScattering;
//---------------------------------------

// Matrices.
uniform float4x4 LSky_SunMatrix;
uniform float4x4 LSky_MoonMatrix;
//---------------------------------------

// Reflection.
uniform half3 LSky_GroundColor;
//---------------------------------------

// HDR.
uniform half LSky_Exposure;
//---------------------------------------


#endif