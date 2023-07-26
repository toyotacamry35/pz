Shader "Hidden/PaintTerrainWeights"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			#define COLLECTION_SIZE	16
			#include "TerrainBakerUtils.cginc"

			struct appdata
			{
				float3 vertex : POSITION;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};


			float _MaterialMapSize;

			Texture2D _Materials;

			#define ARRAY_SIZE	32

			Texture2D _Layers0;
			Texture2D _Layers1;
			Texture2D _Layers2;
			Texture2D _Layers3;
			Texture2D _Layers4;
			Texture2D _Layers5;
			Texture2D _Layers6;
			Texture2D _Layers7;

			SamplerState layer_point_clamp_sampler;


			v2f vert(appdata v)
			{
				v2f o;
				o.vertex.xyz = v.vertex;
				o.vertex.w = 1.0;
				o.uv = 0.5 + v.vertex*float2(0.5, -0.5);
				return o;
			}

			void AddMaterialsToCollection(inout float2 collection[16], in float4 mats[4])
			{
				[unroll]
				for (int i = 0; i < 4; i++)
				{
					AddUnique(collection, mats[i].r, 1);
					AddUnique(collection, mats[i].g, 1);
					AddUnique(collection, mats[i].b, 1);
					AddUnique(collection, mats[i].a, 1);
				}
			}

			void ReadLayersFromtexture(int index, Texture2D tex, float2 uv, inout float layers[ARRAY_SIZE])
			{
				float4 lw = tex.SampleLevel(layer_point_clamp_sampler, uv, 0);
				layers[index * 4 + 0] = lw.r;
				layers[index * 4 + 1] = lw.g;
				layers[index * 4 + 2] = lw.b;
				layers[index * 4 + 3] = lw.a;
			}

			void ReadAlphaMapLayers(float2 uv, out float layers[ARRAY_SIZE])
			{
				ReadLayersFromtexture(0, _Layers0, uv, layers);
				ReadLayersFromtexture(1, _Layers1, uv, layers);
				ReadLayersFromtexture(2, _Layers2, uv, layers);
				ReadLayersFromtexture(3, _Layers3, uv, layers);
				ReadLayersFromtexture(4, _Layers4, uv, layers);
				ReadLayersFromtexture(5, _Layers5, uv, layers);
				ReadLayersFromtexture(6, _Layers6, uv, layers);
				ReadLayersFromtexture(7, _Layers7, uv, layers);
			}

			void ReadMaterialsBlock(float2 uv, out float4 mats[9], float stepSize)
			{
				[unroll]
				for (int v = -1, index = 0; v <= 1; v++)
				{
					[unroll]
					for (int u = -1; u <= 1; u++, index++)
					{
						mats[index] = _Materials.SampleLevel(layer_point_clamp_sampler, uv + float2(stepSize * u, stepSize * v), 0);
					}
				}
			}

			float IsCoverageMaterial(in float4 mats[9], float material)
			{
				float presentCount = 0;
				[unroll]
				for (int i = 0; i < 9; i++)
				{
					float4 presentMask = (mats[i] == material) ? 1 : 0;
					presentCount += saturate(dot(presentMask, 1));
				}
				return presentCount == 9 ? 1 : 0;
			}

			float4 GetBrokeWeightsFixMask(float2 uv, float4 currentQuadMats, float weightsPixelSize)
			{
				float4 mats[9];
				ReadMaterialsBlock(uv, mats, weightsPixelSize);
				float4 channelsMask = 1;
				channelsMask.x = IsCoverageMaterial(mats, currentQuadMats.x);
				channelsMask.y = IsCoverageMaterial(mats, currentQuadMats.y);
				channelsMask.z = IsCoverageMaterial(mats, currentQuadMats.z);
				channelsMask.w = IsCoverageMaterial(mats, currentQuadMats.w);
				return channelsMask;
			}


			fixed4 frag(v2f inp) : SV_Target
			{
				const float weightsMapSize = _MaterialMapSize * 2.0;				
				const float alphaMapSize = _MaterialMapSize - 1.0;

				const float scaleAlphaMap = _MaterialMapSize / (_MaterialMapSize - 1);
				const float offsetAlphaMap = -0.5 / _MaterialMapSize;

				float layers[ARRAY_SIZE];
				ReadAlphaMapLayers(inp.uv*scaleAlphaMap + offsetAlphaMap, layers);

				float4 currentQuadMats = _Materials.SampleLevel(layer_point_clamp_sampler, inp.uv, 0);

				float4 channelsMask = (currentQuadMats < NO_MATERIAL_TRH) ? 1 : 0;
				int4 layerIndex = (int4)(currentQuadMats * 255.0 * channelsMask);

				float4 weights = 0;
				weights.r = layers[layerIndex.r];
				weights.g = layers[layerIndex.g];
				weights.b = layers[layerIndex.b];
				weights.a = layers[layerIndex.a];				
				weights *= channelsMask;

				weights *= GetBrokeWeightsFixMask(inp.uv, currentQuadMats, 1.0 / weightsMapSize);


#ifdef FORCE_SET_ALPHA_CHANNEL_FOR_DEBUG
				weights.a = 1;//debug, now 3 layers in work
#endif

				return weights;
			}
			ENDCG
		}
	}
}
