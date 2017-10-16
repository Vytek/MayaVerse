//================================================================================================================
// This is provided by Unity Technologies as a solution to incorrect shadow when using custom projection matrix
// The engine side solution will not be ported back to 5.4, so for Unity 5.4 users this is nessasery to workaround the issue.
// Download Link: http://files.unity3d.com/florent/Fixes/FixFor686520_ScreenSpaceShadow.unitypackage 
//================================================================================================================

// Collects cascaded shadows into screen space buffer
Shader "ScreenSpaceShadows-ForCustomPerspectiveMat" {
Properties {
	_ShadowMapTexture ("", any) = "" {}
}

//Fix for case 686520 ++ : Shadow moves in relation to the object that casts it when using a custom projection matrix
//see http://issuetracker.unity3d.com/issues/shadow-moves-in-relation-to-the-object-that-casts-it-when-using-a-custom-projection-matrix

CGINCLUDE
#include "UnityCG.cginc"

// Configuration


// Should receiver plane bias be used? This estimates receiver slope using derivatives,
// and tries to tilt the PCF kernel along it. However, since we're doing it in screenspace
// from the depth texture, the derivatives are wrong on edges or intersections of objects,
// leading to possible shadow artifacts. So it's disabled by default.
#define UNITY_USE_RECEIVER_PLANE_BIAS 0
#define UNITY_RECEIVER_PLANE_MIN_FRACTIONAL_ERROR 0.05f


// Blend between shadow cascades to hide the transition seams?
#define UNITY_USE_CASCADE_BLENDING 0
#define UNITY_CASCADE_BLEND_DISTANCE 0.1


struct appdata {
	float4 vertex : POSITION;
	float2 texcoord : TEXCOORD0;
	float3 normal : NORMAL;
};

struct v2f {
	float2 uv : TEXCOORD0;

	//Fix for case 686520 ++
	    //// View space ray, for perspective case
	    //float3 ray : TEXCOORD1;
	    //// Orthographic view space positions (need xy as well for oblique matrices)
        //float3 orthoPosNear : TEXCOORD2;
    
	    float2 screenPos : TEXCOORD1;
	//Fix for case 686520 --

	float4 pos : SV_POSITION;
};

v2f vert (appdata v)
{
	v2f o;
	o.uv = v.texcoord;
	//Fix for case 686520 ++
		//o.ray = v.normal;
	//Fix for case 686520 --
	float4 clipPos = UnityObjectToClipPos(v.vertex);
	o.pos = clipPos;

	//Fix for case 686520 ++
	    // To compute view space position from Z buffer for oblique perspective case
	    // we can't rely on the implicit far value to compute the linear Z thus we use
	    // unity_CameraInvProjection at the PS level.
	    o.screenPos.xy = ComputeScreenPos(clipPos);
	//Fix for case 686520 --

	return o;
}

sampler2D_float _CameraDepthTexture;
// sizes of cascade projections, relative to first one
float4 unity_ShadowCascadeScales;

UNITY_DECLARE_SHADOWMAP(_ShadowMapTexture);
float4 _ShadowMapTexture_TexelSize;

//
// Keywords based defines
//
#if defined (SHADOWS_SPLIT_SPHERES)
	#define GET_CASCADE_WEIGHTS(wpos, z)    getCascadeWeights_splitSpheres(wpos)
	#define GET_SHADOW_FADE(wpos, z)		getShadowFade_SplitSpheres(wpos)
#else
	#define GET_CASCADE_WEIGHTS(wpos, z)	getCascadeWeights( wpos, z )
	#define GET_SHADOW_FADE(wpos, z)		getShadowFade(z)
#endif

#if defined (SHADOWS_SINGLE_CASCADE)
	#define GET_SHADOW_COORDINATES(wpos,cascadeWeights)	getShadowCoord_SingleCascade(wpos)
#else
	#define GET_SHADOW_COORDINATES(wpos,cascadeWeights)	getShadowCoord(wpos,cascadeWeights)
#endif

// prototypes 

//Fix for case 686520 ++
	inline float3 computeCameraSpacePosFromDepth(v2f i);
//Fix for case 686520 --

inline fixed4 getCascadeWeights(float3 wpos, float z);		// calculates the cascade weights based on the world position of the fragment and plane positions
inline fixed4 getCascadeWeights_splitSpheres(float3 wpos);	// calculates the cascade weights based on world pos and split spheres positions
inline float  getShadowFade_SplitSpheres( float3 wpos );	
inline float  getShadowFade( float3 wpos, float z );
inline float4 getShadowCoord_SingleCascade( float4 wpos );	// converts the shadow coordinates for shadow map using the world position of fragment (optimized for single fragment)
inline float4 getShadowCoord( float4 wpos, fixed4 cascadeWeights );// converts the shadow coordinates for shadow map using the world position of fragment
half 		  sampleShadowmap_PCF5x5 (float4 coord);		// samples the shadowmap based on PCF filtering (5x5 kernel)
half 		  unity_sampleShadowmap( float4 coord );		// sample shadowmap SM2.0+

/**
 * Gets the cascade weights based on the world position of the fragment.
 * Returns a float4 with only one component set that corresponds to the appropriate cascade.
 */
inline fixed4 getCascadeWeights(float3 wpos, float z)
{
	fixed4 zNear = float4( z >= _LightSplitsNear );
	fixed4 zFar = float4( z < _LightSplitsFar );
	fixed4 weights = zNear * zFar;
	return weights;
}

/**
 * Gets the cascade weights based on the world position of the fragment and the poisitions of the split spheres for each cascade.
 * Returns a float4 with only one component set that corresponds to the appropriate cascade.
 */
inline fixed4 getCascadeWeights_splitSpheres(float3 wpos)
{
	float3 fromCenter0 = wpos.xyz - unity_ShadowSplitSpheres[0].xyz;
	float3 fromCenter1 = wpos.xyz - unity_ShadowSplitSpheres[1].xyz;
	float3 fromCenter2 = wpos.xyz - unity_ShadowSplitSpheres[2].xyz;
	float3 fromCenter3 = wpos.xyz - unity_ShadowSplitSpheres[3].xyz;
	float4 distances2 = float4(dot(fromCenter0,fromCenter0), dot(fromCenter1,fromCenter1), dot(fromCenter2,fromCenter2), dot(fromCenter3,fromCenter3));
	fixed4 weights = float4(distances2 < unity_ShadowSplitSqRadii);
	weights.yzw = saturate(weights.yzw - weights.xyz);
	return weights;
}

/**
 * Returns the shadow fade based on the 'z' position of the fragment
 */
inline float getShadowFade( float z )
{
	return saturate(z * _LightShadowData.z + _LightShadowData.w);
}

/**
 * Returns the shadow fade based on the world position of the fragment, and the distance from the shadow fade center
 */
inline float getShadowFade_SplitSpheres( float3 wpos )
{	
	float sphereDist = distance(wpos.xyz, unity_ShadowFadeCenterAndType.xyz);
	half shadowFade = saturate(sphereDist * _LightShadowData.z + _LightShadowData.w);
	return shadowFade;	
}

/**
 * Returns the shadowmap coordinates for the given fragment based on the world position and z-depth.
 * These coordinates belong to the shadowmap atlas that contains the maps for all cascades.
 */
inline float4 getShadowCoord( float4 wpos, fixed4 cascadeWeights )
{
	float3 sc0 = mul (unity_WorldToShadow[0], wpos).xyz;
	float3 sc1 = mul (unity_WorldToShadow[1], wpos).xyz;
	float3 sc2 = mul (unity_WorldToShadow[2], wpos).xyz;
	float3 sc3 = mul (unity_WorldToShadow[3], wpos).xyz;
	return float4(sc0 * cascadeWeights[0] + sc1 * cascadeWeights[1] + sc2 * cascadeWeights[2] + sc3 * cascadeWeights[3], 1);
}

/**
 * Same as the getShadowCoord; but optimized for single cascade
 */
inline float4 getShadowCoord_SingleCascade( float4 wpos )
{
	return float4( mul (unity_WorldToShadow[0], wpos).xyz, 0);
}

/**
 * Computes the receiver plane depth bias for the given shadow coord in screen space.
 * Inspirations: 
 *		http://mynameismjp.wordpress.com/2013/09/10/shadow-maps/ 
 *		http://amd-dev.wpengine.netdna-cdn.com/wordpress/media/2012/10/Isidoro-ShadowMapping.pdf
 */
float2 getReceiverPlaneDepthBias (float3 shadowCoord)
{
	float2 biasUV;
	float3 dx = ddx (shadowCoord);
	float3 dy = ddy (shadowCoord);

	biasUV.x = dy.y * dx.z - dx.y * dy.z;
    biasUV.y = dx.x * dy.z - dy.x * dx.z;
    biasUV *= 1.0f / ((dx.x * dy.y) - (dx.y * dy.x));
    return biasUV;
}

//Fix for case 686520 ++
    /**
    * Get Camera space coord from depth
    */
    inline float3 computeCameraSpacePosFromDepth(v2f i)
    {
	    float zdepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
    
	    // view position calculation for oblique perspective case.
	    // this will is not be as precise nor as fast as the other method
	    // but work in all cases.
	    float4 clipPos = float4(i.screenPos.xy, zdepth, 1.0);
	    clipPos.xyz = 2.0f * clipPos.xyz - 1.0f;
	    float4 camPos = mul(unity_CameraInvProjection, clipPos);
	    camPos.xyz /= camPos.w;
	    camPos.z *= -1;
	    return camPos.xyz;
    }
//Fix for case 686520 --

/**
 * Combines the different components of a shadow coordinate and returns the final coordinate.
 */
inline float3 combineShadowcoordComponents (float2 baseUV, float2 deltaUV, float depth, float2 receiverPlaneDepthBias)
{
	float3 uv = float3( baseUV + deltaUV, depth );
	uv.z += dot (deltaUV, receiverPlaneDepthBias); // apply the depth bias
	return uv;
}

/**
 * PCF shadowmap filtering based on a 3x3 kernel (optimized with 4 taps)
 *
 * Algorithm: http://the-witness.net/news/2013/09/shadow-mapping-summary-part-1/
 * Implementation example: http://mynameismjp.wordpress.com/2013/09/10/shadow-maps/
 */
half sampleShadowmap_PCF3x3 (float4 coord, float2 receiverPlaneDepthBias)
{
	const float2 offset = float2(0.5,0.5);
	float2 uv = (coord.xy * _ShadowMapTexture_TexelSize.zw) + offset;
	float2 base_uv = (floor(uv) - offset) * _ShadowMapTexture_TexelSize.xy;
	float2 st = frac(uv);

	float2 uw = float2( 3-2*st.x, 1+2*st.x );
	float2 u = float2( (2-st.x) / uw.x - 1, (st.x)/uw.y + 1 );
	u *= _ShadowMapTexture_TexelSize.x;

	float2 vw = float2( 3-2*st.y, 1+2*st.y );
	float2 v = float2( (2-st.y) / vw.x - 1, (st.y)/vw.y + 1);
	v *= _ShadowMapTexture_TexelSize.y;

    half shadow;
	half sum = 0;

    sum += uw[0] * vw[0] * UNITY_SAMPLE_SHADOW( _ShadowMapTexture, combineShadowcoordComponents( base_uv, float2(u[0], v[0]), coord.z, receiverPlaneDepthBias) );
    sum += uw[1] * vw[0] * UNITY_SAMPLE_SHADOW( _ShadowMapTexture, combineShadowcoordComponents( base_uv, float2(u[1], v[0]), coord.z, receiverPlaneDepthBias) );
    sum += uw[0] * vw[1] * UNITY_SAMPLE_SHADOW( _ShadowMapTexture, combineShadowcoordComponents( base_uv, float2(u[0], v[1]), coord.z, receiverPlaneDepthBias) );
    sum += uw[1] * vw[1] * UNITY_SAMPLE_SHADOW( _ShadowMapTexture, combineShadowcoordComponents( base_uv, float2(u[1], v[1]), coord.z, receiverPlaneDepthBias) );

    shadow = sum / 16.0f;
    shadow = lerp (_LightShadowData.r, 1.0f, shadow);

    return shadow;
}

/**
 * PCF shadowmap filtering based on a 5x5 kernel (optimized with 9 taps)
 *
 * Algorithm: http://the-witness.net/news/2013/09/shadow-mapping-summary-part-1/
 * Implementation example: http://mynameismjp.wordpress.com/2013/09/10/shadow-maps/
 */
half sampleShadowmap_PCF5x5 (float4 coord, float2 receiverPlaneDepthBias)
{

#if defined(SHADOWS_NATIVE)

	const float2 offset = float2(0.5,0.5);
	float2 uv = (coord.xy * _ShadowMapTexture_TexelSize.zw) + offset;
	float2 base_uv = (floor(uv) - offset) * _ShadowMapTexture_TexelSize.xy;
	float2 st = frac(uv);

	float3 uw = float3( 4-3*st.x, 7, 1+3*st.x );
	float3 u = float3( (3-2*st.x) / uw.x - 2, (3+st.x)/uw.y, st.x/uw.z + 2 );
	u *= _ShadowMapTexture_TexelSize.x;

	float3 vw = float3( 4-3*st.y, 7, 1+3*st.y );
	float3 v = float3( (3-2*st.y) / vw.x - 2, (3+st.y)/vw.y, st.y/vw.z + 2 );
	v *= _ShadowMapTexture_TexelSize.y;

	half shadow;
	half sum = 0.0f;

	half3 accum = uw * vw.x;
	sum += accum.x * UNITY_SAMPLE_SHADOW( _ShadowMapTexture, combineShadowcoordComponents( base_uv, float2(u.x,v.x), coord.z, receiverPlaneDepthBias) );
    sum += accum.y * UNITY_SAMPLE_SHADOW( _ShadowMapTexture, combineShadowcoordComponents( base_uv, float2(u.y,v.x), coord.z, receiverPlaneDepthBias) );
    sum += accum.z * UNITY_SAMPLE_SHADOW( _ShadowMapTexture, combineShadowcoordComponents( base_uv, float2(u.z,v.x), coord.z, receiverPlaneDepthBias) );

	accum = uw * vw.y;
    sum += accum.x *  UNITY_SAMPLE_SHADOW( _ShadowMapTexture, combineShadowcoordComponents( base_uv, float2(u.x,v.y), coord.z, receiverPlaneDepthBias) );
    sum += accum.y *  UNITY_SAMPLE_SHADOW( _ShadowMapTexture, combineShadowcoordComponents( base_uv, float2(u.y,v.y), coord.z, receiverPlaneDepthBias) );
    sum += accum.z *  UNITY_SAMPLE_SHADOW( _ShadowMapTexture, combineShadowcoordComponents( base_uv, float2(u.z,v.y), coord.z, receiverPlaneDepthBias) );

	accum = uw * vw.z;
    sum += accum.x * UNITY_SAMPLE_SHADOW( _ShadowMapTexture, combineShadowcoordComponents( base_uv, float2(u.x,v.z), coord.z, receiverPlaneDepthBias) );
    sum += accum.y * UNITY_SAMPLE_SHADOW( _ShadowMapTexture, combineShadowcoordComponents( base_uv, float2(u.y,v.z), coord.z, receiverPlaneDepthBias) );
    sum += accum.z * UNITY_SAMPLE_SHADOW( _ShadowMapTexture, combineShadowcoordComponents( base_uv, float2(u.z,v.z), coord.z, receiverPlaneDepthBias) );

    shadow = sum / 144.0f;

#else // #if defined(SHADOWS_NATIVE)

	// when we don't have hardware PCF sampling, then the above 5x5 optimized PCF really does not work.
	// Fallback to a simple 3x3 sampling with averaged results.

	half shadow = 0;
	float2 base_uv = coord.xy;
	float2 ts = _ShadowMapTexture_TexelSize.xy;
	shadow += UNITY_SAMPLE_SHADOW(_ShadowMapTexture, combineShadowcoordComponents(base_uv, float2(-ts.x,-ts.y), coord.z, receiverPlaneDepthBias));
	shadow += UNITY_SAMPLE_SHADOW(_ShadowMapTexture, combineShadowcoordComponents(base_uv, float2(    0,-ts.y), coord.z, receiverPlaneDepthBias));
	shadow += UNITY_SAMPLE_SHADOW(_ShadowMapTexture, combineShadowcoordComponents(base_uv, float2( ts.x,-ts.y), coord.z, receiverPlaneDepthBias));
	shadow += UNITY_SAMPLE_SHADOW(_ShadowMapTexture, combineShadowcoordComponents(base_uv, float2(-ts.x,    0), coord.z, receiverPlaneDepthBias));
	shadow += UNITY_SAMPLE_SHADOW(_ShadowMapTexture, combineShadowcoordComponents(base_uv, float2(    0,    0), coord.z, receiverPlaneDepthBias));
	shadow += UNITY_SAMPLE_SHADOW(_ShadowMapTexture, combineShadowcoordComponents(base_uv, float2( ts.x,    0), coord.z, receiverPlaneDepthBias));
	shadow += UNITY_SAMPLE_SHADOW(_ShadowMapTexture, combineShadowcoordComponents(base_uv, float2(-ts.x, ts.y), coord.z, receiverPlaneDepthBias));
	shadow += UNITY_SAMPLE_SHADOW(_ShadowMapTexture, combineShadowcoordComponents(base_uv, float2(    0, ts.y), coord.z, receiverPlaneDepthBias));
	shadow += UNITY_SAMPLE_SHADOW(_ShadowMapTexture, combineShadowcoordComponents(base_uv, float2( ts.x, ts.y), coord.z, receiverPlaneDepthBias));
	shadow /= 9.0;

#endif // else of #if defined(SHADOWS_NATIVE)

    shadow = lerp (_LightShadowData.r, 1.0f, shadow);


    return shadow;
}

/**
 *	Samples the shadowmap at the given coordinates.
 */
half unity_sampleShadowmap( float4 coord )
{
	half shadow = UNITY_SAMPLE_SHADOW(_ShadowMapTexture,coord);
	shadow = lerp(_LightShadowData.r, 1.0, shadow);
	return shadow;
}

fixed4 frag_hard (v2f i) : SV_Target
{
	//Fix for case 686520 ++
	    //float zdepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
    	//
	    //// 0..1 linear depth, 0 at near plane, 1 at far plane.
	    //float depth = lerp (Linear01Depth(zdepth), zdepth, unity_OrthoParams.w);
		//
	    //// view position calculation for perspective & ortho cases
	    //float3 vposPersp = i.ray * depth;
	    //float3 vposOrtho = lerp(i.orthoPosNear, i.orthoPosFar, zdepth);
	    //// pick the perspective or ortho position as needed
	    //float3 vpos = lerp (vposPersp, vposOrtho, unity_OrthoParams.w);
		float3 vpos = computeCameraSpacePosFromDepth(i);
	//Fix for case 686520 --

	float4 wpos = mul (unity_CameraToWorld, float4(vpos,1));

	fixed4 cascadeWeights = GET_CASCADE_WEIGHTS (wpos, vpos.z);
	half shadow = unity_sampleShadowmap( GET_SHADOW_COORDINATES(wpos, cascadeWeights) );
	shadow += GET_SHADOW_FADE(wpos, vpos.z);

	fixed4 res = shadow;
	return res;
}
ENDCG


// ----------------------------------------------------------------------------------------
// Subshader for hard shadows:
// Just collect shadows into the buffer. Used on pre-SM3 GPUs and when hard shadows are picked.

SubShader {
	Pass {
		ZWrite Off ZTest Always Cull Off

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag_hard
		#pragma multi_compile_shadowcollector

		ENDCG
	}
}

// ----------------------------------------------------------------------------------------
// Subshader that does PCF 5x5 filtering while collecting shadows.
// Requires SM3 GPU.

Subshader {
	Tags {"ShadowmapFilter"="PCF_5x5"}

Pass {
	ZWrite Off ZTest Always Cull Off

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag_pcf5x5
	#pragma multi_compile_shadowcollector
	#pragma target 3.0

	// 3.0 fragment shader
	fixed4 frag_pcf5x5 (v2f i) : SV_Target
	{
		//Fix for case 686520 ++
	        //float zdepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
        	//
	        //// 0..1 linear depth, 0 at near plane, 1 at far plane.
	        //float depth = lerp (Linear01Depth(zdepth), zdepth, unity_OrthoParams.w);
			//
	        //// view position calculation for perspective & ortho cases
	        //float3 vposPersp = i.ray * depth;
	        //float3 vposOrtho = lerp(i.orthoPosNear, i.orthoPosFar, zdepth);
	        //// pick the perspective or ortho position as needed
	        //float3 vpos = lerp (vposPersp, vposOrtho, unity_OrthoParams.w);
			float3 vpos = computeCameraSpacePosFromDepth(i);
		//Fix for case 686520 --

		// sample the cascade the pixel belongs to
		float4 wpos = mul (unity_CameraToWorld, float4(vpos,1));
		fixed4 cascadeWeights = GET_CASCADE_WEIGHTS (wpos, vpos.z);
		float4 coord = GET_SHADOW_COORDINATES(wpos, cascadeWeights);

		float2 receiverPlaneDepthBiasCascade0 = 0.0;
		float2 receiverPlaneDepthBias = 0.0;
		#if UNITY_USE_RECEIVER_PLANE_BIAS
			// Reveiver plane depth bias: need to calculate it based on shadow coordinate
			// as it would be in first cascade; otherwise derivatives
			// at cascade boundaries will be all wrong. So compute
			// it from cascade 0 UV, and scale based on which cascade we're in.
			// 
			float3 coordCascade0 = getShadowCoord_SingleCascade(wpos);
			receiverPlaneDepthBiasCascade0 = getReceiverPlaneDepthBias (coordCascade0.xyz);
			float biasMultiply = dot(cascadeWeights,unity_ShadowCascadeScales);

			receiverPlaneDepthBias = receiverPlaneDepthBiasCascade0 * biasMultiply;

		    // Static depth biasing to make up for incorrect fractional
		    // sampling on the shadow map grid; from "A Sampling of Shadow Techniques"
		    // (http://mynameismjp.wordpress.com/2013/09/10/shadow-maps/)
    		float fractionalSamplingError = 2 * dot(_ShadowMapTexture_TexelSize.xy, abs(receiverPlaneDepthBias));
    		coord.z -= min(fractionalSamplingError, UNITY_RECEIVER_PLANE_MIN_FRACTIONAL_ERROR);
		#endif


		half shadow = sampleShadowmap_PCF5x5( coord, receiverPlaneDepthBias );


		// Blend between shadow cascades if enabled
		//
		// Not working yet with split spheres, and no need when 1 cascade
		#if UNITY_USE_CASCADE_BLENDING && !defined(SHADOWS_SPLIT_SPHERES) && !defined(SHADOWS_SINGLE_CASCADE)
		half4 z4 = (float4(vpos.z,vpos.z,vpos.z,vpos.z) - _LightSplitsNear) / (_LightSplitsFar-_LightSplitsNear);
		half alpha = dot (z4 * cascadeWeights, half4(1,1,1,1));

		UNITY_BRANCH
		if (alpha > 1-UNITY_CASCADE_BLEND_DISTANCE)
		{
			// get alpha to 0..1 range over the blend distance
			alpha = (alpha-(1-UNITY_CASCADE_BLEND_DISTANCE)) / UNITY_CASCADE_BLEND_DISTANCE;

			// sample next cascade
			cascadeWeights = fixed4 (0, cascadeWeights.xyz);
			coord = GET_SHADOW_COORDINATES(wpos, cascadeWeights);

			#if UNITY_USE_RECEIVER_PLANE_BIAS
				biasMultiply = dot(cascadeWeights,unity_ShadowCascadeScales);
				receiverPlaneDepthBias = receiverPlaneDepthBiasCascade0 * biasMultiply;
	    		fractionalSamplingError = 2 * dot(_ShadowMapTexture_TexelSize.xy, abs(receiverPlaneDepthBias));
    			coord.z -= min(fractionalSamplingError, UNITY_RECEIVER_PLANE_MIN_FRACTIONAL_ERROR);
			#endif

			half shadowNextCascade = sampleShadowmap_PCF3x3 (coord, receiverPlaneDepthBias);

			shadow = lerp (shadow, shadowNextCascade, alpha);
		}
		#endif

		shadow += GET_SHADOW_FADE(wpos, vpos.z);
		return shadow;
	}
	ENDCG
}
}

Fallback Off
}