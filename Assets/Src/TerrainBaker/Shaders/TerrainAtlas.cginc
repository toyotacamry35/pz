#ifndef TERRAIN_ATLAS_SHADER_BASE
#define TERRAIN_ATLAS_SHADER_BASE


struct ShaderLayer
{
	float2 textureOffset;
	float textureTilling;
	float textureMipBias;
	float albedoHeightMapOffset;
	float albedoHeightMapScale;
	float reliefFactor;
	float albedoIndex;
	float normalsIndex;
	float parametersIndex;
	float emissionIndex;
	float macroIndex;
	float macroTilling;
	float macroMipBias;
	float macroScale;
	float3 emissionTint;
	float reserved0;
	float reserved1;
};


UNITY_DECLARE_TEX2DARRAY(TerrainSplatsAlbedo);
UNITY_DECLARE_TEX2DARRAY(TerrainSplatsNormal);
UNITY_DECLARE_TEX2DARRAY(TerrainSplatsParameters);
UNITY_DECLARE_TEX2DARRAY(TerrainSplatsEmission);
UNITY_DECLARE_TEX2DARRAY(TerrainSplatsMacro);



#ifdef SHADER_API_D3D11
StructuredBuffer<ShaderLayer> TerrainLayersBuffer;
#endif


uniform float4 TerrainTexturesMipInfo;


#define TERRAIN_TEX_MIP_ALBEDO		TerrainTexturesMipInfo.x
#define TERRAIN_TEX_MIP_NORMALS		TerrainTexturesMipInfo.x
#define TERRAIN_TEX_MIP_PARAMETERS	TerrainTexturesMipInfo.y
#define TERRAIN_TEX_MIP_MACRO		TerrainTexturesMipInfo.z


ShaderLayer GetTerrainAtlasLayer(int index)
{
	ShaderLayer shaderLayer = (ShaderLayer)0;
#ifdef SHADER_API_D3D11
	shaderLayer = TerrainLayersBuffer[index];
#endif
	return shaderLayer;
}

#endif

