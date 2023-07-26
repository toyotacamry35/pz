// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Dissolve.cginc
/// @brief Surface dissolve effects.
/////////////////////////////////////////////////////////////////////////////////
float _DeathCutoff;

#ifndef ALLOY_SHADERS_FEATURE_DISSOLVE_CGINC
#define ALLOY_SHADERS_FEATURE_DISSOLVE_CGINC

#if !defined(A_DISSOLVE_ON) && defined(_DISSOLVE_ON)
    #define A_DISSOLVE_ON
#endif

#ifdef A_DISSOLVE_ON
    #ifndef A_OPACITY_MASK_ON
        #define A_OPACITY_MASK_ON
    #endif

    #ifndef A_EMISSIVE_COLOR_ON
        #define A_EMISSIVE_COLOR_ON
    #endif
#endif

#include "Assets/Alloy/Shaders/Framework/Feature.cginc"

#ifdef A_DISSOLVE_ON
    
	float _MainTiling;
	sampler2D _MaskMap; uniform float4 _MaskMap_ST;
	float _Difference;
	sampler2D _DiffNoise; uniform float4 _DiffNoise_ST;
	float _DiffNoiseMaskMult;
	float _DiffNoiseTiling;
	float _PanSpeed;

	half3 _DissolveColor;
	half _DissolveGloss;

	void Triplanar(float3 worldPos, float3 normal, inout float3 col, inout float4 diffNoise)
{
	float3 _INPUT_uv = worldPos.xzy;
	float _uv_Relief_z = 1.0;
	fixed3 c;
	fixed4 dN;
	float3 normalHeight = 0;

	//half diffZ = saturate(worldPos.x * doubleVec.x + worldPos.y * doubleVec.y + worldPos.z * doubleVec.z + _DoubleUp);

	float3 triplanar_blend = abs(normal);
	bool3 triplanar_flip = normal.xyz >= 0;
	float triplanar_blend_y_uncomp = triplanar_blend.y;

	float3 triplanar_blend_tmp = triplanar_blend / dot(triplanar_blend, 1);

	triplanar_blend /= dot(triplanar_blend, 1);
	triplanar_blend = pow(triplanar_blend, lerp(32, 64, _uv_Relief_z));
	triplanar_blend /= dot(triplanar_blend, 1);

	bool3 triplanar_blend_vector = triplanar_blend < 0.98;
	bool triplanar_blend_flag = all(triplanar_blend_vector);
	float triplanar_blend_simple = max(max(triplanar_blend.x, triplanar_blend.y), triplanar_blend.z);

	float3 uvTRI = float3(_INPUT_uv.xy, 0);
	uvTRI = abs(triplanar_blend_simple - triplanar_blend.x) < 0.0001 ? float3(_INPUT_uv.yz, 0) : uvTRI;
	uvTRI = abs(triplanar_blend_simple - triplanar_blend.z) < 0.0001 ? float3(_INPUT_uv.xz, 0) : uvTRI;
	float timeSpeed = _Time.g*_PanSpeed;
	
	if (triplanar_blend_flag)
	{
		float3 nA, nB, nC;
		nA = nB = nC = float3(0, 0, 1);

		float3 uvTRI1 = float3(_INPUT_uv.yz, 1);
		float3 uvTRI2 = float3(_INPUT_uv.xy, 1);
		float3 uvTRI3 = float3(_INPUT_uv.xz, 1);
		

		if (triplanar_blend.x > 0.02)
		{
			float2 uv = uvTRI1.xy * _MainTiling;
			c = tex2D(_MaskMap,TRANSFORM_TEX(uv, _MaskMap)).rgb;
			float2 uv2 = ((uvTRI1.xy * _DiffNoiseTiling)+timeSpeed);
			dN = tex2D(_DiffNoise,TRANSFORM_TEX(uv2, _DiffNoise));
			 
			col += triplanar_blend.x * c; 
			diffNoise += triplanar_blend.x * dN; 
		}

		if (triplanar_blend.y > 0.02)
		{
			float2 uv = uvTRI2.xy * _MainTiling;
			c = tex2D(_MaskMap,TRANSFORM_TEX(uv, _MaskMap)).rgb;
			float2 uv2 = ((uvTRI2.xy * _DiffNoiseTiling)+timeSpeed);
			dN = tex2D(_DiffNoise,TRANSFORM_TEX(uv2, _DiffNoise));
			
			col += triplanar_blend.y * c;
			diffNoise += triplanar_blend.y * dN; 
		}

		if (triplanar_blend.z > 0.02)
		{
			float2 uv = uvTRI3.xy * _MainTiling;
			c = tex2D(_MaskMap,TRANSFORM_TEX(uv, _MaskMap)).rgb;
			float2 uv2 = ((uvTRI3.xy * _DiffNoiseTiling)+timeSpeed);
			dN = tex2D(_DiffNoise,TRANSFORM_TEX(uv2, _DiffNoise));
			
			col += triplanar_blend.z * c;
			diffNoise += triplanar_blend.z * dN; 
		}

	}
	else
	{
		float2 uv = uvTRI.xy * _MainTiling;
		c = tex2D(_MaskMap,TRANSFORM_TEX(uv, _MaskMap)).rgb;
		float2 uv2 = ((uvTRI.xy * _DiffNoiseTiling)+timeSpeed);
		dN = tex2D(_DiffNoise,TRANSFORM_TEX(uv2, _DiffNoise));
			
		col += c;
		diffNoise += dN; 
	}
}
	
#endif



float aDissolve(
    inout ASurface s) 
{
float diffD = 0;
#ifdef A_DISSOLVE_ON

    float3 col = 0;
	float4 diffNoise = 0;
	Triplanar(s.positionWorld, s.normalWorld, col, diffNoise);
		
	float node_6786 = ceil(saturate((col.r-((_DeathCutoff-100.0)/100.0))));
	float node_4131 = (node_6786-ceil(saturate((col.r-((_DeathCutoff-100.0+_Difference)/100.0)))));
	
	float node_3593 = ((node_4131 * diffNoise.r)*_DiffNoiseMaskMult);
	

    clip((lerp(node_6786,node_3593,node_4131)-(1.0 - s.opacity)) - 0.5);
	
	diffD = node_4131;
#endif
return diffD;
}




void aDissolveSecond(inout ASurface s, float diffD)
{
#ifdef A_DISSOLVE_ON
	s.baseColor = lerp(s.baseColor, _DissolveColor, diffD);
	s.roughness = lerp(s.roughness, _DissolveGloss, diffD);
#endif
}



#endif // ALLOY_SHADERS_FEATURE_DISSOLVE_CGINC
