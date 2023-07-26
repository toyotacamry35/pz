Shader "Hidden/PaintTerrainLayers"
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
			#define COLLECTION_SIZE 16
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

			float _HalfLayersTexelSize;
			float _TextureScale;


			Texture2D _SortedLayers;			

			SamplerState layer_point_clamp_sampler;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex.xyz = v.vertex;
				o.vertex.w = 1.0;
				o.uv = 0.5 + v.vertex*float2(0.5, -0.5) *_TextureScale;

				//o.uv = 0.5 + v.vertex*float2(0.5, -0.5);

				return o;
			}


			fixed4 frag(v2f inp) : SV_Target
			{
				float4 mats[4];
				mats[0] = _SortedLayers.SampleLevel(layer_point_clamp_sampler, inp.uv + float2(-1, -1) * _HalfLayersTexelSize, 0);
				mats[1] = _SortedLayers.SampleLevel(layer_point_clamp_sampler, inp.uv + float2(1, -1) * _HalfLayersTexelSize, 0);
				mats[2] = _SortedLayers.SampleLevel(layer_point_clamp_sampler, inp.uv + float2(-1, 1) * _HalfLayersTexelSize, 0);
				mats[3] = _SortedLayers.SampleLevel(layer_point_clamp_sampler, inp.uv + float2(1, 1) * _HalfLayersTexelSize, 0);
				
				float2 collection[16];
				ResetCollection(collection);
				[unroll]
				for (int i = 0; i < 4; i++)
				{
					AddUnique(collection, mats[i].r, 1.0);
					AddUnique(collection, mats[i].g, 0.1);
					AddUnique(collection, mats[i].b, 0.01);
					AddUnique(collection, mats[i].a, 0.001);
				}
				SortCollection(collection);

				float4 current = 1;
				AddUnique(current, collection[0].x);
				AddUnique(current, collection[1].x);
				AddUnique(current, collection[2].x);
				AddUnique(current, collection[3].x);
				

#ifdef FORCE_SET_ALPHA_CHANNEL_FOR_DEBUG
				current.a = 1;//debug, now 3 layers in work
#endif

				return current;
			}
			ENDCG
		}
	}
}
