// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file DirectionalBlend.cginc
/// @brief Directional Blend shader definition.
/////////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_DEFINITION_MESHBLENDING_CGINC
#define ALLOY_SHADERS_DEFINITION_MESHBLENDING_CGINC

#define A_MAIN_TEXTURES_ON
#define A_MAIN_TEXTURES_CUTOUT_OFF
#define A_DIRECTIONAL_BLEND_ON
#define A_SECONDARY_TEXTURES_ON
#define DIRECTIONAL_BLEND_COLONY


float _OcclusionStrength;
//sampler2D _CoatingMask;
//float _CoatingMaskPower;
//float _CoatingMaskSpin;
//float4 _CoatingMask_ST;

half4 _NormalSwitch;
half _BlendingWeight;

#define SchlickFresnelApproxExp2Const (-8.6562)
float _MetalnessTerrain;
float _GlossMin;
float _GlossMax;
float _DiffuseScattering;

#include "Assets/Alloy/Shaders/Lighting/Standard.cginc"
#include "Assets/Alloy/Shaders/Type/Standard.cginc"
#include "Assets/Alloy/Shaders/Unity/TerrainBlend.cginc" 



void aSurfaceShader(
    inout ASurface s)
{
	//aParallax(s);
    aMainTextures(s);
    //aDissolve(s);
    //aTeamColor(s);
    //aAo2(s);
    //aDecal(s);
	aDirectionalBlendWorld(s);
    s.mask = 1 - s.mask;
    aDetail(s);
    s.mask = 1 - s.mask;
    aMeshBlending(s);
	aEmission(s);
    //aCutout(s);
	s.mask = 1 - s.mask;
    aWetness(s);
    aRim(s);
}

#endif // ALLOY_SHADERS_DEFINITION_DIRECTIONAL_BLEND_CGINC
