Shader "Hidden/TerrainBaker" {
	Properties{
		_MainTex("NormalsHeights", 2D) = "black" {}
		TerrainMaterials("TerrainMaterials", 2D) = "black" {}
		TerrainWeights("TerrainWeights", 2D) = "black" {}
		TerrainSizes("TerrainSizes", Vector) = (256, 0.00390625, 256, 0.00390625)
		TerrainMatTexInfo("TerrainMatTexInfo", Vector) = (257, 0.001945525, 0.003891051, 0.9961089494)
		TerrainWgtTexInfo("TerrainWgtTexInfo", Vector) = (514, 0.0009727627, 0.0019455253, 0)
	}
	SubShader{
		Tags 
		{ 
			"RenderType" = "Opaque" 
			"Queue" = "Geometry+10"
		}
		LOD 200

		CGPROGRAM

		#pragma surface surf StandardSpecular vertex:vert

		#pragma target 4.6

		#pragma multi_compile __ MACRO_TEXTURE
		#pragma multi_compile __ EMISSION_TEXTURE

		struct Input {
			float3 worldPos;
		};

		#include "TerrainAtlas.cginc"


		sampler2D TerrainMaterials;
		sampler2D TerrainWeights;
		sampler2D _MainTex;


		uniform float4 TerrainMatTexInfo;
		uniform float4 TerrainWgtTexInfo;
		uniform float4 TerrainSizes;

		struct TerrainMaterialInfo
		{
			fixed4 color;
			float2 uv;
			float mipLevel;
			int materialIndex;
			ShaderLayer shaderLayer;
		};

		struct TerrainMaterial
		{
			TerrainMaterialInfo channels[4];
		};


		void ReadMaterial(inout TerrainMaterial mtl, float2 baseUV, float baseMipLevel, int materialIndex, int channelIndex)
		{
			ShaderLayer shaderLayer = GetTerrainAtlasLayer(materialIndex);
			float2 layerUV = baseUV * shaderLayer.textureTilling + shaderLayer.textureOffset;
			float mipLevelAlbedo = baseMipLevel + shaderLayer.textureMipBias + TERRAIN_TEX_MIP_ALBEDO;
			mtl.channels[channelIndex].uv = layerUV;
			mtl.channels[channelIndex].mipLevel = baseMipLevel + shaderLayer.textureMipBias;
			mtl.channels[channelIndex].materialIndex = materialIndex;
			mtl.channels[channelIndex].color = UNITY_SAMPLE_TEX2DARRAY_LOD(TerrainSplatsAlbedo, float3(layerUV, shaderLayer.albedoIndex), mipLevelAlbedo);
			mtl.channels[channelIndex].color.a = mtl.channels[channelIndex].color.a * shaderLayer.albedoHeightMapScale + shaderLayer.albedoHeightMapOffset;
			mtl.channels[channelIndex].shaderLayer = shaderLayer;
		}

		float3 ReadNormalFromTextrue(float2 terrainUV)
		{
			float3 norm = 0;
			norm.xz = (tex2Dlod(_MainTex, float4(terrainUV, 0, 0)).rg - 127.0 / 255.0) * 2.0;
			norm.y = sqrt(1 - dot(norm.xz, norm.xz));
			return normalize(norm);
		}

		void vert(inout appdata_full v)
		{
			v.normal = float3(0, 0, 1);
			v.tangent = float4(1, 0, 0, 1);
		}

		void surf(Input IN, inout SurfaceOutputStandardSpecular o)
		{

			float2 terrainUV = frac(IN.worldPos.xz*TerrainSizes.xz);
			float2 baseUV = IN.worldPos.xz;
			float2 dx = ddx(baseUV);
			float2 dy = ddy(baseUV);
			float d = max(dot(dx, dx), dot(dy, dy));
			float baseMipLevel = 0.5*log2(d);


			float2 materialUV = terrainUV * TerrainMatTexInfo.w + TerrainMatTexInfo.y;

			float2 posWeights = floor(materialUV * TerrainMatTexInfo.x) * TerrainMatTexInfo.z + TerrainWgtTexInfo.y;
			float2 fracPosWeights = frac(materialUV * TerrainMatTexInfo.x);
			float2 weightsUV = posWeights + fracPosWeights * TerrainWgtTexInfo.z;

			int4 matIndeces = floor(tex2Dlod(TerrainMaterials, float4(materialUV, 0, 0)) * 255);
			fixed4 layerWeights = tex2Dlod(TerrainWeights, float4(weightsUV, 0, 0));

			TerrainMaterial mtl = (TerrainMaterial)0;
			//Channels layer
			float4 materialHeights = 0;
			float4 reliefFactor = 0;
			[unroll]
			for (int i = 0; i < 4; i++)
			{
				ReadMaterial(mtl, baseUV, baseMipLevel, matIndeces[i], i);
				materialHeights[i] = mtl.channels[i].color.w;
				reliefFactor[i] = mtl.channels[i].shaderLayer.reliefFactor;
			}

			float4 wgt = 0;
			float4 ws = layerWeights + lerp(materialHeights, materialHeights * materialHeights * 2, saturate((reliefFactor - 0.6) * (1/0.4)));
			ws *= saturate(layerWeights * 10);
			ws = lerp(layerWeights * 4, ws, saturate(reliefFactor * 2));

			float wmax = max(max(ws.x, ws.y), max(ws.z, ws.w)) - 0.2;
			wgt = max(ws - wmax.xxxx, 0);
			wgt = wgt > 0.01 ? wgt : 0;
			wgt /= max(dot(wgt, 1), 0.0001);

			fixed3 color = 0;
			float3 normal = 0;
			fixed4 parameters = 0;
			float3 emission = 0;

			[unroll]
			for (int j = 0; j < 4; j++)
			{
				float weight = wgt[j];
				[branch]
				if (weight > 0.0)
				{
					float3 normalUVW = float3(mtl.channels[j].uv, mtl.channels[j].shaderLayer.normalsIndex);
					float3 layerNormal = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY_LOD(TerrainSplatsNormal, normalUVW, mtl.channels[j].mipLevel + TERRAIN_TEX_MIP_NORMALS));
					float3 parametersUVW = float3(mtl.channels[j].uv, mtl.channels[j].shaderLayer.parametersIndex);
					fixed4 layerParameters = UNITY_SAMPLE_TEX2DARRAY_LOD(TerrainSplatsParameters, parametersUVW, mtl.channels[j].mipLevel + TERRAIN_TEX_MIP_PARAMETERS);
					fixed3 layerColor = mtl.channels[j].color.rgb;
#if EMISSION_TEXTURE
					[branch]
					if (mtl.channels[j].shaderLayer.emissionIndex >= 0)
					{
						float mipLevelEmission = baseMipLevel + mtl.channels[j].shaderLayer.textureMipBias + TERRAIN_TEX_MIP_ALBEDO;
						float3 emissionUVW = float3(mtl.channels[j].uv, mtl.channels[j].shaderLayer.emissionIndex);
						float3 emissionColor = UNITY_SAMPLE_TEX2DARRAY_LOD(TerrainSplatsEmission, emissionUVW, mipLevelEmission).rgb * mtl.channels[j].shaderLayer.emissionTint;
						emission += emissionColor * weight;
					}
#endif

#if MACRO_TEXTURE
					[branch]
					if (mtl.channels[j].shaderLayer.macroIndex >= 0)
					{
						float mipLevelMacro = baseMipLevel + mtl.channels[j].shaderLayer.macroMipBias + TERRAIN_TEX_MIP_MACRO;
						float3 macroUVW = float3(mtl.channels[j].uv * mtl.channels[j].shaderLayer.macroTilling, mtl.channels[j].shaderLayer.macroIndex);
						float3 macroColor = UNITY_SAMPLE_TEX2DARRAY_LOD(TerrainSplatsMacro, macroUVW, mipLevelMacro).rgb;

						layerColor = layerColor * macroColor * 2;
					}
#endif
					color += layerColor * weight;
					normal += layerNormal * weight;
					parameters += layerParameters * weight;
				}

			}

			fixed metalMask = parameters.r;
			fixed ao = parameters.g;
			fixed specular = parameters.b;
			fixed roughtness = parameters.a;

			float3 worldNormal = ReadNormalFromTextrue(terrainUV);
			float3 worldTangent = float3(1, 0, 0);
			float3 worldBinormal = cross(worldNormal, worldTangent);
			worldTangent = cross(worldBinormal, worldNormal);
			float3x3 transformToWorld = float3x3(worldTangent.x, worldBinormal.x, worldNormal.x,
													worldTangent.y, worldBinormal.y, worldNormal.y,
													worldTangent.z, worldBinormal.z, worldNormal.z);

			o.Specular = saturate(lerp(specular.xxx*0.08, color, metalMask));
			o.Albedo = color;

#if UNITY_CONSERVE_ENERGY
			float minInvSpec = 0.01;
	#if UNITY_CONSERVE_ENERGY_MONOCHROME
			half oneMinusReflectivity = 1 - SpecularStrength(o.Specular);
			o.Albedo /= max(oneMinusReflectivity, minInvSpec);
	#else
			o.Albedo /= max((half3(1, 1, 1) - o.Specular), half3(minInvSpec, minInvSpec, minInvSpec));
	#endif
#endif

			//normal = float3(0, 0, 1);
			o.Normal = mul(transformToWorld, normalize(normal));
			o.Emission = emission;
			o.Smoothness = 1.0 - roughtness;
			o.Occlusion = ao;
			o.Alpha = 1.0;



			#if 0	//set 1 for debug
			o.Emission = ;		//debug color
			o.Albedo = 0;
			o.Specular = 0;
			o.Normal = float3(0, 1, 0);
			o.Smoothness = 1.0;
			o.Occlusion = 1;
			o.Alpha = 1;
			#endif

		}
		ENDCG
	}
	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"LightMode" = "ShadowCaster"
		}
		LOD 200

		CGPROGRAM

		#pragma vertex vert
		#pragma surface surf StandardSpecular

		void vert(inout appdata_full v)
		{
			v.vertex.xyz -= UNITY_MATRIX_V[2].xyz*0.8 + float3(0, 0.2, 0);
		}

		struct Input
		{
			float3 worldPos;
		};


		void surf(Input IN, inout SurfaceOutputStandardSpecular o)
		{
		}

		ENDCG
	}
	FallBack Off
}

