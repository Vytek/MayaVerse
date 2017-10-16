

Shader "AC/LSky/Skybox Atmosphere"
{

	Properties{}

	SubShader
	{

		Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
		Cull Off ZWrite Off

		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//-------------------------------------------------------

			#pragma multi_compile __ LSKY_MOON_INFLUENCE
			#pragma multi_compile __ LSKY_NIGHT_COLOR_ATMOSPHERIC
			//-------------------------------------------------------

			#pragma multi_compile __ LSKY_ENABLE_SUN_DISC
			#pragma multi_compile __ LSKY_ENABLE_MOON
			//-------------------------------------------------------

			#pragma multi_compile __ LSKY_ENABLE_STARS
			#pragma multi_compile __ LSKY_ENABLE_NEBULA
			//-------------------------------------------------------

			#pragma multi_compile __ LSKY_HDR
			#pragma multi_compile __ LSKY_GAMMA_COLOR_SPACE
			//-------------------------------------------------------

			#pragma target 3.0
			//-------------------------------------------------------

			#include "UnityCG.cginc"
			#include "LSkyVariablesInc.cginc"  
			#include "LSkyInc.cginc"  
			#include "LSkyAtmosphereInc.cginc"  
			//-------------------------------------------------------

			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID 
			};

			struct v2f
			{
				float3 worldPos             : TEXCOORD0;
				half3  inScatter            : TEXCOORD1;
				half4  outScatter           : TEXCOORD2;

				float4 vertex               : SV_POSITION;

				UNITY_VERTEX_OUTPUT_STEREO 
			};


			v2f vert (appdata v)
			{
				v2f o;

			
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				//------------------------------------------------------------------------------

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = normalize(mul((float3x3)unity_ObjectToWorld, v.vertex.xyz));
				//------------------------------------------------------------------------------

				AtmosphericScattering(o.worldPos, o.inScatter, o.outScatter, true);
				//------------------------------------------------------------------------------


				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{

				float3 ray = normalize(i.worldPos);
				//----------------------------------------------------------

				float  sunCosTheta  = dot(ray, LSky_SunDir.xyz);
				float  moonCosTheta = dot(ray, LSky_MoonDir.xyz); 
				//-------------------------------------------------------- --

			
				half3 color = i.inScatter;

				//if(ray.y> 0.0)
				//{
					// Sun
					color += MiePhase(sunCosTheta, LSky_SunBetaMiePhase, LSky_SunMieScattering,  LSky_SunMieColor) * i.outScatter.rgb;


					// Moon.
					color.rgb  += MiePhaseSimplified(moonCosTheta, LSky_MoonBetaMiePhase, LSky_MoonMieScattering,  LSky_MoonMieColor) * i.outScatter.a;

				//}
				ColorCorrection(color, LSky_GroundColor);

			//	color = lerp(color, LSky_GroundColor, saturate((-ray.y - 0.01) * 30));

				return half4(color,1);
			}
			ENDCG
		}

	}
}
