Shader "Hidden/SortTerrainLayers"
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

			#define COLLECTION_SIZE	32
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

			float _TexelSize;

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


			float4 ReadWeights(Texture2D tex, float2 uv)
			{
				//return tex.SampleLevel(layer_point_clamp_sampler, uv, 0);

				float4 weight = 0;
				[unroll]
				for (int y = -1; y <= 1; y++)
				{
					[unroll]
					for (int x = -1; x <= 1; x++)
					{
						float2 offset = float2(x, y)*_TexelSize;
						weight += tex.SampleLevel(layer_point_clamp_sampler, uv + offset, 0);
					}
				}
				return weight;

			}

			void FillCollectionbyIndex(int index, float4 weights, inout float2 collection[COLLECTION_SIZE])
			{
				int baseIndex = index * 4;
				collection[baseIndex + 0] = float2((baseIndex + 0) / 255.0, weights.x);
				collection[baseIndex + 1] = float2((baseIndex + 1) / 255.0, weights.y);
				collection[baseIndex + 2] = float2((baseIndex + 2) / 255.0, weights.z);
				collection[baseIndex + 3] = float2((baseIndex + 3) / 255.0, weights.w);
			}

			void MakeLayersCollection(float2 uv, inout float2 collection[COLLECTION_SIZE])
			{
				FillCollectionbyIndex(0, ReadWeights(_Layers0, uv), collection);
				FillCollectionbyIndex(1, ReadWeights(_Layers1, uv), collection);
				FillCollectionbyIndex(2, ReadWeights(_Layers2, uv), collection);
				FillCollectionbyIndex(3, ReadWeights(_Layers3, uv), collection);
				FillCollectionbyIndex(4, ReadWeights(_Layers4, uv), collection);
				FillCollectionbyIndex(5, ReadWeights(_Layers5, uv), collection);
				FillCollectionbyIndex(6, ReadWeights(_Layers6, uv), collection);
				FillCollectionbyIndex(7, ReadWeights(_Layers7, uv), collection);
			}


			fixed4 frag(v2f inp) : SV_Target
			{
				float2 collection[32];
				ResetCollection(collection);

				MakeLayersCollection(inp.uv, collection);
				SortCollection(collection);

				for (int i = 0; i < 4; i++)
				{
					if (collection[i].y < ALPHA_MAP_TRH) collection[i] = float2(1, 0);
				}

				float4 result = float4(collection[0].x, collection[1].x, collection[2].x, collection[3].x);

				//result = float4(collection[0].x, collection[1].x, collection[2].x, 1);
				//result = float4(collection[0].y, collection[1].y, collection[2].y, 1);
#ifdef FORCE_SET_ALPHA_CHANNEL_FOR_DEBUG
				result.a = 1;//debug, now 3 layers in work
#endif
				//return float4((inp.uv * 32)/255.0, 0, 1);


				return result;
			}
			ENDCG
		}
	}
}
