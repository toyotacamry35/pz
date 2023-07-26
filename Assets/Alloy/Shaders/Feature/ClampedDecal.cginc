/////////////////////////////////////////////////////////////////////////////////
/// @file ClampedDecal.cginc
/// @brief Handles vertex-weighted alpha-blended decals with mask.
/////////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FEATURE_CLAMPEDDECAL_CGINC
#define ALLOY_SHADERS_FEATURE_CLAMPEDDECAL_CGINC

#if !defined(A_CLAMPEDDECAL_ON) && defined(_CLAMPEDDECAL_ON)
    #define A_CLAMPEDDECAL_ON
#endif

#include "Assets/Alloy/Shaders/Framework/Feature.cginc"

#ifdef A_CLAMPEDDECAL_ON    
    /// Decal texture.
    /// Expects an RGBA map with sRGB sampling.
    A_SAMPLER_2D(_ClampedDecalTex);
    
	A_SAMPLER_2D(_ClampedDecalSpecTex);
	
	A_SAMPLER_2D(_ClampedDecalNormalMap);

	/// 'a' value of the equation y = sat(a*x + b) of the decal effect.
	half _ClampedDecalSlope;

	/// 'b' value of the equation y = sat(a*x + b) of the decal effect.
	half _ClampedDecalYintercept;

    /// The specularity that will be applied over the decal.
    /// Expects values in the range [0,1].
    half _ClampedDecalSpecularity;

	/// The specularity that will be applied over the decal.
	/// Expects values in the range [0,1].
	half _ClampedDecalRoughness;
    
	half _ClampedDecalNormalScale;

	half _ClampedDecalNormalWeight;

#endif

void aClampedDecal(
    inout ASurface s)
{
#ifdef A_CLAMPEDDECAL_ON
    float2 detailUv = A_TEX_TRANSFORM_UV(s, _ClampedDecalTex);
    half4 clampedDecalColor = tex2D(_ClampedDecalTex, detailUv);

	half weight = s.mask * saturate(_ClampedDecalSlope * clampedDecalColor.a + _ClampedDecalYintercept);

	half3 decalNormals = UnpackScaleNormal(tex2D(_ClampedDecalNormalMap, detailUv), _ClampedDecalNormalScale);
	half4 material = tex2D(_ClampedDecalSpecTex, detailUv);
    
	s.baseColor = lerp(s.baseColor, clampedDecalColor.rgb, weight);
	s.normalTangent = A_NT(s, lerp(s.normalTangent, decalNormals, weight * _ClampedDecalNormalWeight));
	
	s.metallic = lerp(s.metallic, material.A_METALLIC_CHANNEL, weight);
	s.ambientOcclusion = lerp(s.ambientOcclusion, material.A_AO_CHANNEL, weight);
	s.specularity = lerp(s.specularity, _ClampedDecalSpecularity * material.A_SPECULARITY_CHANNEL, weight);
	s.roughness = lerp(s.roughness, _ClampedDecalRoughness * material.A_ROUGHNESS_CHANNEL, weight);
	
#endif
} 

#endif // ALLOY_SHADERS_FEATURE_CLAMPEDDECAL_CGINC
