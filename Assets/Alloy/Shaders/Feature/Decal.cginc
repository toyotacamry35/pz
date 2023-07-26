// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Decal.cginc
/// @brief Handles vertex-weighted alpha-blended decals.
/////////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FEATURE_DECAL_CGINC
#define ALLOY_SHADERS_FEATURE_DECAL_CGINC

#if !defined(A_DECAL_ON) && defined(_DECAL_ON)
    #define A_DECAL_ON
#endif

#include "Assets/Alloy/Shaders/Framework/Feature.cginc"

#ifdef A_DECAL_ON
    /// The decal tint color.
    /// Expects a linear LDR color with alpha.
    half4 _DecalColor;
    
    /// Decal texture.
    /// Expects an RGBA map with sRGB sampling.
    A_SAMPLER_2D(_DecalTex);
    
	A_SAMPLER_2D(_DecalSpecTex);
	
	A_SAMPLER_2D(_DecalBumpMap);

	/// Weight of the decal effect.
	/// Expects values in the range [0,1].
	half _DecalWeight;
        
    /// The specularity that will be applied over the decal.
    /// Expects values in the range [0,1].
    half _DecalSpecularity;

	/// The specularity that will be applied over the decal.
	/// Expects values in the range [0,1].
	half _DecalRoughness;
    

	half _DecalBumpScale;

#endif

void aDecal(
    inout ASurface s)
{
#ifdef A_DECAL_ON
    float2 detailUv = A_TEX_TRANSFORM_UV(s, _DecalTex);
    half4 decalColor = _DecalColor * tex2D(_DecalTex, detailUv);
	half weight = s.mask * _DecalWeight * decalColor.a;
	
	half3 decalNormals = UnpackScaleNormal(tex2D(_DecalBumpMap, detailUv), _DecalBumpScale);
	half4 material = tex2D(_DecalSpecTex, detailUv);
    
	s.baseColor = lerp(s.baseColor, decalColor.rgb, weight);
	s.normalTangent = A_NT(s, lerp(s.normalTangent, decalNormals, weight));
	
	
	s.metallic = lerp(s.metallic, material.A_METALLIC_CHANNEL, weight);
	s.ambientOcclusion = lerp(s.ambientOcclusion, material.A_AO_CHANNEL, weight);
	s.specularity = lerp(s.specularity, _DecalSpecularity * material.A_SPECULARITY_CHANNEL, weight);
	s.roughness = lerp(s.roughness, _DecalRoughness * material.A_ROUGHNESS_CHANNEL, weight);
	
#endif 
} 

#endif // ALLOY_SHADERS_FEATURE_DECAL_CGINC
