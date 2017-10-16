Shader "Unlit/ForkParticle"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _SrcBlend  ("__src", Float) = 0.000000
        _DstBlend ("__dst", Float) = 0.000000
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType"="Transparent"  }
        
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
                float4 _color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
                float4 _color : COLOR;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex =  mul(UNITY_MATRIX_VP, v.vertex);
                o._color = v._color;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * i._color;
				return col;
			}
            
			ENDCG
            ZWrite Off
            cull off
            Blend [_SrcBlend] [_DstBlend]
            Lighting Off
            
            
		}
	}
}
