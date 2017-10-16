

Shader "AC/LSky/Skybox Reflection"
{

	Properties
	{
		_GroundFade("Ground Fade", Range(0,60)) = 30
		_GroundAltitude("Ground Altitude", Range(0, 0.1)) = 0.01

	}

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

			half _GroundFade;
			half _GroundAltitude;
			//-------------------------------------------------------

			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID 
			};

			struct v2f
			{
				float3 worldPos : TEXCOORD0;
				half3  color    : TEXCOORD1;
				float4 vertex   : SV_POSITION;
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

				half3 inscatter; half4 outscatter;

				AtmosphericScattering(o.worldPos, inscatter, outscatter, true);
				//------------------------------------------------------------------------------

				half3 color = inscatter;

				ColorCorrection(color, LSky_GroundColor);

				color = lerp(color, LSky_GroundColor, saturate((-o.worldPos.y - _GroundAltitude) * _GroundFade));

				o.color = color;

				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				return half4(i.color, 1);
			}
			ENDCG
		}

	}
}
